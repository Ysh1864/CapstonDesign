using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInventory : MonoBehaviour
{

    [Header("지면 레이어 (PlayerStatData 와 동일하게 설정)")]
    [SerializeField] private LayerMask groundLayer;

    public ToolObject CurrentTool;
    public bool HasTool
    {
        get { return CurrentTool != null; }
    }

    public event System.Action<ToolObject> OnInventoryChanged;

    public void TrySwap(ToolObject newTool)
    {
        if (newTool == null) return;
        if (newTool == CurrentTool) return;

        // 1. 기존 도구 내려놓기
        if (CurrentTool != null)
            DropCurrent();

        // 2. 새 도구 집기
        PickUp(newTool);
    }

    public void DropCurrent()
    {
        if (CurrentTool == null) return;

        Vector2 dropPos = GetDropPosition();
        CurrentTool.OnDropped(dropPos);

        CurrentTool = null;
        OnInventoryChanged?.Invoke(null);
    }


    private void PickUp(ToolObject tool)
    {
        CurrentTool = tool;
        tool.OnPickedUp();
        OnInventoryChanged?.Invoke(CurrentTool);
    }


    private Vector2 GetDropPosition()
    {
        // 도구 콜라이더 절반 높이
        float toolHalfHeight = 0f;
        if (CurrentTool != null)
        {
            Collider2D toolCol = CurrentTool.GetComponent<Collider2D>();
            if (toolCol != null)
                toolHalfHeight = toolCol.bounds.extents.y;
        }

        // 플레이어 위치에서 아래로 레이캐스트 → 지면 표면 Y 탐색
        Vector2 rayOrigin = (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 5f, groundLayer);

        if (hit.collider != null)
        {
            // 지면 표면 Y + 도구 절반 높이 = 도구가 지면 위에 딱 붙는 위치
            return new Vector2(transform.position.x, hit.point.y + toolHalfHeight);
        }

        // 폴백: 플레이어 발 근처
        return (Vector2)transform.position + Vector2.down * 0.5f;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        string label = (CurrentTool != null)
            ? "[인벤토리] " + (CurrentTool.Data != null ? CurrentTool.Data.toolName : "이름없음")
            : "[인벤토리] 빈 슬롯";

        GUI.Label(new Rect(10, 10, 300, 30), label);
    }
#endif
}