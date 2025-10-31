using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float acceleration = 10f;
    public float gravity = -15f;

    [Space(10)]
    [Header("Camera")]
    public Camera playerCamera;
    public Transform cameraTransform;
    public float mouseSensitivity = 80f;
    public float cameraSmooth = 8f;
    public float maxLookAngle = 85f;
    public bool blockCamera = false;
    public bool isInspectingRadio = false;

    [Space(10)]
    [Header("Head Bobbing")]
    public float bobFrequency = 1.8f;
    public float bobAmplitude = 0.05f;
    public float bobRunMultiplier = 1.6f;
    public float bobSmooth = 8f;
    
    // private variables
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 moveInput;
    private Vector3 currentMove;
    private float xRotation;
    private float targetSpeed;
    private float bobTimer;
    private Vector3 defaultCamPos;
    private float yaw;
    private float pitch;
    private Quaternion camTargetRot;
    private Quaternion bodyTargetRot;
    private bool isMoving = false;
    public Vector3 gravityDir = Vector3.up;

    private void Awake() => instance = this;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        defaultCamPos = cameraTransform.localPosition;

        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.localEulerAngles.x;
    }

    private void Update()
    {
        if (!blockCamera && !isInspectingRadio) HandleLook();
        if (!isInspectingRadio) HandleMovement();
        if (!isInspectingRadio) HandleHeadBob();
        CheckFootSteps();
    }

    private void HandleLook()
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

    private void HandleMovement()
    {
        // --- Inputs ---
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        targetSpeed = isRunning ? runSpeed : walkSpeed;

        // --- Déplacement ---
        Vector3 desiredMove = (transform.forward * inputZ + transform.right * inputX).normalized * targetSpeed;
        currentMove = Vector3.Lerp(currentMove, desiredMove, Time.deltaTime * acceleration);

        controller.Move(currentMove * Time.deltaTime);

        // --- Gravité ---
        if (!controller.isGrounded)
            playerVelocity += -gravityDir * gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckFootSteps()
    {
        // --- Footstep (détection début / fin de mouvement) ---
        bool currentlyMoving = controller.isGrounded && currentMove.magnitude > 0.1f;

        // Début du mouvement
        if (currentlyMoving && !isMoving)
        {
            isMoving = true;
            AudioManager.instance.PlaySoundFootStep(); // joué une seule fois quand on commence à bouger
        }

        // Fin du mouvement
        if (!currentlyMoving)
        {
            isMoving = false;
            AudioManager.instance.StopPlaySoundFootStep(); // stop quand on s’arrête vraiment
        }
    }

    private void HandleHeadBob()
    {
        if (!controller.isGrounded)
        {
            // On coupe le bob si en l’air
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                defaultCamPos,
                Time.deltaTime * bobSmooth
            );
            return;
        }

        float speedPercent = currentMove.magnitude / runSpeed;
        float effectiveSpeed = Mathf.Clamp01(speedPercent);
        float frequency = Mathf.Lerp(0.8f, bobFrequency * (effectiveSpeed > 0.6f ? bobRunMultiplier : 1f), effectiveSpeed);
        float amplitude = Mathf.Lerp(0.02f, bobAmplitude, effectiveSpeed);
        
        bobTimer += Time.deltaTime * frequency * Mathf.Max(0.3f, effectiveSpeed + 0.2f);
        
        float verticalBob = Mathf.Sin(bobTimer * Mathf.PI * 2f) * amplitude;
        float horizontalBob = Mathf.Cos(bobTimer * Mathf.PI * 1f) * amplitude * 0.5f;
        
        Vector3 targetPos = defaultCamPos + new Vector3(horizontalBob, verticalBob, 0f);
        
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetPos,
            Time.deltaTime * bobSmooth
        );
    }
    
    public void SetYaw(float newYaw)
    {
        yaw = newYaw;
    }
}
