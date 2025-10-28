using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 80f;
    public float cameraSmooth = 8f;
    public float maxLookAngle = 85f;

    [Space(10)]
    [Header("Head Bobbing")]
    public float bobFrequency = 1.8f;
    public float bobAmplitude = 0.05f;
    public float bobRunMultiplier = 1.6f;
    public float bobSmooth = 8f;
    
    private CharacterController controller;
    private Vector3 defaultCamPos;
    private float yaw;
    private float pitch;
    private Quaternion camTargetRot;
    private Quaternion bodyTargetRot;
    private float xRotation;
    private float bobTimer;
    
    
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        defaultCamPos = cameraTransform.localPosition;

        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.localEulerAngles.x;
    }

    void Update()
    {
        HandleLook();
    }
    
    void HandleLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * 0.01f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 0.01f;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
        
        camTargetRot = Quaternion.Euler(pitch, 0f, 0f);
        bodyTargetRot = Quaternion.Euler(0f, yaw, 0f);
        
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            camTargetRot,
            Time.deltaTime * cameraSmooth
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            bodyTargetRot,
            Time.deltaTime * cameraSmooth
        );
    }
}
