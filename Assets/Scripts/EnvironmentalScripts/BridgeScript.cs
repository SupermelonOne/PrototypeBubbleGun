using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshSurface))]
[RequireComponent(typeof(RectMeshBuilder))]
[RequireComponent(typeof(AudioSource))]

public class BridgeScript : MonoBehaviour
{
    [SerializeField] public GameObject bridgeStartObject;
    [SerializeField] public GameObject bridgeEndObject;
    [SerializeField] public GameObject plankPrefab;
    [SerializeField] public GameObject FencePostPrefab;
    
    [SerializeField] public AudioClip soundEffect;
    [SerializeField] public Material[] plankMaterials;

    [SerializeField] public float emptySpace = 0.1f;
    [SerializeField] public float fallDist;
    [SerializeField] public float maxCurveHeight;
    
    [SerializeField] private float monsterHoverHeight = .1f;
    
    [SerializeField] public bool ShowGizmos = true;
    
    private Vector3 bridgeStart;
    private Vector3 bridgeEnd;
    
    private NavMeshLink startLink;
    private NavMeshLink endLink;
    
    private NavMeshObstacle obstacle;
    
    private GameObject planksObject;
    
    private AudioSource audioSource;
    
    private NavMeshSurface surface;
    
    private RectMeshBuilder rectMeshBuilder;
    
    private GameObject groundStart, groundEnd;

    private Renderer plankRenderer;

    private void Awake()
    {
        InitializeComponents();
        
        planksObject = new GameObject();
        planksObject.name = "Planks";
        planksObject.transform.SetParent(transform);
    
        audioSource.clip = soundEffect;
        
        var startToEnd = (bridgeEnd - bridgeStart).normalized;
        startToEnd.y = 0; 
        bridgeEndObject.transform.rotation = Quaternion.LookRotation(startToEnd, Vector3.up);
        bridgeStartObject.transform.rotation = Quaternion.LookRotation(-startToEnd, Vector3.up);
        
        SetNavObstacles();
        RemoveMesh();
    }


    private void OnEnable()
    {
        //Debug.Log("Enabled");
        GameManager.Instance.AddBridge(this);
    }

    private void InitializeComponents()
    {
        //no need for null checks because these components are all required
        bridgeStart = bridgeStartObject.transform.position;
        bridgeEnd = bridgeEndObject.transform.position;
        
        rectMeshBuilder = GetComponent<RectMeshBuilder>();
        surface = GetComponent<NavMeshSurface>();
        audioSource = GetComponent<AudioSource>();
        
        startLink = bridgeStartObject.GetComponentInChildren<NavMeshLink>();
        endLink = bridgeEndObject.GetComponentInChildren<NavMeshLink>();
        
        obstacle = GetComponentInChildren<NavMeshObstacle>();
        
        plankRenderer = plankPrefab.GetComponent<Renderer>();
    }

    private void SetCollisionPoints()
    {
        LayerMask includedLayers = LayerMask.GetMask("Surface");

        if (startLink == null || endLink == null) return; // Exit if links are missing


        float plankTotalSize = plankRenderer.bounds.extents.z * 2f + emptySpace;
        int plankAmount = GetPlankAmount(bridgeStart, bridgeEnd, plankTotalSize);

        Vector3 groundStartLocal, groundEndLocal;
        
    
        if (TryGetHighestOverlapY(bridgeStartObject, includedLayers, out groundStartLocal, out groundStart))
        {
            Debug.Log(groundStart);
            startLink.startPoint = groundStartLocal;
            startLink.endPoint = bridgeStartObject.transform.InverseTransformPoint(GetPlankPosition(1) + new Vector3(0,plankRenderer.bounds.extents.y * 2,0));
        }

        if (TryGetHighestOverlapY(bridgeEndObject, includedLayers, out groundEndLocal, out groundEnd))
        {
            Debug.Log(groundEnd);
            endLink.startPoint = bridgeEndObject.transform.InverseTransformPoint(GetPlankPosition(plankAmount-1) +  new Vector3(0,plankRenderer.bounds.extents.y * 2,0));
            endLink.endPoint = groundEndLocal;
        }

    }


    public void OnDrawGizmos()
    {
        if (!ShowGizmos) return;
        bridgeStart = bridgeStartObject.transform.position;
        bridgeEnd = bridgeEndObject.transform.position;
        
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("No renderer");
            return;
        }

