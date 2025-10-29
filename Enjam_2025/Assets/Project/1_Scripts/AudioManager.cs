using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio References")] 
    [SerializeField] private AK.Wwise.Event ambiance;
    [SerializeField] private AK.Wwise.Event radioEvent;
    [SerializeField] private AK.Wwise.Event footStepEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event stopFootStepEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event doorOpenEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event doorKnockEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event lightEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event interactEvent;////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event interactPaintEvent;////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event flipRoomEvent;////////////////////////////TO TEST

    private void Awake() => instance = this;

    public void PlayAmbiance()
    {
        // je sais pas 
    }

    public void PlayRadio()
    {
        // je sais pas
    }
    
    public void PlaySoundFootStep() => footStepEvent.Post(gameObject);
    
    public void StopPlaySoundFootStep() => stopFootStepEvent.Post(gameObject);
    
    public void PlaySoundDoorKnock(GameObject door) => doorKnockEvent.Post(door);
    
    public void PlaySoundDoorOpen() => doorOpenEvent.Post(gameObject);
    
    public void PlaySoundLight(GameObject lightSource) => lightEvent.Post(lightSource);
    
    public void PlaySoundInteract() => interactEvent.Post(gameObject);
    
    public void PlaySoundInteractPaint() => interactPaintEvent.Post(gameObject);
    
    public void PlaySoundFlipRoom() => flipRoomEvent.Post(gameObject);
}
