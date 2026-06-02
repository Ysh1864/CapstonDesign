using UnityEngine;

/// <summary>
/// 간소화된 플레이어 인벤토리.
/// 랜턴은 항상 소지 (별도 슬롯 불필요).
/// 열쇠 보유 여부만 관리 — GameManager 와 동기화됨.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerInventory : MonoBehaviour
{

   /* [Header("랜턴 (항상 소지)")]
    [SerializeField] private LanternController lantern;

    public LanternController Lantern => lantern;

    public bool HasKey => GameManager.Instance != null && GameManager.Instance.HasKey;

    private void Awake()
    {
        // 랜턴이 슬롯에 없으면 자식에서 자동 탐색
        if (lantern == null)
            lantern = GetComponentInChildren<LanternController>();

        if (lantern == null)
            Debug.LogWarning("[PlayerInventory] LanternController 를 찾을 수 없습니다.");
    }*/
}