using UnityEngine;


[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class TriggerInfinityCorridor : MonoBehaviour
{
    [SerializeField] private GameObject doorGO;
    [SerializeField] private CorridorGenerated corridorGenerated;
    
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    
    private bool spawnedNewCorridor = false;
    
    private bool initialized = false;
    private Vector3 lookDir;

    private void Start()
    {
        lookDir = PlayerController.instance.transform.position - transform.position;
        lookDir.y = 0; // ignore la hauteur

        initialized = true;
    }

    private void Update()
    {
        if (meshRenderer == null) return;
        if (meshRenderer.enabled) return;
        if (!initialized) return;
        
        //Debug.Log(FrustumCheck.IsBehind(meshRenderer.gameObject.transform, PlayerController.instance.playerCamera.gameObject.transform.position, Vector3.right));
        
        if (FrustumCheck.CanSpawnDoor(meshRenderer, lookDir.normalized))
        {
            EnableElements(true);
            enabled = false;
            corridorGenerated.MakeAfterElementAppear(true);
            corridorGenerated.RemovePreviousCorridor();
        }
    }

    public void EnableElements(bool enable)
    {
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        meshCollider.enabled = enable;
        meshRenderer.enabled = enable;
        doorGO.SetActive(enable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnedNewCorridor) return;
        
        if (other.gameObject.GetComponent<PlayerController>())
        {
            spawnedNewCorridor = true;
            CorridorsManager.instance.SpawnNewInfinityCorridor();
        }
    }
}
