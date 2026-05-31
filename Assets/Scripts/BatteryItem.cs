using UnityEngine;

public class BatteryItem : MonoBehaviour, IInteractable
{
    [Header("배터리 충전량")]
    [SerializeField] private float rechargeAmount = 20f; // 획득 시 충전될 양 (+20)

    // 플레이어가 근처에서 F나 아래방향키를 누르면 PlayerMovement를 통해 이 메서드가 실행됩니다.
    public void Interact(PlayerMovement player)
    {
        // 씬에 존재하는 BatteryController를 찾습니다.
        BatteryController batteryController = FindObjectOfType<BatteryController>();

        if (batteryController != null)
        {
            // 배터리 충전 실행
            batteryController.Recharge(rechargeAmount);

            Debug.Log($"[BatteryItem] 배터리를 획득하여 {rechargeAmount}만큼 충전했습니다.");

            // 획득했으므로 배터리 아이템 오브젝트 삭제
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[BatteryItem] 씬에서 BatteryController를 찾을 수 없습니다.");
        }
    }
}