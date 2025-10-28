using UnityEngine;

public class DoorsManager : MonoBehaviour
{
    public static DoorsManager instance;
    [SerializeField] private GameObject[] coridorListID;

    [Header("DEBUG")]
    public GameObject previousCoridor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() => instance = this;

    public GameObject GetCoridorAssociated(int ID)
    {
        if (ID < 0 || ID >= coridorListID.Length) return null;
        
        return coridorListID[ID];
    }

    public void SetPreviousCoridor(GameObject newCoridor)
    {
        previousCoridor = newCoridor;
    }
    
    public void RemovePreviousCoridor()
    {
        if (previousCoridor == null) return;
        Destroy(previousCoridor);
        previousCoridor = null;
    }
}
