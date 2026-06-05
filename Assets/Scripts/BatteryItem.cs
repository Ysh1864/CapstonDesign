using UnityEngine;

public class BatteryItem : MonoBehaviour, IInteractable
{
    [Header("위아래 부유 연출 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 0.2f;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Update()
    {
        OnMove();
    }

    private void OnMove()   // 배터리 아이템이 위아래로 부유하는 연출
    {
        float sinValue = Mathf.Sin(Time.time * moveSpeed);  // 시간에 따라 -1에서 1 사이의 값을 생성
        transform.position = startPosition + new Vector3(0f, sinValue * moveDistance, 0f);  // 원래 위치에서 위아래로 움직임
    }

    public void Interact(PlayerMovement player)
    {
        if (BatteryController.Instance != null)
        {
            BatteryController.Instance.Recharge(20f);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[BatteryItem] 배터리 컨트롤러 인스턴스를 찾을 수 없습니다.");
        }
    }
}