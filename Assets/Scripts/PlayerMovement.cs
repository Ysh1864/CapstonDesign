using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerMovement : MonoBehaviour
{
    [Header("스탯 데이터")]
    [SerializeField] private PlayerStatData stat;

    [Header("지면 감지")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private PlayerInventory inventory;
    private bool isGrounded;
    public int FacingDirection { get; private set; } = 1;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpHoldTimer;
    private bool isJumping;

    private IPickupable nearbyPickupable;   // F 키로 집을 대상
    private IInteractable nearbyInteractable; // ↓ 키로 상호작용할 대상

    public event System.Action OnToolUsed;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        UpdateGroundCheck();
        UpdateTimers();
        HandleJumpInput();
        HandleInteractInput();   // ↓ 상호작용
        HandlePickupInput();     // F  줍기
        HandleDropInput();       // G  내려놓기
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        ApplyGravity();
    }


    private void UpdateGroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            stat.groundCheckRadius,
            stat.groundLayer);

        if (!wasGrounded && isGrounded)
        {
            coyoteTimer = stat.coyoteTime;
            isJumping = false;
        }

        if (isGrounded)
            coyoteTimer = stat.coyoteTime;
    }


    private void UpdateTimers()
    {
        if (!isGrounded) coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
        if (isJumping) jumpHoldTimer -= Time.deltaTime;
    }

    private void HandleHorizontalMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float targetVelX = input * stat.moveSpeed;

        float rate = (Mathf.Abs(input) > 0.01f)
            ? stat.acceleration
            : stat.deceleration;

        float newVelX = Mathf.MoveTowards(rb.velocity.x, targetVelX, rate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(newVelX, rb.velocity.y);
    }

    private void UpdateFacing()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input > 0.01f) { FacingDirection = 1; transform.localScale = new Vector3(1f, 1f, 1f); }
        else if (input < -0.01f) { FacingDirection = -1; transform.localScale = new Vector3(-1f, 1f, 1f); }

       /* if(input != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }*/
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            jumpBufferTimer = stat.jumpBufferTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            ExecuteJump();

        if (Input.GetKeyUp(KeyCode.UpArrow) && isJumping)
        {
            isJumping = false;
            jumpHoldTimer = 0f;
            if (rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void ExecuteJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, stat.jumpForce);
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
        jumpHoldTimer = stat.jumpHoldDuration;
        isJumping = true;
    }

    private void ApplyGravity()
    {
        float scale;
        if (isJumping && Input.GetKey(KeyCode.UpArrow) && jumpHoldTimer > 0f)
            scale = stat.gravityScale * stat.jumpHoldGravityScale;
        else if (rb.velocity.y < 0f)
            scale = stat.fallGravityScale;
        else
            scale = stat.gravityScale;

        rb.gravityScale = scale;
    }


    private void HandleInteractInput()
    {
        bool pressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.F);
        if (!pressed) return;

        // 트리거 범위 안에 IInteractable 이 있으면 상호작용
        if (nearbyInteractable != null)
        {
            nearbyInteractable.Interact(this);
            OnToolUsed?.Invoke();
            return;
        }

        // 없으면 전방 레이캐스트로 재시도
        TryInteractRaycast();
    }

    private void TryInteractRaycast()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        Vector2 direction = new Vector2(FacingDirection, 0f);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.6f, ~stat.groundLayer);

        if (hit.collider != null &&
            hit.collider.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(this);
            OnToolUsed?.Invoke();
        }
    }

    private void HandlePickupInput()
    {
        bool pressed = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.DownArrow);
        if (!pressed) return;
        if (nearbyPickupable == null) return;

        nearbyPickupable.OnSwitch(this);
    }

    private void HandleDropInput()
    {
        if (!Input.GetKeyDown(KeyCode.G)) return;
        if (inventory == null) return;

        inventory.DropCurrent();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPickupable pickupable))
            nearbyPickupable = pickupable;

        if (other.TryGetComponent(out IInteractable interactable))
            nearbyInteractable = interactable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPickupable pickupable) &&
            pickupable == nearbyPickupable)
            nearbyPickupable = null;

        if (other.TryGetComponent(out IInteractable interactable) &&
            interactable == nearbyInteractable)
            nearbyInteractable = null;
    }


    public float HorizontalSpeed => Mathf.Abs(rb.velocity.x);
    public bool IsGrounded => isGrounded;
    public float VerticalVelocity => rb.velocity.y;
    public bool HasNearbyPickupable => nearbyPickupable != null;
    public bool HasNearbyInteractable => nearbyInteractable != null;


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null || stat == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, stat.groundCheckRadius);
    }
#endif
}