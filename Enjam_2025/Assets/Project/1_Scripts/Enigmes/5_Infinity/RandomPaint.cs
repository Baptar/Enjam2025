using UnityEngine;

public class RandomPaint : MonoBehaviour
{
    [SerializeField] private bool isRandomPaint = true;
    [SerializeField] private GameObject[] paintGO;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isRandomPaint) InitRandomPaint();
    }

    private void InitRandomPaint()
    { 
        int rand = Random.Range(0, paintGO.Length);
        for (int i = 0; i < paintGO.Length; i++)
        {
            paintGO[i].SetActive(i == rand);
        }
    }
}
