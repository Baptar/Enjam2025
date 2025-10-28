using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask interactMask;
    public KeyCode interactKey = KeyCode.E;

    [Space(10)]
    [Header("UI")]
    public TMP_Text interactText;
    public Camera playerCamera;

    private IInteractable currentTarget;

    void Update()
    {
        HandleInteractionRay();
        HandleInput();
    }

    void HandleInteractionRay()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentTarget = interactable;
                currentTarget.OnHovered(this);
                //ShowPrompt(interactable.GetInteractPrompt());
                return;
            }
        }

        if (currentTarget != null) currentTarget.OnReleased();
        currentTarget = null;
        HidePrompt();
    }

    void HandleInput()
    {
        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(this);
        }
    }

    void ShowPrompt(string text)
    {
        if (interactText == null) return;
        interactText.text = $"[E] {text}";
        interactText.enabled = true;
    }

    void HidePrompt()
    {
        if (interactText == null) return;
        interactText.enabled = false;
    }
}