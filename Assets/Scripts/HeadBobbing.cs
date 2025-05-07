using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    private CharacterController controller;
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;

    private float defaultYPos;
    private float timer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * bobbingSpeed;
            float newY = defaultYPos + Mathf.Sin(timer) * bobbingAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * 5f), transform.localPosition.z);
        }
    }
}
