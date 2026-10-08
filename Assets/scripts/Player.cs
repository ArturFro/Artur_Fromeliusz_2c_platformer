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

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 9f;
    [SerializeField] private float wallJumpUpForce = 8f;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float wallCheckHeight = 0.75f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float groundCheckWidth = 0.8f;
    [SerializeField] private float wallJumpGraceTime = 0.15f;

    [Header("Wall Jump Feel")]
    [SerializeField] private float wallJumpControlTime = 0.2f;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideSpeed = 2.5f;

    [Header("Dash")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float dashPower = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool duringDash;
    [SerializeField] private float animTimer;

    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashCooldownTimer;

    // Movement
    private float dashDirection;

    // Jump
    private float coyoteTimer;
    private float jumpBufferTimer;

    // Wall
    private bool touchingWall;
    private int wallDirection;
    private float wallJumpTimer;

    // Wall jump
    private bool wallJumping;
    private float wallJumpControlTimer;
    private float wallJumpDirection;

    // Collider
    private Collider2D playerCollider;


    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        if (rig == null)
        {
            rig = GetComponent<Rigidbody2D>();
        }

        playerCollider = GetComponent<Collider2D>();

        if (playerCollider == null)
        {
            Debug.LogError("Player nie ma Collider2D!");
        }
    }


    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        CheckGround();
        CheckWalls();


        // ==========================================
        // DASH COOLDOWN
        // ==========================================

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;

            if (dashCooldownTimer < 0f)
            {
                dashCooldownTimer = 0f;
            }
        }


        // ==========================================
        // JUMP BUFFER
        // ==========================================

        if (Input.GetKeyDown(jumpKey))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }


        // ==========================================
        // COYOTE TIME
        // ==========================================

        if (onGround)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }


        // ==========================================
        // WALL GRACE
        // ==========================================

        if (touchingWall)
        {
            wallJumpTimer = wallJumpGraceTime;
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
        }


        // ==========================================
        // WALL JUMP
        // ==========================================

        if (jumpBufferTimer > 0f &&
            wallJumpTimer > 0f &&
            !onGround &&
            !duringDash)
        {
            WallJump();
        }


        // ==========================================
        // NORMAL JUMP
        // ==========================================

        else if (jumpBufferTimer > 0f &&
                 coyoteTimer > 0f &&
                 !duringDash)
        {
            Jump();
        }


        // ==========================================
        // WALL JUMP TIMER
        // ==========================================

        if (wallJumping)
        {
            wallJumpControlTimer -= Time.deltaTime;

            if (wallJumpControlTimer <= 0f)
            {
                wallJumping = false;
            }
        }


        // ==========================================
        // DASH
        // ==========================================

        if (Input.GetKeyDown(dashKey) &&
            horizontalInput != 0f &&
            !duringDash &&
            canDash &&
            dashCooldownTimer <= 0f)
        {
            Dash();
        }
    }


    // ==========================================
    // FIXED UPDATE
    // ==========================================

    private void FixedUpdate()
    {
        if (duringDash)
        {
            UpdateDash();
            return;
        }


        if (wallJumping)
        {
            UpdateWallJump();
            return;
        }


        // ==========================================
        // WALL SLIDE
        // ==========================================

        if (touchingWall &&
            !onGround &&
            rig.velocity.y < 0f)
        {
            rig.velocity = new Vector2(
                rig.velocity.x,
                Mathf.Max(
                    rig.velocity.y,
                    -wallSlideSpeed
                )
            );
        }


        // ==========================================
        // NORMAL MOVEMENT
        // ==========================================

        rig.velocity = new Vector2(
            horizontalInput * moveSpeed,
            rig.velocity.y
        );
    }


    // ==========================================
    // CHECK GROUND
    // ==========================================

    private void CheckGround()
    {
        if (playerCollider == null)
        {
            onGround = false;
            return;
        }


        Bounds bounds = playerCollider.bounds;

        Vector2 checkSize = new Vector2(
            bounds.size.x * groundCheckWidth,
            groundCheckDistance
        );

        Vector2 checkPosition = new Vector2(
            bounds.center.x,
            bounds.min.y - groundCheckDistance / 2f
        );


        Collider2D[] hits = Physics2D.OverlapBoxAll(
            checkPosition,
            checkSize,
            0f
        );


        onGround = false;


        foreach (Collider2D hit in hits)
        {
            if (hit == playerCollider)
            {
                continue;
            }


            if (hit.CompareTag("Ground"))
            {
                onGround = true;

                // Dotknięcie ziemi odnawia dash.
                canDash = true;

                return;
            }
        }
    }


    // ==========================================
    // CHECK WALLS
    // ==========================================

    private void CheckWalls()
    {
        touchingWall = false;
        wallDirection = 0;


        if (playerCollider == null)
        {
            return;
        }


        Bounds bounds = playerCollider.bounds;


        Vector2 checkSize = new Vector2(
            wallCheckDistance,
            bounds.size.y * wallCheckHeight
        );


        // ==========================================
        // LEWA
        // ==========================================

        Vector2 leftPosition = new Vector2(
            bounds.min.x - wallCheckDistance / 2f,
            bounds.center.y
        );


        Collider2D[] leftHits = Physics2D.OverlapBoxAll(
            leftPosition,
            checkSize,
            0f
        );


        foreach (Collider2D hit in leftHits)
        {
            if (hit == playerCollider)
            {
                continue;
            }


            if (hit.CompareTag("Ground"))
            {
                touchingWall = true;

                // Ściana po lewej.
                wallDirection = -1;

                return;
            }
        }


        // ==========================================
        // PRAWA
        // ==========================================

        Vector2 rightPosition = new Vector2(
            bounds.max.x + wallCheckDistance / 2f,
            bounds.center.y
        );


        Collider2D[] rightHits = Physics2D.OverlapBoxAll(
            rightPosition,
            checkSize,
            0f
        );


        foreach (Collider2D hit in rightHits)
        {
            if (hit == playerCollider)
            {
                continue;
            }


            if (hit.CompareTag("Ground"))
            {
                touchingWall = true;

                // Ściana po prawej.
                wallDirection = 1;

                return;
            }
        }
    }


    // ==========================================
    // NORMAL JUMP
    // ==========================================

    private void Jump()
    {
        rig.velocity = new Vector2(
            rig.velocity.x,
            jumpForce
        );

        onGround = false;

        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }


    // ==========================================
    // WALL JUMP
    // ==========================================

    private void WallJump()
    {
        // Ściana po lewej  = -1
        // Odbicie w prawo  = +1
        //
        // Ściana po prawej = +1
        // Odbicie w lewo   = -1

        wallJumpDirection = -wallDirection;


        // ==========================================
        // ODBICIE
        // ==========================================

        rig.velocity = Vector2.zero;

        rig.velocity = new Vector2(
            wallJumpDirection * wallJumpForce,
            wallJumpUpForce
        );


        onGround = false;
        touchingWall = false;


        // ==========================================
        // WALL JUMP CONTROL
        // ==========================================

        wallJumping = true;

        wallJumpControlTimer = wallJumpControlTime;


        // ==========================================
        // RESET JUMP
        // ==========================================

        jumpBufferTimer = 0f;
        wallJumpTimer = 0f;
        coyoteTimer = 0f;


        // ==========================================
        // ODŚWIEŻ DASH
        // ==========================================

        // Dzięki temu:
        //
        // skok
        // ↓
        // wall jump
        // ↓
        // dash
        //
        // jest możliwy.

        canDash = true;
    }


    // ==========================================
    // WALL JUMP UPDATE
    // ==========================================

    private void UpdateWallJump()
    {
        rig.velocity = new Vector2(
            wallJumpDirection * wallJumpForce,
            rig.velocity.y
        );
    }


    // ==========================================
    // DASH
    // ==========================================

    private void Dash()
    {
        // Zużywamy aktualny dash.

        canDash = false;


        // Uruchamiamy cooldown.

        dashCooldownTimer = dashCooldown;


        // Uruchamiamy dash.

        duringDash = true;

        animTimer = 0f;

        dashDirection = Mathf.Sign(
            horizontalInput
        );
    }


    // ==========================================
    // UPDATE DASH
    // ==========================================

    private void UpdateDash()
    {
        animTimer += Time.fixedDeltaTime;


        float normalizedTime =
            animTimer / dashDuration;


        normalizedTime =
            Mathf.Clamp01(normalizedTime);


        float curveValue =
            curve.Evaluate(normalizedTime);


        float dashVelocity =
            dashDirection *
            dashPower *
            curveValue;


        rig.velocity = new Vector2(
            dashVelocity,
            0f
        );


        if (animTimer >= dashDuration)
        {
            duringDash = false;
            animTimer = 0f;
        }
    }


    // ==========================================
    // DEBUG
    // ==========================================

    private void OnDrawGizmosSelected()
    {
        if (playerCollider == null)
        {
            return;
        }


        Bounds bounds = playerCollider.bounds;


        // ==========================================
        // GROUND
        // ==========================================

        Vector2 groundSize = new Vector2(
            bounds.size.x * groundCheckWidth,
            groundCheckDistance
        );


        Vector2 groundPosition = new Vector2(
            bounds.center.x,
            bounds.min.y -
            groundCheckDistance / 2f
        );


        // ==========================================
        // WALLS
        // ==========================================

        Vector2 wallSize = new Vector2(
            wallCheckDistance,
            bounds.size.y * wallCheckHeight
        );


        Vector2 leftPosition = new Vector2(
            bounds.min.x -
            wallCheckDistance / 2f,
            bounds.center.y
        );


        Vector2 rightPosition = new Vector2(
            bounds.max.x +
            wallCheckDistance / 2f,
            bounds.center.y
        );


        // ==========================================
        // DRAW
        // ==========================================

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            groundPosition,
            groundSize
        );


        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            leftPosition,
            wallSize
        );


        Gizmos.DrawWireCube(
            rightPosition,
            wallSize
        );
    }
}