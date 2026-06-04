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
}
