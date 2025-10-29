using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaintingFlipManagers : MonoBehaviour
{
    public static PaintingFlipManagers Instance;
    
    [SerializeField] private PaintingFlipComponent[] paintingComponents;
    [SerializeField] private TMP_Text doorText;
    [SerializeField] private GameObject corridorGo;
    [SerializeField] private float durationRotate = 5.0f;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private DoorComponent doorEnd;

    private void Awake() => Instance = this;

    private void Start()
    {
        doorText.text = "End";
        doorEnd.SetIsInteractable(false);
    }
    public void CheckPaintingFlipped()
    {
        foreach (PaintingFlipComponent paintingComponent in paintingComponents)
        {
            if (paintingComponent.GetIsFlipped())
                return;
        }

        FlipRoom();
    }

    private void FlipRoom()
    {
        AudioManager.instance.PlaySoundFlipRoom();
        
        Debug.Log("Flip Room");
        ChangeTextDoor();
        corridorGo.transform.DOLocalRotate(new Vector3(-180, corridorGo.transform.localEulerAngles.y, corridorGo.transform.localEulerAngles.z), durationRotate).SetEase(ease)
            .OnComplete(() =>
            {
                doorEnd.SetIsInteractable(true);
            });
    }

    private void ChangeTextDoor()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(doorText.DOFade(0, 1.0f))
            .AppendCallback(() => doorText.text = "Start").SetEase(ease)
            .Insert(0.0f, doorText.DOFade(1, 1.0f));
    }
}
