using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private PaintComponent[] paintsComponent;
    [SerializeField] private GameObject spidermanPaint;

    private void Start()
    {
        InitRandomPaintSpiderman();
    }
    
    private void InitRandomPaintSpiderman()
    {
        if (paintsComponent.Length == 0) return;
        
        int value = Random.Range(0,paintsComponent.Length);
        paintsComponent[value].gameObject.SetActive(false);
        
        Transform transformSpiderman =  paintsComponent[value].transform;
        spidermanPaint.transform.position =  transformSpiderman.position;
        spidermanPaint.transform.rotation =  transformSpiderman.rotation;

        if (!paintsComponent[value].isDoorAtRight)
        {
            spidermanPaint.transform.localScale = new Vector3(-spidermanPaint.transform.localScale.x, spidermanPaint.transform.localScale.y, spidermanPaint.transform.localScale.z);
        }

        paintsComponent[value].doorComponent.doorIDLinked = 2;
    }
}
