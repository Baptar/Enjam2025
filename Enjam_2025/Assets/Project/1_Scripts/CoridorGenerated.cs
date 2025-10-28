using System;
using UnityEngine;

public class CoridorGenerated : MonoBehaviour
{
    [SerializeField] private DoorComponent firstDoorOfCoridor;
    [SerializeField] private bool isFirstCoridor = false;


    private void Start()
    {
        if (isFirstCoridor) DoorsManager.instance.SetPreviousCoridor(gameObject);
    }
    
    public void OnCoridorGenerated()
    {
        //set the door that is opening to not interactable
        firstDoorOfCoridor.SetIsInteractable(false);
        
        
        firstDoorOfCoridor.OpenDoor(this);
    }
}
