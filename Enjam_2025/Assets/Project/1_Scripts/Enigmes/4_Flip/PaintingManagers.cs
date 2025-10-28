using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaintingManagers : MonoBehaviour
{
    public static PaintingManagers Instance;
    
    [SerializeField] private PaintingComponent[] paintingComponents;
    [SerializeField] private TMP_Text doorText;
    [SerializeField] private GameObject corridorGo;
    [SerializeField] private float durationRotate = 5.0f;
    [SerializeField] private Ease ease = Ease.Linear;

    private void Awake() => Instance = this;

    private void Start()
    {
        doorText.text = "End";
    }
    public void CheckPaintingFlipped()
    {
        foreach (PaintingComponent paintingComponent in paintingComponents)
        {
            if (paintingComponent.GetIsFlipped())
                return;
        }

        FlipRoom();
    }

    private void FlipRoom()
    {
        Debug.Log("Flip Room");
        ChangeTextDoor();
        corridorGo.transform.DOLocalRotate(new Vector3(-180, corridorGo.transform.localEulerAngles.y, corridorGo.transform.localEulerAngles.z), durationRotate).SetEase(ease);
    }

    private void ChangeTextDoor()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(doorText.DOFade(0, 1.0f))
            .AppendCallback(() => doorText.text = "Start").SetEase(ease)
            .Insert(0.0f, doorText.DOFade(1, 1.0f));
    }
}