        float plankLength = renderer.bounds.extents.z * 2f + emptySpace;
        int amount = GetPlankAmount(bridgeStart, bridgeEnd, plankLength);
        

        Vector3 totalDirection = bridgeEnd - bridgeStart;
        Vector3 distPerStep = totalDirection / amount;

        float width = renderer.bounds.extents.x * 2f;
        float height = renderer.bounds.extents.y * 2f;
        float length = renderer.bounds.extents.z * 2f;
        
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < amount; i++)
        {
            float t = (float)i / (amount - 1);
            float heightOffset = -4f * maxCurveHeight * (t - 0.5f) * (t - 0.5f) + maxCurveHeight;
            Vector3 heightVector = new Vector3(0, heightOffset, 0);

            Vector3 position = bridgeStart + distPerStep * i + heightVector;
            
            var direction = (bridgeEnd - position).normalized;
            direction.y = 0; 
            var r = Quaternion.LookRotation(direction, Vector3.up);
            Gizmos.color = Color.green;
            Matrix4x4 matrix = Matrix4x4.TRS(position, r, Vector3.one);
            Gizmos.matrix = matrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, length));

            if (i % 3 == 0 || i >= amount-1 || i <= 1)
            {
                Vector3 localOffset1 = r * new Vector3(-width / 2f, height, 0); // left bottom corner
                Vector3 localOffset2 = r * new Vector3(width / 2f, height, 0);   // right top corner

                points.Add(position + localOffset1);
                points.Add(position + localOffset2);
            }
        }

        for (int i = 2; i < points.Count; i++)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(points[i], points[i - 2]);
        }
        

    }
    
    public void OnBuildBridge()
    {
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null)
            return;
        var plankSize = renderer.bounds.extents.z * 2 + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
        StartCoroutine(BuildBridge(amount, .01f));
        RemoveNavObstacles();
    }
    
    private IEnumerator BuildBridge(int amount, float delay)
    {
        var fallTime = 0.3f;
        var minPitch = 0.6f;
        var maxPitch = 2f;
        var pitchInterval = (maxPitch - minPitch) / amount;
        
        var distToMove = bridgeEnd - bridgeStart;
        var distPerStep = distToMove / amount;
        var material = 0;
        
        
        
        for (var i = 0; i < amount; i++)
        {
            if (material + 1 < plankMaterials.Length)
                material++;
            else material = 0;

            
            var fall = new Vector3(0,fallDist,0);
            var pos = GetPlankPosition(i);

            var plank = PlacePlank(pos + fall, material);
            
            if (i % 2 == 0)
                PlaceFencePost(plank, material);
            
            var pitch = minPitch + pitchInterval * i;
            StartCoroutine(MovePlankDown(plank, fallDist, pitch, fallTime));
            yield return new WaitForSeconds(delay); 
        }
        yield return new WaitForSeconds(fallTime);
        
        
    }

    public void SetNavMesh()
    {
        GameManager.Instance.AddBridge(this);
        Debug.Log("yeeee");
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Debug.Log("In edit mode");
            var gm = GameManager.Instance;
            if (gm != null)
            {
                Debug.Log(gm.name);
                gm.SetNavMesh();
            }
            else
            {
                Debug.LogWarning("No GameManager found in scene.");
            }
            return;
        }
