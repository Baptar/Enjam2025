using UnityEngine;

public interface IInteractable
{
    bool GetIsInteractable();
    void SetIsInteractable(bool value);
    string GetInteractPrompt();  
    void Interact(PlayerInteractor interactor);
    
    void OnHovered(PlayerInteractor interactor);
    
    void OnReleased();
}