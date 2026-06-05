using UnityEngine;

public class BatteryPickupItem : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 20f;
    [SerializeField] private BatteryController batteryController;
    [SerializeField] private bool destroyOnPickup = true;

    private void Start()
    {
        if (batteryController == null)
            batteryController = FindObjectOfType<BatteryController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (batteryController == null) return;

        batteryController.Recharge(rechargeAmount);

        if (destroyOnPickup)
            Destroy(gameObject);
    }

    // 해결: BatteryPickupItem도 자체 OnTriggerEnter2D를 사용하므로, PlayerMovement의 충돌 감지 및 Portal의 포탈 진입 감지와 겹칠 수 있습니다.
    // 배터리 아이템은 자동 픽업 처리만 담당하고, PlayerMovement에서는 중복된 배터리 감지 로직을 주석 처리해야 합니다.
}
