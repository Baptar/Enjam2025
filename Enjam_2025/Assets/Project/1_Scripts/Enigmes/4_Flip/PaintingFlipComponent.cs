using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PaintingFlipComponent : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isFlipped;
    [SerializeField] private bool isInteractable = false;
    
    private Outline outline;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void InitRotation()
    {
        if (isFlipped) gameObject.transform.Rotate(0, 0, 180);
    }
    
    public bool GetIsFlipped() => isFlipped;
    
    public void SetIsFlipped(bool value) => isFlipped = value;


    public bool GetIsInteractable()
    {
        return isInteractable;
    }

    public void SetIsInteractable(bool value)
    {
        isInteractable =  value;
    }

    public string GetInteractPrompt()
    {
        return "";
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!GetIsInteractable()) return;
        
        AudioManager.instance.PlaySoundInteract();
        AudioManager.instance.PlaySoundInteractPaint();
        
        Vector3 newEulerRoration =new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y,
            (gameObject.transform.localEulerAngles.z - 180) % 360);
        gameObject.transform.DOLocalRotate(newEulerRoration, 1.2f).SetEase(Ease.InOutFlash)
            .OnStart(()=>
            {
                SetIsInteractable(false);
                outline.enabled = false;
            })
            .OnComplete((() =>
            {
                SetIsInteractable(true);
                isFlipped = !isFlipped;
                if (PaintingFlipManagers.Instance == null) return;
                PaintingFlipManagers.Instance.CheckPaintingFlipped();
            }));
    }

    public void OnHovered(PlayerInteractor interactor)
    {
        if (GetIsInteractable()) outline.enabled = true;
    }

    public void OnReleased()
    {
        outline.enabled = false;
    }
}
