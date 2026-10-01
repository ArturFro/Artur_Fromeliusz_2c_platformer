using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rig;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float horizontalInput;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private bool onGround;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Dash")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float dashPower = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool duringDash;
    [SerializeField] private float animTimer;
  

    private Vector2 tempData;
    private float dashDirection;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(jumpKey) && onGround && !duringDash)
        {
            tempData.Set(rig.velocity.x, jumpForce);
            rig.velocity = tempData;
            onGround = false;
        }

        
        if (Input.GetKeyDown(dashKey) && horizontalInput != 0f && !duringDash)
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        if (duringDash)
        {
            UpdateDash();
        }
        else
        {
            tempData.Set(horizontalInput * moveSpeed, rig.velocity.y);
            rig.velocity = tempData;
        }
    }

    private void Dash()
    {
        duringDash = true;
        animTimer = 0f;

        // Zapisujemy wy³¹cznie kierunek:
        // 1 oznacza prawo, a -1 oznacza lewo.
        dashDirection = Mathf.Sign(horizontalInput);
    }

    private void UpdateDash()
    {
        animTimer += Time.fixedDeltaTime;

       
        float normalizedTime = animTimer / dashDuration;

        float curveValue = curve.Evaluate(normalizedTime);
        float dashVelocity = dashDirection * dashPower * curveValue;

        rig.velocity = new Vector2(dashVelocity, 0f);

        if (animTimer >= dashDuration)
        {
            duringDash = false;
            animTimer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
}