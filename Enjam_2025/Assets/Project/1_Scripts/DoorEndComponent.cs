using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class DoorEndComponent : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private Transform corridorTransform;
    [SerializeField] private bool isInteractable;
    
    [Space(10)]
    [Header("Open Door")]
    [SerializeField] private float newRotateValue;
    [SerializeField] private float durationRotate;
    
    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject doorCenter;
    [SerializeField] private GameObject moveTarget;
    
    private Outline outline;
    private bool inZone = false;
    private float startRotateValue;
    
    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        startRotateValue = gameObject.transform.localEulerAngles.y;
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
        AudioManager.instance.PlaySoundInteract();
        
        // can't interact with anymore
        SetIsInteractable(false);

        OpenDoor();
    }
    

    public void OpenDoor()
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
        seq.Append(gameObject.transform.DOLocalRotate(new Vector3(gameObject.transform.localEulerAngles.x, -newRotateValue, gameObject.transform.localEulerAngles.z), durationRotate).SetEase(Ease.InOutFlash));
        
        // move player
        Vector3 targetLocalPosition = new Vector3(moveTarget.transform.position.x, playerController.gameObject.transform.position.y, moveTarget.transform.position.z);
        seq.Insert(0.7f,
                playerController.gameObject.transform.DOMove(targetLocalPosition, 4.0f)
                    .SetEase(Ease.InOutSine))
            .Insert(1.0f, DOVirtual.DelayedCall(0, () =>
            {
                CorridorsManager.instance.endScreen.DOFade(1.0f, 0.7f).SetEase(Ease.InFlash)
                    .OnComplete(QuitGame);
            }));
        seq.Play();
    }
    
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stoppe le mode Play dans l’éditeur
#else
        Application.Quit(); // quitte l’exécutable
#endif
        Debug.Log("Quitter le jeu...");
    }
}
