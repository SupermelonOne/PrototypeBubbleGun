using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]

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
    
    private Vector3 bridgeStart;
    private Vector3 bridgeEnd;
    
    private NavMeshLink startLink;
    private NavMeshLink endLink;
    
    private GameObject planksObject;
    
    private AudioSource audioSource;
    
    private NavMeshSurface surface;

    private void Start()
    {
        planksObject = new GameObject();
        planksObject.name = "Planks";
        planksObject.transform.SetParent(transform);
    
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.clip = soundEffect;
    
        bridgeStart = bridgeStartObject.transform.position;
        bridgeEnd = bridgeEndObject.transform.position;
    
        var startToEnd = (bridgeEnd - bridgeStart).normalized;
        startToEnd.y = 0; 
        bridgeEndObject.transform.rotation = Quaternion.LookRotation(startToEnd, Vector3.up);
        bridgeStartObject.transform.rotation = Quaternion.LookRotation(-startToEnd, Vector3.up);
    
        surface = GetComponent<NavMeshSurface>();
    
        LayerMask includedLayers = LayerMask.GetMask("Surface");

        startLink = bridgeStartObject.GetComponentInChildren<NavMeshLink>();
        endLink = bridgeEndObject.GetComponentInChildren<NavMeshLink>();
        


        if (startLink == null || endLink == null) return; // Exit if links are missing

        var plankRenderer = plankPrefab.GetComponent<Renderer>();
        if (plankRenderer == null) return; // Exit if plank prefab renderer is missing

        float plankTotalSize = plankRenderer.bounds.extents.z * 2f + emptySpace;
        int plankAmount = GetPlankAmount(bridgeStart, bridgeEnd, plankTotalSize);

        Vector3 groundStartLocal, groundEndLocal;
    
        if (TryGetHighestOverlapY(bridgeStartObject, includedLayers, out groundStartLocal))
        {
            startLink.startPoint = groundStartLocal;
            startLink.endPoint = bridgeStartObject.transform.InverseTransformPoint(GetPlankPosition(0));
        }

        if (TryGetHighestOverlapY(bridgeEndObject, includedLayers, out groundEndLocal))
        {
            endLink.startPoint = bridgeEndObject.transform.InverseTransformPoint(GetPlankPosition(plankAmount));
            endLink.endPoint = groundEndLocal;
        }
        
        
        startLink.enabled = false;
        endLink.enabled = false;

    }


    public void OnDrawGizmos()
    {
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
        }
    }





    public void OnBuildBridge()
    {
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null)
            return;
        var plankSize = renderer.bounds.extents.z + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
        StartCoroutine(BuildBridge(amount, .01f));
    }
    
    private IEnumerator BuildBridge(int amount, float delay)
    {
        var fallTime = 0.3f;
        var minPitch = 0.6f;
        var maxPitch = 2f;
        var pitchInterval = (maxPitch - minPitch) / amount;
        
        var distToMove = bridgeEnd - bridgeStart;
        var distPerStep = distToMove / amount;
        int material = 0;
        for (int i = 0; i < amount; i++)
        {
            if (material + 1 < plankMaterials.Length)
                material++;
            else material = 0;

            


            var startIndex = 0f;
            var endIndex = amount;
            var maxHeight = maxCurveHeight;

            var heightFactor = -4 * maxHeight;
            var indexRangeSquared = (endIndex - startIndex) * (endIndex - startIndex);
            var distanceFromEnds = (i - startIndex) * (i - endIndex);
            var heightOffset = heightFactor / indexRangeSquared * distanceFromEnds;
            var heightVector = new Vector3(0, heightOffset, 0);
            
            var fall = new Vector3(0,fallDist,0);
            var pos = bridgeStart + heightVector + distPerStep * i + fall;
            
            var plank = PlacePlank(pos, material);
            
            if (i % 2 == 0)
                PlaceFencePost(plank, material);
            
            var pitch = minPitch + pitchInterval * i;
            StartCoroutine(MovePlankDown(plank, fallDist, pitch, fallTime));
            yield return new WaitForSeconds(delay); 
        }
        yield return new WaitForSeconds(fallTime);
        surface.BuildNavMesh();
        startLink.enabled = true;
        endLink.enabled = true;
        
        startLink.UpdateLink();
        endLink.UpdateLink();
        
    }

    private Vector3 GetPlankPosition(int index)
    {
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null) return Vector3.zero;

        float plankHeight = renderer.bounds.extents.y * 2f;
        float plankDepth = renderer.bounds.extents.z * 2f;

        var plankSize = plankDepth + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
    
        var distPerStep = (bridgeEnd - bridgeStart) / amount;
    
        // Calculate height offset based on curve
        float t = (float)index / (amount - 1);
        float heightOffset = -4f * maxCurveHeight * (t - 0.5f) * (t - 0.5f) + maxCurveHeight;
    
        // Base position at the center of the plank's base
        Vector3 basePlankCenterPosition = bridgeStart + new Vector3(0, heightOffset, 0) + distPerStep * index;

        // Return the position at the top center of the plank
        return basePlankCenterPosition + new Vector3(0, plankHeight / 2f, 0);
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
        var plankRenderer = plank.GetComponent<Renderer>();
        var postRenderer = FencePostPrefab.GetComponent<Renderer>();
        
        var plankHeight = plankRenderer.bounds.max.y + postRenderer.bounds.extents.y;
        var centerX = plank.transform.position.x;
        var pos1z = plankRenderer.bounds.max.z - postRenderer.bounds.size.z;
        var pos2z = plankRenderer.bounds.min.z + postRenderer.bounds.size.z;
        
        var pos1 = new Vector3(centerX, plankHeight, pos1z);
        var pos2 = new Vector3(centerX, plankHeight, pos2z);
        
        var post1 = Instantiate(FencePostPrefab, pos1, Quaternion.identity);
        var post2 = Instantiate(FencePostPrefab, pos2, Quaternion.identity);
        
        post1.GetComponent<Renderer>().material = plankMaterials[matIndex];
        post2.GetComponent<Renderer>().material = plankMaterials[matIndex];
        
        
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

    public static bool TryGetHighestOverlapY(GameObject obj, LayerMask additionalExcludedLayers, out Vector3 localContactPoint)
    {
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
                    highestContactPoint = hit.point;
                    hitFound = true;
                }
            }
        }

        // Optional: SphereCast from corners for robustness (small radius for edge precision)
        // If you strictly want *only* edges and the raycast from corners is sufficient, remove this part.
        // However, for practical NavMeshLinks, a small sphere at corners can be more reliable.
        float sphereRadius = 0.05f; 
        foreach (Vector3 baseCornerPoint in cornerPoints)
        {
            Vector3 sphereOrigin = baseCornerPoint + Vector3.up * rayOriginHeightOffset;
            RaycastHit hit;
            if (Physics.SphereCast(sphereOrigin, sphereRadius, Vector3.down, out hit, raycastMaxDistance, includedMask))
            {
                if (hit.point.y > highestContactPoint.y)
                {
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
