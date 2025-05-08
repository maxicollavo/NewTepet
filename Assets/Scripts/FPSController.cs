using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] CinemachineCamera playerCam;
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
    public float newSensitivity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        defaultYPos = playerCam.transform.localPosition.y;
    }

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        newSensitivity = sensSlider.value;

        originalCameraLocalPosition = cameraHolder.localPosition;
        crouchedCameraLocalPosition = originalCameraLocalPosition + new Vector3(0, -1.5f, 0);

        originalHeight = characterController.height;
        crouchedHeight = 2.5f;
        currentSpeed = moveSpeed;

        characterController.center = Vector3.zero;
        cameraHolder.localPosition = originalCameraLocalPosition;

        footstepClip = Resources.Load<AudioClip>("Sounds/" + footstepClipName);
        if (footstepClip == null)
            Debug.LogError("Footstep clip no encontrado en Resources/Sounds/" + footstepClipName);
    }

    private void Update()
    {
        HandleMovement();
        HandleFootsteps();
        RotationInputs();

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
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(currentMovement.x) > 0.1f || Mathf.Abs(currentMovement.z) > 0.1f)
        {
            timer += Time.deltaTime * (shouldCrouch ? crouchBobSpeed : walkBobSpeed);
            playerCam.transform.localPosition = new Vector3(playerCam.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * (shouldCrouch ? crouchBobAmount : walkBobAmount), playerCam.transform.localPosition.z);
        }
    }

    private void HandleFootsteps()
    {
        if (!isGrounded || footstepClip == null)
            return;

        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > footstepSpeedThreshold)
        {
            stepTimer += Time.deltaTime;

            if (shouldCrouch)
            {
                if (stepTimer >= crouchStepInterval)
                {
                    footstepAudioSource.volume = UnityEngine.Random.Range(0.3f, 0.5f);
                    footstepAudioSource.PlayOneShot(footstepClip);

                    stepTimer = 0f;
                }
            }
            else
            {
                if (stepTimer >= stepInterval)
                {
                    footstepAudioSource.volume = UnityEngine.Random.Range(0.8f, 1f);
                    footstepAudioSource.PlayOneShot(footstepClip);

                    stepTimer = 0f;
                }
            }
        }
        else
        {
            stepTimer = 0f;
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
}