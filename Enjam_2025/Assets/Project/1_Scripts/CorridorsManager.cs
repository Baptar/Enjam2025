using UnityEngine;
using UnityEngine.UI;

public class CorridorsManager : MonoBehaviour
{
    public static CorridorsManager instance;
    [SerializeField] private GameObject[] corridorListID;
    
    [HideInInspector] public Transform nextTransformCorridorSpawnINFINITY;
    [HideInInspector] public GameObject lastInfinityCorridorCreated;

    [Header("DEBUG")]
    public GameObject previousCorridor;
    public RawImage endScreen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() => instance = this;

    public GameObject GetCorridorAssociated(int ID)
    {
        if (ID < 0 || ID >= corridorListID.Length) return null;
        
        return corridorListID[ID];
    }

    public void SetPreviousCorridor(GameObject newCoridor)
    {
        previousCorridor = newCoridor;
    }
    
    public void RemovePreviousCorridor()
    {
        if (previousCorridor == null) return;
        Destroy(previousCorridor);
        previousCorridor = null;
    }






    public void SpawnNewInfinityCorridor()
    {
        if (nextTransformCorridorSpawnINFINITY == null) return;
        
        GameObject go = Instantiate(GetCorridorAssociated(4), nextTransformCorridorSpawnINFINITY.position, nextTransformCorridorSpawnINFINITY.rotation);
        if (go.TryGetComponent(out CorridorGenerated corridorGenerated))
        {
            nextTransformCorridorSpawnINFINITY = corridorGenerated.lastSpawnPointNEXT.transform;
            
            corridorGenerated.previousCorridor = lastInfinityCorridorCreated;
            lastInfinityCorridorCreated = go;
        }
        if (go.TryGetComponent(out HelperInfinity inj))
        {
            inj.RemoveElement();
        }
        Debug.Log("Spawn new corridor infinit !");
    }
}
