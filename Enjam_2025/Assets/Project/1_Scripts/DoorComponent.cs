using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class DoorComponent : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int doorIDLinked;
    public int doorNumber;
    [SerializeField] private bool randomNumberDoor;
    [SerializeField] private Transform corridorTransform;
    [SerializeField] private bool isInteractable;
    [SerializeField] private bool initDoorNumberAtStart = true;
    [SerializeField] private bool makeItDisappearAfterOpen = false;
    
    [Space(10)]
    [Header("Open Door")]
    [SerializeField] private float newRotateValue;
    [SerializeField] private float durationRotate;
    
    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject[] elementToRemoveWhenOpenDoor;
    [SerializeField] private TMP_Text doorNumberText;
    [SerializeField] private TMP_Text doorNumberBehindText;
    [SerializeField] private GameObject doorCenter;
    [SerializeField] private GameObject moveTarget;
    
    private Outline outline;
    private bool inZone = false;
    private float startRotateValue;
    
    private void Start()
    {
        outline = GetComponent<Outline>();
        startRotateValue = gameObject.transform.localEulerAngles.y;
        
        if (randomNumberDoor) doorNumber = UnityEngine.Random.Range(0, 99);
        if (initDoorNumberAtStart) doorNumberText.text = doorNumber.ToString();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            inZone = false;
            outline.enabled = false;
        }
    }


    /// <summary>
    /// Interface Functions
    /// </summary>
    public bool GetIsInteractable() => isInteractable && inZone;

    public void SetIsInteractable(bool value) => isInteractable = value;
    
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
        
        // generate new corridor
        GameObject corridor = GenerateCorridorAssociated();
        if (corridor == null) return;
        
        
        if (corridor.TryGetComponent(out CorridorGenerated corridorGenerated))
        {
            corridorGenerated.OnCorridorGenerated(doorNumber);
        }
    }
    
    private GameObject GenerateCorridorAssociated()
    {
        GameObject corridorGo = CorridorsManager.instance.GetCorridorAssociated(doorIDLinked);
        if (corridorGo == null)
        {
            Debug.Log("Corridor associated with doorIDLinked = " + doorIDLinked + " not found");
            return null;
        }

        if (doorIDLinked == 4)
        {
            InfinitySystem[] all = FindObjectsOfType<InfinitySystem>();
            foreach (InfinitySystem i in all)
                Destroy(i.gameObject, 3.0f);
        }
        GameObject go = Instantiate(corridorGo, corridorTransform.position, corridorTransform.rotation);
        if (go == null) return null;
        if (go.TryGetComponent(out InfinitySystem infinitySystem))
        {
            infinitySystem.Spawn5Corridors(go);
        }
        
        return go;
    }

    public void OpenDoor(CorridorGenerated corridorGenerated)
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
                if (makeItDisappearAfterOpen) 
                    // deactivate elements of door to make the new one appear at the same place
                    foreach (GameObject go in elementToRemoveWhenOpenDoor)
                        go.SetActive(false);
            }))
            // move door
            .Insert(1.7f,
                gameObject.transform
                    .DOLocalRotate(
                        new Vector3(gameObject.transform.localEulerAngles.x, startRotateValue,
                            gameObject.transform.localEulerAngles.z), durationRotate).SetEase(Ease.InOutFlash))
            // manage corridor Data
            .Insert(1.9f, DOVirtual.DelayedCall(0, () =>
            {
                CorridorsManager.instance.RemovePreviousCorridor();
                CorridorsManager.instance.SetPreviousCorridor(corridorGenerated.gameObject);
            }))
            // make this door interactable
            .Append(DOVirtual.DelayedCall(0, () =>
            {
                SetIsInteractable(true);
            }));

        seq.Play();
    }

    public void SetDoorNumberBehind(int number)
    {
        doorNumberBehindText.text = number.ToString();
    }
}
