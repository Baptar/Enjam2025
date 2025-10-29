using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class RadioComponent : MonoBehaviour, IInteractable
{
    [Header("Parameters")]
    [SerializeField] private bool isInteractable = false;

    [Space(10)]
    [Header("References")]
    [SerializeField] private Transform radioInspectPosition;
    [SerializeField] private GameObject indicatorGO;
    [SerializeField] private Transform[] channelsPosition;
    
    [Space(10)]
    [Header("Debug")]
    public E_RadioState actualRadio = E_RadioState.First;
    public E_RadioState solutionRadio;
    
    private Camera playerCam;
    private Outline outline;
    private bool isInspectingRadio = false;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    

    void Start()
    {
        outline = GetComponent<Outline>();
        playerCam = PlayerController.instance.playerCamera;
        outline.enabled = false;
    }

    private void Update()
    {
        if (isInspectingRadio)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeBeforeChannel();
            }
            
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeNextChannel();
            }
        }
    }
    
    /// <summary>
    /// Interface Functions
    /// </summary>
    public bool GetIsInteractable() => isInteractable;

    public void SetIsInteractable(bool value) => isInteractable = value;

    public string GetInteractPrompt() => "";

    public void Interact(PlayerInteractor interactor)
    {
        if (!GetIsInteractable()) return;
        
        if (!isInspectingRadio) 
            ZoomOnRadio();
        
        else ExitInspectMode();
    }

    public void OnHovered(PlayerInteractor interactor)
    {
        if (GetIsInteractable() && !isInspectingRadio)
            outline.enabled = true;
    }

    public void OnReleased() => outline.enabled = false;
    
    
    /// <summary>
    /// FUNCTIONS
    /// </summary>
    private void ZoomOnRadio()
    {
        isInspectingRadio = true;
        outline.enabled = false;
        PlayerController.instance.isInspectingRadio = true;
        originalCamPos = playerCam.transform.position;
        originalCamRot = playerCam.transform.rotation;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(playerCam.transform.DOMove(radioInspectPosition.position, 1.0f).SetEase(Ease.InOutFlash))
            .Insert(0.0f, playerCam.transform.DORotate(radioInspectPosition.eulerAngles, 1.0f).SetEase(Ease.InOutFlash));
    }
    
    private void ExitInspectMode()
    {
        isInspectingRadio = false;
        PlayerController.instance.isInspectingRadio = false;
        /*Sequence seq = DOTween.Sequence();
        seq.Append(playerCam.transform.DOMove(originalCamPos, 0.5f).SetEase(Ease.InOutFlash))
            .Insert(0.0f, playerCam.transform.DORotate(originalCamRot.eulerAngles, 0.5f).SetEase(Ease.InOutFlash))
            .Insert(0.7f, DOVirtual.DelayedCall(0, () =>
            {
                SetIsInteractable(true);
            }));*/
    }

    [ContextMenu("ChangeNextChannel")]
    private void ChangeNextChannel()
    {
        if (actualRadio == E_RadioState.Third) actualRadio = E_RadioState.First;
        else actualRadio++;

        indicatorGO.transform.DOMove(channelsPosition[(int)actualRadio].transform.position, 0.7f).SetEase(Ease.InOutFlash)
            .OnComplete(()=>
        {
            if (actualRadio == solutionRadio)
            {
                // WWISE good sound
            }
            else
            {
                // WWISE bad sound
            }
        });
    }

    private void ChangeBeforeChannel()
    {
        if (actualRadio == E_RadioState.First) actualRadio = E_RadioState.Third;
        else actualRadio--;

        indicatorGO.transform.DOMove(channelsPosition[(int)actualRadio].transform.position, 0.7f).SetEase(Ease.InOutFlash)
            .OnComplete(()=>
        {
            if (actualRadio == solutionRadio)
            {
                // WWISE good sound
            }
            else
            {
                // WWISE bad sound
            }
        });
    }
}
