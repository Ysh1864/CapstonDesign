using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FishingHookProjectile : MonoBehaviour
{
    private Transform playerTransform;
    private FishingRodAction ownerAction;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isPulling = false;       // 회수 중 여부
    private Transform caughtTarget = null; // 낚인 물체의 Transform
    private Vector3 targetOffset;          // 물체가 찌에 붙을 때의 오프셋

    [Header("찌 물리 설정")]
    [SerializeField] private float pullSpeed = 10f;       // 당겨지는 속도
    [SerializeField] private float catchDistance = 0.5f;   // 플레이어 도달 회수 거리
    [SerializeField] private LayerMask catchableLayer;    // 낚시로 당길 수 있는 오브젝트 레이어
    [SerializeField] private LayerMask groundLayer;       // 지면 레이어 (박히면 멈추게 함)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Launch(Vector2 velocity, Transform player, FishingRodAction owner)
    {
        playerTransform = player;
        ownerAction = owner;
        rb.velocity = velocity;
        isPulling = false;
        caughtTarget = null;
    }

    private void Update()
    {
        // 당기는 상태일 때 플레이어를 향해 이동
        if (isPulling)
        {
            if (playerTransform == null)
            {
                RetireHook();
                return;
            }

            Vector3 targetPos = playerTransform.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.Translate(direction * pullSpeed * Time.deltaTime, Space.World);

            // 낚인 물체가 있다면 찌 위치에 강제로 동기화시켜 끌고 옴
            if (caughtTarget != null)
            {
                caughtTarget.position = transform.position - targetOffset;
            }

            // 플레이어에게 충분히 가까워지면 회수 완료
            if (Vector3.Distance(transform.position, targetPos) <= catchDistance)
            {
                // 만약 도구(ToolObject)를 낚아챈 거였다면 플레이어 발밑에 이쁘게 놔줌
                if (caughtTarget != null)
                {
                    if (caughtTarget.TryGetComponent(out ToolObject tool))
                    {
                        tool.OnDropped(playerTransform.position);
                    }
                    else if (caughtTarget.TryGetComponent(typeof(Rigidbody2D), out var targetRb))
                    {
                        // 일반 물리 상자라면 속도 초기화
                        ((Rigidbody2D)targetRb).velocity = Vector2.zero;
                    }
                }
                RetireHook();
            }
        }
    }

    // 낚싯대를 한 번 더 써서 당기기 시작할 때 호출
    public void PullBack()
    {
        if (isPulling) return;

        isPulling = true;
        rb.isKinematic = true; // 당겨질 때는 기존 물리 끄기
        rb.velocity = Vector2.zero;
        col.enabled = false;   // 회수 중 추가 충돌 방지
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPulling) return;

        // 당길 수 있는 레이어의 물체와 충돌했는지 체크 (낚기 성공)
        if (((1 << collision.gameObject.layer) & catchableLayer) != 0)
        {
            caughtTarget = collision.transform;

            // 만약 물리 엔진을 쓰는 상자라면 일시적으로 물리 연산 정지
            if (caughtTarget.TryGetComponent(out Rigidbody2D targetRb))
            {
                targetRb.velocity = Vector2.zero;
            }

            // 물체와 찌 사이의 거리 오프셋 유지
            targetOffset = transform.position - caughtTarget.position;

            // 물체를 낚자마자 바로 자동으로 끌어당김
            PullBack();
        }
        // 지면에 닿으면 그 자리에 찌가 멈추어 대기함
        else if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

    private void RetireHook()
    {
        if (ownerAction != null)
        {
            ownerAction.ClearHook(); // 낚싯대에 신호 전송
        }
        Destroy(gameObject);
    }
}