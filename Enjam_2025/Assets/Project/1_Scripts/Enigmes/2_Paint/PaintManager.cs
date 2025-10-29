using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private PaintComponent[] paintsComponent;

    private void Start()
    {
        InitRandomPaintSpiderman();
    }
    
    private void InitRandomPaintSpiderman()
    {
        if (paintsComponent.Length == 0) return;
        
        int value = Random.Range(0,paintsComponent.Length);
        Debug.Log(value);
        
        paintsComponent[value].DisplaySpiderman();
        paintsComponent[value].doorComponent.doorIDLinked = 2;
    }
}
