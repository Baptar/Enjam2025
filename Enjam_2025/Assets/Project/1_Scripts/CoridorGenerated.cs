using System;
using UnityEngine;

public class CorridorGenerated : MonoBehaviour
{
    [SerializeField] private DoorComponent firstDoorOfCoridor;
    [SerializeField] private bool isFirstCoridor = false;
    [HideInInspector] public GameObject previousCorridor;
    public GameObject lastSpawnPointNEXT;
    [SerializeField] private GameObject[] elementToAppearAfter;


    private void Start()
    {
        if (isFirstCoridor)
        {
            CorridorsManager.instance.SetPreviousCorridor(gameObject);
        }
        else
        {
            MakeAfterElementAppear(false);
        }
    }

    public void MakeAfterElementAppear(bool enable)
    {
        foreach (GameObject go in elementToAppearAfter)
        {
            go.SetActive(enable);
        }
    }
    
    public void OnCorridorGenerated(int doorNumber, bool displayOnNewDoorNumber, bool isPreviousDoorInfinity)
    {
        if (isPreviousDoorInfinity) firstDoorOfCoridor.FirstInfinityDoorOpened();
        else if (displayOnNewDoorNumber) firstDoorOfCoridor.SetDoorNumberBehind(doorNumber);
            
        //set the door that is opening to not interactable
        firstDoorOfCoridor.SetIsInteractable(false);
        
        
        firstDoorOfCoridor.OpenDoor(this);
    }

    // used for infinity logic, corridor number 4
    public void RemovePreviousCorridor()
    {
        if (previousCorridor != null)
        {
            Destroy(previousCorridor);
            previousCorridor = null;
        }
    }
}
