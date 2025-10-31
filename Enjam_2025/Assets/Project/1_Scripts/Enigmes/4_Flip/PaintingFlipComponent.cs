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
        if (isFlipped) gameObject.transform.Rotate(0, 0, 180);
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }
    
    public bool GetIsFlipped() => isFlipped;


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
        if (PaintingFlipManagers.Instance == null) return;
        
        
        AudioManager.instance.PlaySoundInteract();
        AudioManager.instance.PlaySoundInteractPaint();
        
        Vector3 newEulerRoration =new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y,
            (gameObject.transform.localEulerAngles.z - 180) % 360);
        gameObject.transform.DOLocalRotate(newEulerRoration, 2.0f).SetEase(Ease.InOutFlash)
            .OnComplete((() =>
            {
                isFlipped = !isFlipped;
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
