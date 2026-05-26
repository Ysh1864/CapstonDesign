using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    private Transform playerTransform;
    private Vector2 moveVelocity;
    private bool isReturning = false;
    private float timer = 0f;

    [Header("부메랑 프리포머 물리")]
    [SerializeField] private float forwardDuration = 0.5f; // 앞으로 날아가는 시간
    [SerializeField] private float returnSpeed = 14f;       // 돌아올 때 속도
    [SerializeField] private float rotationSpeed = 720f;    // 자전 회전 속도
    [SerializeField] private float catchDistance = 0.6f;   // 회수 판정 거리

    public void Launch(Vector2 initialVelocity, Transform player)
    {
        moveVelocity = initialVelocity;
        playerTransform = player;
        isReturning = false;
        timer = 0f;
    }

    private void Update()
    {
        // 회전 연출
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // 궤적 처리
        if (!isReturning)
        {
            timer += Time.deltaTime;
            transform.Translate(moveVelocity * Time.deltaTime, Space.World);
            // 자연스럽게 감속하며 멈추기
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, timer / forwardDuration);

            if (timer >= forwardDuration) isReturning = true;
        }
        else
        {
            if (playerTransform == null) { Destroy(gameObject); return; }

            // 플레이어 허리 높이 추적
            Vector3 targetPosition = playerTransform.position + Vector3.up * 0.5f;
            Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
            transform.Translate(directionToPlayer * returnSpeed * Time.deltaTime, Space.World);

            // 거리 체크 후 회수
            if (Vector3.Distance(transform.position, targetPosition) <= catchDistance)
            {
                Destroy(gameObject); // 회수 완료
            }
        }
    }
}