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
    [SerializeField] private CorridorGenerated corridorGenerated;
    
    private Outline outline;
    private bool inZone = false;
    
    private void Start()
    {
        outline = GetComponent<Outline>();
        
        if (randomNumberDoor) doorNumber = UnityEngine.Random.Range(0, 99);
        if (initDoorNumberAtStart) InitTextDoorWithNumber();
    }

    public void InitTextDoorWithNumber()
    {
        doorNumberText.text = doorNumber.ToString();
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
            corridorGenerated.OnCorridorGenerated(doorNumber, doorIDLinked != 4);
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
        
        Vector3 rot = corridorTransform.rotation.eulerAngles;
        rot.z = 0;
        
        GameObject go = Instantiate(corridorGo, corridorTransform.position, Quaternion.Euler(rot));
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
        
        // remove sound FS
        AudioManager.instance.StopPlaySoundFootStep();
        
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
        

        // --- rotation de la porte ---
        Quaternion startRot = transform.localRotation;
        Quaternion openRot = Quaternion.Euler(startRot.eulerAngles.x, newRotateValue, startRot.eulerAngles.z); // rotation ouverte
        
        //AudioManager.instance.PlaySoundDoorOpen();
        Sequence seq = DOTween.Sequence();
        // open door
        seq.Append(transform.DOLocalRotateQuaternion(openRot, durationRotate).SetEase(Ease.InOutFlash));
        
        // move player
        Vector3 targetLocalPosition = new Vector3(moveTarget.transform.position.x, playerController.gameObject.transform.position.y, moveTarget.transform.position.z);
        seq.Insert(0.7f, playerController.gameObject.transform.DOMove(targetLocalPosition, 2.0f)
            .SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                AudioManager.instance.PlaySoundFootStep();
            }))
            // add player input
            .Insert(2.0f, DOVirtual.DelayedCall(0, () =>
            {
                playerController.enabled = true; 
                playerController.SetYaw(targetEuler.y);
                playerController.blockCamera = false;  
                if (makeItDisappearAfterOpen) 
                    // deactivate elements of door to make the new one appear at the same place
                    foreach (GameObject go in elementToRemoveWhenOpenDoor)
                        go.SetActive(false);
            }))
            // close door
            .Insert(1.7f,
                transform.DOLocalRotateQuaternion(startRot, durationRotate / 2).SetEase(Ease.InOutFlash))
            // manage corridor Data
            .Insert(1.9f, DOVirtual.DelayedCall(0, () =>
            {
                corridorGenerated.MakeAfterElementAppear(true);
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
