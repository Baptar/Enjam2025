using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class RadioComponent : MonoBehaviour, IInteractable
{
    public enum E_RadioState
    {
        First,
        Second,
        Third,
    }
    
    [Header("Parameters")]
    [SerializeField] private bool isInteractable = false;

    [Space(10)]
    [Header("References")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform radioInspectPosition;
    [SerializeField] private GameObject indicatorGO;
    [SerializeField] private Transform[] channelsPosition;
    
    [Space(10)]
    [Header("Debug")]
    public E_RadioState actualRadio = E_RadioState.First;
    public E_RadioState solutionRadio;
    
    private Outline outline;
    private bool isInspectingRadio = false;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        InitRandomSolution();
    }

    private void Update()
    {
        if (isInspectingRadio)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeBeforeChannel();
            }
            
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeNextChannel();
            }
        }
    }
    
    private void InitRandomSolution() => solutionRadio = (E_RadioState)Random.Range(0,3);
    
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
        if (GetIsInteractable())
            outline.enabled = true;
    }

    public void OnReleased() => outline.enabled = false;

    private void ZoomOnRadio()
    {
        SetIsInteractable(false);
        outline.enabled = false;
        isInspectingRadio = true;
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
        Sequence seq = DOTween.Sequence();
        seq.Append(playerCam.transform.DOMove(originalCamPos, 3.0f).SetEase(Ease.InOutFlash))
            .Insert(0.0f, playerCam.transform.DORotate(originalCamRot.eulerAngles, 3.0f).SetEase(Ease.InOutFlash))
            .OnComplete(()=>{SetIsInteractable(true);});
    }

    private void ChangeNextChannel()
    {
        
        switch (actualRadio)
        {
            case E_RadioState.First:
                    
                break;
            
            case E_RadioState.Second:
                
                break;
            
            case E_RadioState.Third:
                
                break;
        }
    }

    private void ChangeBeforeChannel()
    {
        
    }
}