#endif
        if (GameManager.Instance != null)
            GameManager.Instance.SetNavMesh();
        else
            Debug.LogWarning("GameManager.Instance is null.");
    }



    public void SetThisNav()
    {
        InitializeComponents();
        SetCollisionPoints();
        var plankTotalSize = plankRenderer.bounds.extents.z * 2f + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankTotalSize);
    
        var points = new List<Vector3>();
        var width = plankRenderer.bounds.extents.x * 2f;
        var height = plankRenderer.bounds.extents.y * 2f;
        
        var bothDirection = (bridgeEnd - bridgeStart).normalized;
        bothDirection.y = 0; 
        var rr = Quaternion.LookRotation(bothDirection, Vector3.up);
                
        Vector3 localOffset1r = rr * new Vector3(-width / 2f, height, 0); // left bottom corner
        Vector3 localOffset2r = rr * new Vector3(width / 2f, height, 0);   // right top corner
        
        
        points.Add(GetPlankPosition(0) +  new Vector3(0,plankRenderer.bounds.extents.y,0) + localOffset1r);
        points.Add(GetPlankPosition(0) +  new Vector3(0,plankRenderer.bounds.extents.y,0) + localOffset2r);
        
        Debug.Log(amount);
        for (int i = 0; i < amount; i++)
        {
            if (i % 3 == 0 || i >= amount-1 || i <= 1)
            {
                Debug.Log("shiiiii, testing number: " + i);
                var pos = GetPlankPosition(i);
                
                var direction = (bridgeEnd - pos).normalized;
                direction.y = 0; 
                var r = Quaternion.LookRotation(direction, Vector3.up);
                
                Vector3 localOffset1 = r * new Vector3(-width / 2f, height, 0); // left bottom corner
                Vector3 localOffset2 = r * new Vector3(width / 2f, height, 0);   // right top corner

                points.Add(pos + localOffset1);
                points.Add(pos + localOffset2);

            }
        }


        points.Add(GetPlankPosition(amount) -  new Vector3(0,plankRenderer.bounds.extents.y,0) + localOffset1r);
        points.Add(GetPlankPosition(amount) -  new Vector3(0,plankRenderer.bounds.extents.y,0) + localOffset2r);

        
        
        rectMeshBuilder.SetMesh(points);

    
        startLink.width = width;
        endLink.width = width;
        

    
        startLink.UpdateLink();
        endLink.UpdateLink();
    
        


        
        Transform obstacleTransform = obstacle.transform;

        // Center between start and end, in local space
        Vector3 localStart = obstacleTransform.InverseTransformPoint(bridgeStart);
        Vector3 localEnd = obstacleTransform.InverseTransformPoint(bridgeEnd);
        obstacle.center = (localEnd + localStart) / 2;

        // Size in local space: convert world-space vector to local direction
        Vector3 worldDiff = bridgeEnd - bridgeStart;
        Vector3 localDiff = obstacleTransform.InverseTransformVector(worldDiff);

        obstacle.size = new Vector3(
            width,
            maxCurveHeight * 2 + 3,
            Mathf.Abs(localDiff.magnitude)
        );
        
        obstacle.transform.rotation = Quaternion.LookRotation(worldDiff.normalized,Vector3.up);

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(surface);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(surface.gameObject.scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        #endif
    }

    public void RemoveMesh()
    {
        rectMeshBuilder.RemoveMesh();
    }

    private void SetNavObstacles()
    {
        startLink.enabled = false;
        endLink.enabled = false;
        
        obstacle.enabled = true;
    }

    private void RemoveNavObstacles()
    {
        startLink.enabled = true;
        endLink.enabled = true;
        
        obstacle.enabled = false;
        
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    private Vector3 GetPlankPosition(int index)
    {
        var rend = plankPrefab.GetComponent<Renderer>();
        if (rend == null) return Vector3.zero;

        var plankDepth = rend.bounds.extents.z * 2f;

        var plankSize = plankDepth + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
    
        var distPerStep = (bridgeEnd - bridgeStart) / amount;
    
        // Calculate height offset based on curve
        float t = (float)index / (amount - 1);
        float heightOffset = -4f * maxCurveHeight * (t - 0.5f) * (t - 0.5f) + maxCurveHeight;
    
        // Base position at the center of the plank's base
        Vector3 basePlankCenterPosition = bridgeStart + new Vector3(0, heightOffset, 0) + distPerStep * index;

        // Return the position at the top center of the plank
        return basePlankCenterPosition;
    }


    private GameObject PlacePlank(Vector3 pos, int matIndex)
    {
        var plank = Instantiate(plankPrefab, pos, Quaternion.identity);
        plank.GetComponent<Renderer>().material = plankMaterials[matIndex];
        plank.transform.parent = planksObject.transform;
        var direction = (bridgeEnd - plank.transform.position).normalized;
        direction.y = 0; 
        plank.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        return plank;
    }

    private void PlaceFencePost(GameObject plank, int matIndex)
    {
        var postRenderer = FencePostPrefab.GetComponent<Renderer>();
        var plankTransform = plank.transform;
        var halfLength = plankTransform.localScale.x / 2;
        
        var center = plankTransform.position;
        var offset = plankTransform.right * halfLength;
        
        float postHeight = postRenderer.bounds.extents.y;
        Vector3 upOffset = Vector3.up * postHeight;
        
        Vector3 pos1 = center + offset + upOffset;
        Vector3 pos2 = center - offset + upOffset;
        
        var post1 = Instantiate(FencePostPrefab, pos1, Quaternion.identity);
        var post2 = Instantiate(FencePostPrefab, pos2, Quaternion.identity);
        
        post1.GetComponent<Renderer>().material = plankMaterials[matIndex];
        post2.GetComponent<Renderer>().material = plankMaterials[matIndex];
        
        post1.transform.SetParent(transform, true);
        post2.transform.SetParent(transform, true);
        
        
        StartCoroutine(MovePlankDown(post1, fallDist, 0));
        StartCoroutine(MovePlankDown(post2, fallDist, 0));
    }


    private IEnumerator MovePlankDown(GameObject plank, float distance, float pitch, float duration = 0.2f)
    {
        Vector3 startPos = plank.transform.position;
        Vector3 endPos = startPos - new Vector3(0, distance, 0);
        float elapsed = 0f;
        

        while (elapsed < duration)
        {
            plank.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        audioSource.pitch = pitch * Random.Range(0.99f, 1.01f);
        audioSource.Play();
        
        plank.transform.position = endPos;
    }
    
    public void ClearBridge()
    {
        var children = planksObject.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if(child != planksObject.transform)
                DestroyImmediate(child.gameObject);
        }
    }

    private int GetPlankAmount(Vector3 startPos, Vector3 endPos, float plankSize)
    {
        var distance = Vector3.Distance(startPos, endPos);
        return Mathf.FloorToInt(distance / plankSize);
    }
    

    public static bool TryGetHighestOverlapY(GameObject obj, LayerMask additionalExcludedLayers, out Vector3 localContactPoint, out GameObject otherSurface)
    {
        otherSurface = null;

        localContactPoint = Vector3.zero;
        var objTransform = obj.transform;
        var objCollider = obj.GetComponent<Collider>();

        if (objCollider == null) return false;

        LayerMask combinedExcludedLayers = additionalExcludedLayers;
        int includedMask = combinedExcludedLayers.value;

        Vector3 highestContactPoint = Vector3.negativeInfinity;
        bool hitFound = false;


        Vector3 center = objCollider.bounds.center;
        Vector3 extents = objCollider.bounds.extents;

        // Define the 4 corners of the object's bottom face in world space
        Vector3[] cornerPoints = new Vector3[4];
        cornerPoints[0] = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z); // Min X, Min Z
        cornerPoints[1] = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z); // Max X, Min Z
        cornerPoints[2] = new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z); // Min X, Max Z
        cornerPoints[3] = new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z); // Max X, Max Z

        // Raycast max distance: from origin (above object) down to well below object's bottom
        float raycastMaxDistance = (extents.y * 2f) + 2f; // Height of object + 2 units buffer
        float rayOriginHeightOffset = (extents.y * 2f) + 1f; // Lift origin by full height + 1 unit buffer

        foreach (Vector3 baseCornerPoint in cornerPoints)
        {
            Vector3 rayOrigin = baseCornerPoint + Vector3.up * rayOriginHeightOffset;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastMaxDistance, includedMask))
            {
                if (hit.point.y > highestContactPoint.y)
                {
                    otherSurface = hit.collider.gameObject;
                    Debug.Log(otherSurface.name);
                    highestContactPoint = hit.point;
                    hitFound = true;
                }
            }
        }

        float sphereRadius = 0.05f; 
        foreach (Vector3 baseCornerPoint in cornerPoints)
        {
            Vector3 sphereOrigin = baseCornerPoint + Vector3.up * rayOriginHeightOffset;
            RaycastHit hit;
            if (Physics.SphereCast(sphereOrigin, sphereRadius, Vector3.down, out hit, raycastMaxDistance, includedMask))
            {
                if (hit.point.y > highestContactPoint.y)
                {
                    otherSurface = hit.collider.gameObject;
                    Debug.Log(otherSurface.name);
                    highestContactPoint = hit.point;
                    hitFound = true;
                }
            }
        }

        if (hitFound)
        {
            localContactPoint = objTransform.InverseTransformPoint(highestContactPoint);
            return true;
        }

        return false;
    }



}
