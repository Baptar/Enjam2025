using DG.Tweening;
using UnityEngine;

public class MorseManager : MonoBehaviour
{
    [System.Serializable]
    public struct LightStruct
    {
        public Light light1;
        public Light light2;
        public MeshRenderer meshRenderer;
        public Material materialOn;
        public Material materialOff;
    }
    
    
    [Header("Door")]
    [SerializeField] private DoorComponent[] doorsComponent;
    
    [Space(10)]
    [Header("Morse Settings")]
    [Header("Duration")]
    [SerializeField] private float blinkOnTime = 0.1f;
    [SerializeField] private float blinkOffTime = 0.1f;
    [SerializeField] private float shortPause = 0.3f;
    [SerializeField] private float longPause = 1.5f;
    [SerializeField] private float pauseBetweenSequences = 6.0f;
    
    [Space(3)]
    [Header("Light Material reference")]
    [SerializeField] private LightStruct[] lights;
    
    
    private Sequence blinkSequence;
    private int firstValue = 0;
    private int secondValue = 0;
    private DoorComponent selectedDoor;
    private LightStruct selectedLightStruct;

    private int[] doorValues = { 11, 12, 13, 21, 22, 23, 31, 32, 33 };

    private void Start()
    {
        reshuffle(doorValues);
        
        if (doorValues.Length == doorsComponent.Length)
            for (int i = 0; i < doorsComponent.Length; i++)
            {
                doorsComponent[i].doorNumber = doorValues[i];
                doorsComponent[i].InitTextDoorWithNumber();
            }
        
        InitRandomSolution();
        InitRandomLight();
        SelectRandomDoor();

        StartBlinkLoop();
    }
    
    private void reshuffle(int[] valuesTab)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < valuesTab.Length; t++ )
        {
            int tmp = valuesTab[t];
            int r = Random.Range(t, valuesTab.Length);
            valuesTab[t] = valuesTab[r];
            valuesTab[r] = tmp;
        }
    }

    private void InitRandomSolution()
    {
        firstValue =  Random.Range(1,4);
        secondValue =  Random.Range(1,4);
        
        Debug.Log(firstValue * 10 + secondValue);
        
        foreach (DoorComponent door in doorsComponent)
            door.doorIDLinked = door.doorNumber == (firstValue * 10 + secondValue) ? 3 : 0;
    }
    
    private void InitRandomLight()
    {
        int value = Random.Range(0, lights.Length);
        selectedLightStruct =  lights[value];
        
        selectedLightStruct.light1.intensity = 0.0f;
        selectedLightStruct.light2.intensity = 0.0f;
        selectedLightStruct.meshRenderer.material = selectedLightStruct.materialOff;
    }
    
    private void SelectRandomDoor()
    {
        int value = Random.Range(0, doorsComponent.Length);
        selectedDoor = doorsComponent[value];
    }
    
    void StartBlinkLoop()
    {
        blinkSequence?.Kill();
        blinkSequence = DOTween.Sequence();

        blinkSequence.AppendInterval(pauseBetweenSequences);
        
        // --- Première série de blink ---
        for (int i = 0; i < firstValue; i++)
        {
            AddBlinkToSequence();
        }

        // --- Pause longue entre les deux séries ---
        blinkSequence.AppendInterval(longPause);

        // --- Deuxième série de blink ---
        for (int i = 0; i < secondValue; i++)
        {
            AddBlinkToSequence();
        }

        // --- Boucle infinie ---
        blinkSequence.SetLoops(-1, LoopType.Restart)
            .OnStepComplete(() =>
            {
                //InitRandomLight();
                SelectRandomDoor();
            });
    }

    void OnDisable()
    {
        blinkSequence?.Kill();
    }

    void AddBlinkToSequence()
    {
        // Capture locale des références au moment de la création du step
        Light light1 = selectedLightStruct.light1;
        Light light2 = selectedLightStruct.light2;
        MeshRenderer mesh = selectedLightStruct.meshRenderer;
        Material matOn = selectedLightStruct.materialOn;
        Material matOff = selectedLightStruct.materialOff;
        DoorComponent door = selectedDoor;

        // Lumière se rallume + son
        blinkSequence.AppendCallback(() =>
        {
            // Change le matériau (sur la copie mesh)
            if (mesh != null) mesh.material = matOn;

            // Joue le son en utilisant la copie du door et de la light
            if (door != null) AudioManager.instance.PlaySoundDoorKnock(selectedDoor.gameObject);
                //PlaySoundOnEachDoors();

            if (light1 != null)
                AudioManager.instance.PlaySoundLight(light1.gameObject);
        });

        // Tween des lumières (sur les copies)
        if (light1 != null && light2 != null)
        {
            blinkSequence.Append(light1.DOIntensity(1.0f, blinkOnTime))
                .Join(light2.DOIntensity(1.0f, blinkOnTime));
        }
        else if (light1 != null) // fallback si une seule lumière
        {
            blinkSequence.Append(light1.DOIntensity(1.0f, blinkOnTime));
        }

        // Pause éteinte
        blinkSequence.AppendInterval(shortPause);

        // Lumière s'éteint (mat off)
        blinkSequence.AppendCallback(() =>
        {
            if (mesh != null) mesh.material = matOff;
        });

        // Tween éteindre
        if (light1 != null && light2 != null)
        {
            blinkSequence.Append(light1.DOIntensity(0.0f, blinkOnTime))
                .Join(light2.DOIntensity(0.0f, blinkOnTime));
        }
        else if (light1 != null)
        {
            blinkSequence.Append(light1.DOIntensity(0.0f, blinkOnTime));
        }
    }

    private void PlaySoundOnEachDoors()
    {
        foreach (DoorComponent doorComponent in doorsComponent)
        {
            AudioManager.instance.PlaySoundDoorKnock(doorComponent.gameObject);
        }
    }
}
