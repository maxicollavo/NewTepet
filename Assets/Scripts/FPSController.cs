using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float crouchSpeed = 1.5f;
    private bool shouldCrouch;

    [Header("Camera Settings")]
    [SerializeField] private bool invertYAxis = false;
    [SerializeField] private Transform cameraHolder;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    [SerializeField] private float clampRange = 80f;

    [Header("Footstep Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private string footstepClipName = "Footstep";
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float crouchStepInterval = 0.8f;
    [SerializeField] private float footstepSpeedThreshold = 0.1f;
    private AudioClip footstepClip;
    private float stepTimer;

    [Header("HeadBob Parameters")]
    private bool canUseHeadBob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Other Settings")]
    Vector2 mouseInput;
    private bool stepPlayedThisCycle = false;

    [SerializeField] private CinemachineCamera playerCam;
    private CharacterController characterController;
    private PlayerInputHandler inputHandler;
    private Vector3 currentMovement = Vector3.zero;
    private float verticalRotation;

    private Vector3 originalCameraLocalPosition;
    private Vector3 crouchedCameraLocalPosition;

    private float originalHeight;
    private float crouchedHeight;
    private float currentSpeed;

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float terminalVelocity = -50f;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;
    public Slider sensSlider;
    public Slider VolumeSlider;
    public float newSensitivity;
    public float VolumeValue = 50;

    [Header("Hand Settings")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private float handRotationLagSpeed = 5f;
    [SerializeField] private float handMaxYawAngle = 50f;
    [SerializeField] private float handBobAmount = 0.03f;
    [SerializeField] private float handBobSpeed = 14f;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCam.transform.localPosition.y;
    }

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        newSensitivity = sensSlider.value;
        VolumeValue = VolumeSlider.value;

        originalCameraLocalPosition = cameraHolder.localPosition;
        crouchedCameraLocalPosition = originalCameraLocalPosition + new Vector3(0, -1.5f, 0);

        originalHeight = characterController.height;
        crouchedHeight = 2.5f;
        currentSpeed = moveSpeed;

        characterController.center = Vector3.zero;
        cameraHolder.localPosition = originalCameraLocalPosition;

        footstepClip = Resources.Load<AudioClip>("Sounds/" + footstepClipName);
    }

    private void Update()
    {
        HandleMovement();
        RotationInputs();
        HandleHandLag();
        HandleHandBob();

        if (canUseHeadBob) HandleHeadBob();
    }


    private void LateUpdate()
    {
        HandleRotation();
    }

    void RotationInputs()
    {
        mouseInput.y = invertYAxis ? -inputHandler.LookInput.y : inputHandler.LookInput.y;
        mouseInput.x = inputHandler.LookInput.x * mouseSensitivity;
    }

    private void HandleRotation()
    {
        transform.Rotate(0, mouseInput.x, 0);

        verticalRotation -= mouseInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampRange, clampRange);

        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleHandLag()
    {
        if (handTransform == null || cameraHolder == null) return;

        float bodyYaw = transform.eulerAngles.y;
        float targetYaw = cameraHolder.rotation.eulerAngles.y;

        float yawDifference = Mathf.DeltaAngle(bodyYaw, targetYaw);
        yawDifference = Mathf.Clamp(yawDifference, -handMaxYawAngle, handMaxYawAngle);

        Quaternion limitedRotation = Quaternion.Euler(0f, bodyYaw + yawDifference, 0f);
        handTransform.rotation = Quaternion.Slerp(handTransform.rotation, limitedRotation, Time.deltaTime * handRotationLagSpeed);
    }

    private void HandleHandBob()
    {
        if (handTransform == null || cameraHolder == null) return;

        if (characterController.isGrounded && (Mathf.Abs(currentMovement.x) > 0.1f || Mathf.Abs(currentMovement.z) > 0.1f))
        {
            float bobTimer = Time.time * (shouldCrouch ? crouchBobSpeed : handBobSpeed);
            float bobOffsetY = Mathf.Sin(bobTimer) * handBobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * handBobAmount * 0.5f;

            handTransform.localPosition = new Vector3(
                Mathf.Lerp(handTransform.localPosition.x, bobOffsetX, Time.deltaTime * 5f),
                Mathf.Lerp(handTransform.localPosition.y, bobOffsetY, Time.deltaTime * 5f),
                handTransform.localPosition.z
            );
        }
        else
        {
            handTransform.localPosition = Vector3.Lerp(handTransform.localPosition,
                Vector3.zero, Time.deltaTime * 5f);
        }
    }



    private void HandleMovement()
    {
        bool wantsToStand = !Keyboard.current.cKey.isPressed;
        bool ceilingAbove = IsCeilingAbove();
        shouldCrouch = Keyboard.current.cKey.isPressed || ceilingAbove;

        float targetHeight = shouldCrouch ? crouchedHeight : originalHeight;
        float currentHeight = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 10f);
        float heightDifference = currentHeight - characterController.height;
        characterController.height = currentHeight;
        characterController.center += new Vector3(0, heightDifference / 2f, 0);

        Vector3 targetCamPos = shouldCrouch ? crouchedCameraLocalPosition : originalCameraLocalPosition;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCamPos, Time.deltaTime * 10f);

        currentSpeed = shouldCrouch ? crouchSpeed : moveSpeed;

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection).normalized;

        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;

        isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);
        }

        currentMovement.y = verticalVelocity;
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleHeadBob()
    {
        if (!characterController.isGrounded)
            return;

        if (Mathf.Abs(currentMovement.x) > 0.1f || Mathf.Abs(currentMovement.z) > 0.1f)
        {
            timer += Time.deltaTime * (shouldCrouch ? crouchBobSpeed : walkBobSpeed);
            float sinValue = Mathf.Sin(timer);
            float bobAmount = shouldCrouch ? crouchBobAmount : walkBobAmount;

            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos + sinValue * bobAmount,
                playerCam.transform.localPosition.z
            );

            if (sinValue <= -0.9f && !stepPlayedThisCycle)
            {
                footstepAudioSource.volume = shouldCrouch ? UnityEngine.Random.Range(0.3f, 0.5f) : UnityEngine.Random.Range(0.8f, 1f);
                footstepAudioSource.PlayOneShot(footstepClip);
                stepPlayedThisCycle = true;
            }
            else if (sinValue > -0.9f)
            {
                stepPlayedThisCycle = false;
            }
        }
        else
        {
            timer = 0f;
            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos,
                playerCam.transform.localPosition.z
            );
        }
    }

    private bool IsCeilingAbove()
    {
        Vector3 origin = transform.position + Vector3.up * (characterController.height / 2f);
        float checkDistance = 0.6f;
        bool hasHit = Physics.Raycast(origin, Vector3.up, out RaycastHit hit, checkDistance, ~0, QueryTriggerInteraction.Ignore);
        return hasHit;
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    public void ChangeVolume(float VolumeValue)
    {
        AudioListener.volume = VolumeValue;
    }
}