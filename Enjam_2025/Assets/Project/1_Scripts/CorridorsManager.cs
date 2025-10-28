using UnityEngine;

public class CorridorsManager : MonoBehaviour
{
    public static CorridorsManager instance;
    [SerializeField] private GameObject[] corridorListID;

    [Header("DEBUG")]
    public GameObject previousCorridor;
    
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
}
