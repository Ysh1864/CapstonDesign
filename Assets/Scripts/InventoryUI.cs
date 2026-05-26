using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("플레이어 참조")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("UI 요소")]
    [SerializeField] private Image itemIcon;  
    [SerializeField] private GameObject emptyLabel; 


    [Header("슬롯 색상")]
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color filledColor = new Color(1f, 1f, 1f, 1f);


    private void Awake()
    {
        // playerInventory 슬롯이 비어 있으면 자동으로 Player에서 찾기
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;
    }

    private void Start()
    {
        // 시작 시 현재 상태로 초기화
        Refresh(playerInventory != null ? playerInventory.CurrentTool : null);
    }

    private void Refresh(ToolObject tool)
    {
        bool hasTool = tool != null && tool.Data != null;

        // 아이콘 갱신
        if (itemIcon != null)
        {
            itemIcon.sprite = hasTool ? tool.Data.icon : null;
            itemIcon.color = hasTool ? filledColor : emptyColor;
            itemIcon.enabled = true;
        }

        // "비어 있음" 레이블 토글
        if (emptyLabel != null)
            emptyLabel.SetActive(!hasTool);
    }
}