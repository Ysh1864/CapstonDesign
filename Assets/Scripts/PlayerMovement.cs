using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerMovement : MonoBehaviour
{
    [Header("스탯 데이터")]
    [SerializeField] private PlayerStatData stat;

    [Header("지면 감지")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Animator aniRun;

    public Rigidbody2D rb;
    private PlayerInventory inventory;
    private bool isGrounded;
    public int FacingDirection { get; private set; } = 1;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpHoldTimer;
    private bool isJumping;
    private bool isDead = false;
    private IPickupable nearbyPickupable;
    private IInteractable nearbyInteractable;

    private Portal nearbyPortal;
    private DeadUI deadUI;

    public PlayerSpawner ps;
    public EndPotalTrigger ept;
    public bool stopControll = false;   //조작불가 상태

    public event System.Action OnToolUsed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inventory = GetComponent<PlayerInventory>();
        deadUI = FindObjectOfType<DeadUI>();
    }

    private void OnEnable()
    {
        BatteryController.OnBatteryEmpty += Dead;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryEmpty -= Dead;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isDead) return; // 죽었다면 입력 및 업데이트 중지

        UpdateGroundCheck();
        UpdateTimers();
        HandleJumpInput();
        HandleInteractInput();
        HandlePickupInput();
        UpdateFacing();

        if (nearbyPortal != null)
            nearbyPortal.SetPlayerGrounded(isGrounded);

    }

    private void FixedUpdate()
    {
        if (isDead) return; // 죽었다면 물리 이동 중지

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

        aniRun.SetBool("isJumping", !isGrounded); // 점프 애니메이션 추가
    }

    private void UpdateTimers()
    {
        if (!isGrounded) coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
        if (isJumping) jumpHoldTimer -= Time.deltaTime;
    }

    private void HandleHorizontalMovement()
    {
        if(stopControll)
        return;

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
        if(stopControll)
        return;

        float input = Input.GetAxisRaw("Horizontal");
        if (input > 0.01f) { FacingDirection = 1; transform.localScale = new Vector3(1f, 1f, 1f); }
        else if (input < -0.01f) { FacingDirection = -1; transform.localScale = new Vector3(-1f, 1f, 1f); }

        if (input != 0)
        {
            aniRun.SetBool("isRunning", true);
        }
        else
        {
            aniRun.SetBool("isRunning", false);
        }
    }

    private void HandleJumpInput()
    {
        if(stopControll)
        return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nearbyPortal != null && nearbyPortal.IsUnlocked && isGrounded)
                return;
            jumpBufferTimer = stat.jumpBufferTime;
        }

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

        if (nearbyInteractable != null)
        {
            nearbyInteractable.Interact(this);
            OnToolUsed?.Invoke();
            return;
        }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        rb = GetComponent<Rigidbody2D>();
        deadUI = FindObjectOfType<DeadUI>();

        isDead = false;

        if (aniRun != null)
        {
            aniRun.ResetTrigger("isDead");
            aniRun.SetBool("isDead", false);
            aniRun.Play("Idle"); 
        }
    }

    void Dead()
    {
        if (isDead) return;

        isDead = true;
        ps.isStartcut = true;

        aniRun.SetBool("isDead", true);
        Debug.Log("[PlayerMovement] 플레이어가 배터리 방전으로 사망했습니다.");

        StartCoroutine(DelayedDead());
    }
    

    private IEnumerator DelayedDead()
    {
        yield return new WaitForSeconds(3f); // 2초 대기
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        deadUI.ShowDeadPanel();
        Time.timeScale = 0f;
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPickupable pickupable))
            nearbyPickupable = pickupable;

        if (other.TryGetComponent(out IInteractable interactable))
            nearbyInteractable = interactable;

        if (other.TryGetComponent(out Portal portal))   // ← 포탈 감지
            nearbyPortal = portal;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //탈출포탈과 상호작용
        if(other.CompareTag("EndPotal")&&Input.GetKeyDown(KeyCode.F))
            ept.OpenEndPotal();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPickupable pickupable) &&
            pickupable == nearbyPickupable)
            nearbyPickupable = null;

        if (other.TryGetComponent(out IInteractable interactable) &&
            interactable == nearbyInteractable)
            nearbyInteractable = null;

        if (other.TryGetComponent(out Portal p) && p == nearbyPortal)  // ← 포탈 감지 해제
            nearbyPortal = null;
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