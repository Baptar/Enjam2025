using System;
using UnityEngine;

public class CorridorGenerated : MonoBehaviour
{
    [SerializeField] private DoorComponent firstDoorOfCoridor;
    [SerializeField] private bool isFirstCoridor = false;


    private void Start()
    {
        if (isFirstCoridor) CorridorsManager.instance.SetPreviousCorridor(gameObject);
    }
    
    public void OnCorridorGenerated(int doorNumber)
    {
        firstDoorOfCoridor.SetDoorNumberBehind(doorNumber);
            
        //set the door that is opening to not interactable
        firstDoorOfCoridor.SetIsInteractable(false);
        
        
        firstDoorOfCoridor.OpenDoor(this);
    }
}
