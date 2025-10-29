using UnityEngine;

public class HelperInfinity : MonoBehaviour
{
    [SerializeField] private TriggerInfinityCorridor triggerInfinityCorridor;

    public void RemoveElement()
    {
        triggerInfinityCorridor.EnableElements(false);
    }
}
