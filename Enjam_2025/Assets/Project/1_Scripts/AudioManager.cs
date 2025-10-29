using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio References")] 
    [SerializeField] private AK.Wwise.Event ambiance;
    [SerializeField] private AK.Wwise.Event radioEvent;
    [SerializeField] private AK.Wwise.Event footStepEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event doorOpenEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event doorKnockEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event lightEvent;/////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event interactEvent;////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event interactPaintEvent;////////////////////////////TO TEST
    [SerializeField] private AK.Wwise.Event flipRoomEvent;////////////////////////////TO TEST


    private uint playing_FS_ID;
    private uint playing_radio_ID;
    
    private void Awake() => instance = this;

    private void Start() {
        PlayAmbiance();
    }

    private void PlayAmbiance() => ambiance.Post(gameObject);

    public void PlayRadio()
    {
        AkSoundEngine.StopPlayingID(playing_radio_ID);
        playing_radio_ID = radioEvent.Post(gameObject);
    }

    public void StopRadio() => AkSoundEngine.StopPlayingID(playing_radio_ID);
  

    public void PlaySoundFootStep() => playing_FS_ID = footStepEvent.Post(gameObject);

    public void StopPlaySoundFootStep() => AkSoundEngine.StopPlayingID(playing_FS_ID);
    
    public void PlaySoundDoorKnock(GameObject door) => doorKnockEvent.Post(door);
    
    public void PlaySoundDoorOpen() => doorOpenEvent.Post(gameObject);
    
    public void PlaySoundLight(GameObject lightSource) => lightEvent.Post(lightSource);
    
    public void PlaySoundInteract() => interactEvent.Post(gameObject);
    
    public void PlaySoundInteractPaint() => interactPaintEvent.Post(gameObject);
    
    public void PlaySoundFlipRoom() => flipRoomEvent.Post(gameObject);
}
