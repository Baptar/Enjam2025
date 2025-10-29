using UnityEngine;

public class PaintComponent : MonoBehaviour
{
    public DoorComponent doorComponent;
    [SerializeField] private GameObject spidermanPaint;
    
    public void DisplaySpiderman()
    {
        Debug.Log("active spiderman paint");
        //gameObject.SetActive(false);
        spidermanPaint.SetActive(true);
    }
    
}
