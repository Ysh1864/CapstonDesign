using UnityEngine;

public class ToolObject : MonoBehaviour, IPickupable
{
    [SerializeField] private ToolData data;

    public ToolData Data => data;

    public bool IsHeld { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void OnSwitch(PlayerMovement player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        inventory.TrySwap(this);
    }

    public void OnPickedUp()
    {
        IsHeld = true;
        SetVisible(false);
    }

    public void OnDropped(Vector2 dropPosition)
    {
        transform.position = dropPosition;
        IsHeld = false;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (col != null) col.enabled = visible;
    }
}