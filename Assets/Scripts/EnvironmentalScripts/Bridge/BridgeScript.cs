using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private GameObject planksObject;
    
    private AudioSource audioSource;

    private void Start()
    {
        planksObject = new GameObject();
        planksObject.name = "Planks";
        planksObject.transform.SetParent(transform);
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundEffect;
        bridgeStart = bridgeStartObject.transform.position;
        bridgeEnd = bridgeEndObject.transform.position;
    }
    
    

    public void OnBuildBridge()
    {
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null)
            return;
        var plankSize = renderer.bounds.extents.z + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
        BuildBridge(amount, plankSize);
    }
    
    private void BuildBridge(int amount, float size)
    {
        var distToMove = bridgeEnd - bridgeStart;
        var distPerStep = distToMove / amount;
        for (int i = 0; i < amount; i++)
        {
            var pos = bridgeStart + distPerStep * i;
            var plank = Instantiate(plankPrefab, pos, Quaternion.identity);
            plank.transform.parent = transform;
        }
    }
    
    public void OnBuildBridgeWithDelay()
    {
        var renderer = plankPrefab.GetComponent<Renderer>();
        if (renderer == null)
            return;
        var plankSize = renderer.bounds.extents.z + emptySpace;
        var amount = GetPlankAmount(bridgeStart, bridgeEnd, plankSize);
        StartCoroutine(BuildBridgeWithDelay(amount, .01f));
    }
    
    private IEnumerator BuildBridgeWithDelay(int amount, float delay)
    {
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
            StartCoroutine(MovePlankDown(plank, fallDist, pitch));
            yield return new WaitForSeconds(delay); 
        }
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

}
