using UnityEngine;

public enum E_RadioState
{
    First,
    Second,
    Third,
}

public class RadioManager : MonoBehaviour
{
    [Header("Radio References")]
    [SerializeField] private RadioComponent firstRadioComponent;
    [SerializeField] private RadioComponent secondeRadioComponent;

    [Space(10)]
    [Header("Doors References")]
    [SerializeField] private DoorComponent[] doorsComponent;
    
    
    private void Start()
    {
        InitRandomRadioSolution();
    }

    private void InitRandomRadioSolution()
    {
        firstRadioComponent.solutionRadio = (E_RadioState)Random.Range(0,3);
        secondeRadioComponent.solutionRadio = (E_RadioState)Random.Range(0,3);
        
        int doorValue = ((int)firstRadioComponent.solutionRadio+1) * 10 + (int)secondeRadioComponent.solutionRadio + 1;
        Debug.Log("doorValue is : " + doorValue);
        
        InitDoors(doorValue);
    }

    private void InitDoors(int doorValue)
    {
        foreach (DoorComponent doorComponent in doorsComponent)
            doorComponent.doorIDLinked = doorComponent.doorNumber == doorValue ? 1 : 0;
    }
}
