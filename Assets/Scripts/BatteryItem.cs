using UnityEngine;

public class BatteryItem : MonoBehaviour, IInteractable
{
    [Header("배터리 충전량")]
    [SerializeField] private float rechargeAmount = 20f; // 획득 시 충전될 양

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
    public void Interact(PlayerMovement player)
    {
        BatteryController batteryController = FindObjectOfType<BatteryController>();

        if (batteryController != null)
        {
            batteryController.Recharge(rechargeAmount);
            Debug.Log($"[BatteryItem] 배터리를 획득하여 {rechargeAmount}만큼 충전했습니다.");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[BatteryItem] 씬에서 BatteryController를 찾을 수 없습니다.");
        }
    }

    private void OnMove()   // 배터리 아이템이 위아래로 부유하는 연출
    {
        float sinValue = Mathf.Sin(Time.time * moveSpeed);  // 시간에 따라 -1에서 1 사이의 값을 생성
        transform.position = startPosition + new Vector3(0f, sinValue * moveDistance, 0f);  // 원래 위치에서 위아래로 움직임
    }
}