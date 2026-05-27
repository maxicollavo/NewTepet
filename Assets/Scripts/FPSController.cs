using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Camera Settings")]
    [SerializeField] private bool invertYAxis = false;
    [SerializeField] private Transform cameraHolder;
    private bool freeze;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    [SerializeField] private float clampRange = 80f;

    [Header("Footstep Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private string footstepClipName = "Footstep";
    private AudioClip footstepClip;
    private float stepTimer;

    [Header("HeadBob Parameters")]
    private bool canUseHeadBob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Other Settings")]
    Vector2 mouseInput;
    private bool stepPlayedThisCycle = false;
    [SerializeField] float onReadingTime = 1.5f;

    [SerializeField] private CinemachineCamera playerCam;
    private CharacterController characterController;
    private PlayerInputHandler inputHandler;
    private Vector3 currentMovement = Vector3.zero;
    private float verticalRotation;

    private Vector3 originalCameraLocalPosition;

    private float originalHeight;
    private float currentSpeed;
    [SerializeField] private LayerMask ceilingLayerMask;

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
    [SerializeField] private Animator miAnimator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCam.transform.localPosition.y;

        NewEventManager.OnFreezePlayer += PlayerOnFreeze;
        NewEventManager.OnUnfreezePlayer += PlayerOnUnfreeze;
    }

    private void OnDestroy()
    {
        NewEventManager.OnFreezePlayer -= PlayerOnFreeze;
        NewEventManager.OnUnfreezePlayer -= PlayerOnUnfreeze;
    }

    private void PlayerOnFreeze()
    {
        freeze = true;
    }
    private void PlayerOnUnfreeze()
    {
        freeze = false;
    }
    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        newSensitivity = sensSlider.value;
        VolumeValue = VolumeSlider.value;

        originalCameraLocalPosition = cameraHolder.localPosition;

        originalHeight = characterController.height;
        currentSpeed = moveSpeed;

        characterController.center = Vector3.zero;
        cameraHolder.localPosition = originalCameraLocalPosition;

        footstepClip = Resources.Load<AudioClip>("Sounds/" + footstepClipName);

        this.enabled = false;
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
        if (freeze) return;

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

        Quaternion targetRotation = cameraHolder.rotation;
        float bodyYaw = transform.eulerAngles.y;
        float targetYaw = targetRotation.eulerAngles.y;
        float yawDifference = Mathf.DeltaAngle(bodyYaw, targetYaw);
        yawDifference = Mathf.Clamp(yawDifference, -handMaxYawAngle, handMaxYawAngle);
        float targetPitch = cameraHolder.localEulerAngles.x;
        if (targetPitch > 180f) targetPitch -= 360f;
        Quaternion limitedTargetRotation = Quaternion.Euler(targetPitch, bodyYaw + yawDifference, 0f);
        handTransform.rotation = Quaternion.Slerp(handTransform.rotation, limitedTargetRotation, Time.deltaTime * handRotationLagSpeed);
    }


    private void HandleHandBob()
    {
        if (handTransform == null || cameraHolder == null) return;

        if (characterController.isGrounded && (Mathf.Abs(currentMovement.x) > 0.1f || Mathf.Abs(currentMovement.z) > 0.1f))
        {
            float bobTimer = Time.time * handBobSpeed;
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
        if (freeze)
        {
            currentMovement = Vector3.zero;
            verticalVelocity = -2f;
            characterController.Move(Vector3.zero);
            return;
        }

        bool wantsToStand = !Keyboard.current.cKey.isPressed;
        bool ceilingAbove = IsCeilingAbove();

        float targetHeight = originalHeight;
        float currentHeight = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * 10f);
        float heightDifference = currentHeight - characterController.height;
        characterController.height = currentHeight;
        characterController.center += new Vector3(0, heightDifference / 2f, 0);

        Vector3 targetCamPos = originalCameraLocalPosition;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetCamPos, Time.deltaTime * 10f);

        currentSpeed = moveSpeed;

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
            timer += Time.deltaTime * walkBobSpeed;
            float sinValue = Mathf.Sin(timer);
            float bobAmount = walkBobAmount;

            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos + sinValue * bobAmount,
                playerCam.transform.localPosition.z
            );

            if (sinValue <= -0.9f && !stepPlayedThisCycle)
            {
                footstepAudioSource.volume = UnityEngine.Random.Range(0.8f, 1f);
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
        float checkHeight = originalHeight;
        float radius = characterController.radius - 0.05f;

        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = transform.position + Vector3.up * (checkHeight - radius);

        return Physics.CheckCapsule(bottom, top, radius, ceilingLayerMask, QueryTriggerInteraction.Ignore);
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    public void ChangeVolume(float VolumeValue)
    {
        AudioListener.volume = VolumeValue;
    }

    public void EnableInputs()
    {
        this.enabled = true; 
    }
}