using UnityEngine;

public class RandomPaint : MonoBehaviour
{
    public DoorComponent doorComponent;
    
    [SerializeField] private bool initRandomAtStart = false;
    [SerializeField] private GameObject[] paintGO;
    [SerializeField] private GameObject spidermanPaint;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (initRandomAtStart) InitRandomPaint();
    }
    
    public void InitPaint(bool isSpiderman)
    {
        if (isSpiderman)
        {
            spidermanPaint.SetActive(true);
            foreach (GameObject go in paintGO)
                go.SetActive(false);
        }

        if (!isSpiderman)
        {
            InitRandomPaint();
        }
    }
    
    private void InitRandomPaint()
    {
        if (spidermanPaint) spidermanPaint.SetActive(false);
        
        int rand = Random.Range(0, paintGO.Length);
        for (int i = 0; i < paintGO.Length; i++)
        {
            paintGO[i].SetActive(i == rand);

            if (i == rand)
            {
                // case we are in flipped corridor
                if (PaintingFlipManagers.Instance != null)
                {
                    if (paintGO[i].TryGetComponent(out PaintingFlipComponent paintingFlip))
                    {
                        // flip paint if we need to
                        if (PaintingFlipManagers.Instance.numberPaintFlippedAtStart > 0)
                        {
                            PaintingFlipManagers.Instance.numberPaintFlippedAtStart--;
                            paintingFlip.SetIsFlipped(true);
                            paintingFlip.InitRotation();
                        }
                        else
                        {
                            paintingFlip.SetIsFlipped(false);
                        }
                    
                        PaintingFlipManagers.Instance.AddPaintToList(paintingFlip);
                    }
                }   
            }
        }
    }
}
