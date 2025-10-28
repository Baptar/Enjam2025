using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class DoorComponent : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private int doorIDLinked;
    [SerializeField] private Transform coridorTransform;
    [SerializeField] private bool isInteratable;
    
    [Space(10)]
    [Header("Open Door")]
    [SerializeField] private float newRotateValue;
    [SerializeField] private GameObject[] elementToRemoveWhenOpenDoor;
    [SerializeField] private float durationRotate;
    [SerializeField] private GameObject doorCenter;
    [SerializeField] private GameObject moveTarget;
    
    private Outline outline;
    private float startRotateValue;
    
    private void Start()
    {
        outline = GetComponent<Outline>();
        startRotateValue = gameObject.transform.localEulerAngles.y;
    }
    

    /// <summary>
    /// Interface Functions
    /// </summary>
    public bool GetIsInteractable() => isInteratable;

    public void SetIsInteractable(bool value) => isInteratable = value;
    
    public string GetInteractPrompt() => "Open Door";

    public void Interact(PlayerInteractor interactor)
    {
        if (GetIsInteractable()) OnInteract();
    }

    public void OnHovered(PlayerInteractor interactor)
    {
        if (GetIsInteractable()) outline.enabled = true;
    }

    public void OnReleased() => outline.enabled = false;
    
    
    /// <summary>
    /// Functions
    /// </summary>
    private void OnInteract()
    {
        // can't interact with anymore
        SetIsInteractable(false);
        
        // deactivate elements of door to make the new one appear at the same place
        foreach (GameObject go in elementToRemoveWhenOpenDoor)
            go.SetActive(false);
        
        // generate new coridor
        GameObject coridor = GenerateCoridorAssociated();
        if (coridor == null) return;
        
        
        if (coridor.TryGetComponent(out CoridorGenerated coridorGenerated))
        {
            coridorGenerated.OnCoridorGenerated();
        }
    }
    
    private GameObject GenerateCoridorAssociated()
    {
        GameObject coridorGo = DoorsManager.instance.GetCoridorAssociated(doorIDLinked);
        if (coridorGo == null)
        {
            Debug.Log("Coridor associated with doorIDLinked = " + doorIDLinked + " not found");
            return null;
        }
        
        return Instantiate(coridorGo, coridorTransform.position, coridorTransform.rotation);
    }

    public void OpenDoor(CoridorGenerated coridorGenerated)
    {
        PlayerController playerController = PlayerController.instance;
        
        // remove player input
        playerController.enabled = false;
        
        // remove player look (to fix an issue camera position comes back)
        playerController.blockCamera = true;
        
        // look center of door
        Transform playerTransform = playerController.gameObject.transform;
        Vector3 lookDirection = moveTarget.transform.position - playerTransform.position;
        lookDirection.y = 0f; // ignore verticality
        Quaternion targetRot = Quaternion.LookRotation(lookDirection);
        Vector3 targetEuler = targetRot.eulerAngles;
        float rotationDuration = 0.8f;
        Ease rotationEase = Ease.InOutSine;
        playerTransform.DORotate(targetEuler, rotationDuration, RotateMode.Fast)
            .SetEase(rotationEase);
        

        Sequence seq = DOTween.Sequence();
        // open door
        seq.Append(gameObject.transform.DOLocalRotate(new Vector3(gameObject.transform.localEulerAngles.x, newRotateValue, gameObject.transform.localEulerAngles.z), durationRotate).SetEase(Ease.InOutFlash));
        
        // move player
        Vector3 targetLocalPosition = new Vector3(moveTarget.transform.position.x, playerController.gameObject.transform.position.y, moveTarget.transform.position.z);
        seq.Insert(0.7f,
                playerController.gameObject.transform.DOMove(targetLocalPosition, 2.0f)
                    .SetEase(Ease.InOutSine))
            // add player input
            .Insert(2.0f, DOVirtual.DelayedCall(0, () =>
            {
                playerController.enabled = true; 
                PlayerController.instance.SetYaw(targetEuler.y);
                playerController.blockCamera = false;  
            }))
            // move door
            .Insert(1.7f,
                gameObject.transform
                    .DOLocalRotate(
                        new Vector3(gameObject.transform.localEulerAngles.x, startRotateValue,
                            gameObject.transform.localEulerAngles.z), durationRotate).SetEase(Ease.InOutFlash))
            // manage coridor Data
            .Append(DOVirtual.DelayedCall(0, () =>
            {
                DoorsManager.instance.RemovePreviousCoridor();
                DoorsManager.instance.SetPreviousCoridor(coridorGenerated.gameObject);
            }))
            // make this door interactable
            .Append(DOVirtual.DelayedCall(0, () =>
            {
                SetIsInteractable(true);
            }));

        seq.Play();
    }
}
