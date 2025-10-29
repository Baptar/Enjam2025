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

    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (meshRenderer == null) return;
        if (meshRenderer.enabled) return;

        if (FrustumCheck.CanSpawnDoor(meshRenderer))
        {
            EnableElements(true);
            enabled = false;
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
