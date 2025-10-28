using UnityEngine;

public interface IInteractable
{
    bool GetIsInteractable();
    void SetIsInteractable(bool value);
    string GetInteractPrompt();      // Texte à afficher (ex : "Open door")
    void Interact(PlayerInteractor interactor); // Action à exécuter
    
    void OnHovered(PlayerInteractor interactor);
    
    void OnReleased();
}