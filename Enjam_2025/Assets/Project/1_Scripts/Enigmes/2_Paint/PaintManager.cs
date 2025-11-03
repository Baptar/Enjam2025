using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private RandomPaint[] paintsComponent;

    private void Start()
    {
        InitRandomPaintSpiderman();
        AudioManager.instance.StopRadio();
    }
    
    private void InitRandomPaintSpiderman()
    {
        if (paintsComponent.Length == 0) return;
        
        int value = Random.Range(0,paintsComponent.Length);

        for (int i = 0; i < paintsComponent.Length; i++)
        {
            paintsComponent[i].InitPaint(i == value);
            if (i == value) paintsComponent[i].doorComponent.doorIDLinked = 2;
        }
    }
}
