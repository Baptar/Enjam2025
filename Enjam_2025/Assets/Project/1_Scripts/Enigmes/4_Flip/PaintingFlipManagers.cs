using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaintingFlipManagers : MonoBehaviour
{
    public static PaintingFlipManagers Instance;
    
    [SerializeField] private TMP_Text doorText;
    [SerializeField] private GameObject corridorGo;
    [SerializeField] private float durationRotate = 5.0f;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private DoorComponent doorEnd;
    public int numberPaintFlippedAtStart;

    public List<PaintingFlipComponent> paintingComponents = new List<PaintingFlipComponent>();
    private bool isFlipped = true;
    private bool isFlipping = false;
    
    private void Awake() => Instance = this;

    private void Start()
    {
        doorText.text = "END";
        doorEnd.SetIsInteractable(false);
    }
    
    public void CheckPaintingFlipped()
    {
        if (isFlipping) return;
        
        foreach (PaintingFlipComponent paintingComponent in paintingComponents)
        {
            if (paintingComponent.GetIsFlipped())
            {
                if (!isFlipped) FlipRoom();
                return;
            }
        }

        FlipRoom();
    }

    private void FlipRoom()
    {
        isFlipping = true;
        AudioManager.instance.PlaySoundFlipRoom();
        
        Debug.Log("Flip Room");
        ChangeTextDoor();
        corridorGo.transform.DOLocalRotate(new Vector3(-180, corridorGo.transform.localEulerAngles.y, corridorGo.transform.localEulerAngles.z), durationRotate).SetEase(ease)
            .OnComplete(() =>
            {
                isFlipping = false;
                doorEnd.SetIsInteractable(true);
                AudioManager.instance.StopSoundFlipRoom();
                isFlipped = !isFlipped;
            });
    }

    private void ChangeTextDoor()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(doorText.DOFade(0, 1.0f))
            .AppendCallback(() => doorText.text = "START").SetEase(ease)
            .Insert(0.0f, doorText.DOFade(1, 1.0f));
    }

    public void AddPaintToList(PaintingFlipComponent paintingComponent)
    {
        paintingComponents.Add(paintingComponent);
    }
}
