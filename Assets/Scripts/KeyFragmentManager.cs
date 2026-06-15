using UnityEngine;

public class KeyFragmentManager : MonoBehaviour
{
    public static KeyFragmentManager Instance { get; private set; }

    [Header("Key Fragment Settings")]
    [SerializeField] private int totalFragments = 2;

    [Header("Key UI Sprites")]
    [SerializeField] private Sprite bottomKeySprite;
    [SerializeField] private Sprite topKeySprite;
    [SerializeField] private Sprite completeKeySprite;

    private bool map1Collected;
    private bool map2Collected;

    public int TotalFragments => totalFragments;
    public int CollectedCount => (map1Collected ? 1 : 0) + (map2Collected ? 1 : 0);
    public bool HasBottomPiece => map1Collected;
    public bool HasTopPiece => map2Collected;
    public bool HasAllFragments => CollectedCount >= totalFragments;

    public event System.Action<int, int> OnKeyCountChanged;
    public event System.Action OnAllFragmentsCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSpritesIfNeeded();
    }

    private void Start()
    {
        ResetFragments();
    }

    public bool IsCollected(string fragmentId)
    {
        if (fragmentId == "Map1") return map1Collected;
        if (fragmentId == "Map2") return map2Collected;
        return false;
    }

    public void CollectFragment(string fragmentId)
    {
        bool changed = false;

        if (fragmentId == "Map1")
        {
            if (map1Collected) return;
            map1Collected = true;
            changed = true;
        }
        else if (fragmentId == "Map2")
        {
            if (map2Collected) return;
            map2Collected = true;
            changed = true;
        }
        else
        {
            Debug.LogWarning($"[KeyFragmentManager] ¾Ë ¼ö ¾ø´Â ¿­¼è Á¶°¢ ID: {fragmentId}");
            return;
        }

        if (!changed) return;

        Debug.Log($"[KeyFragmentManager] ¿­¼è Á¶°¢ È¹µæ: {CollectedCount}/{totalFragments}");
        Debug.Log($"Map1: {map1Collected}, Map2: {map2Collected}, HasAll: {HasAllFragments}");
        UpdateUI(true);

        if (HasAllFragments)
            OnAllFragmentsCollected?.Invoke();
    }

    public void ResetFragments()
    {
        map1Collected = false;
        map2Collected = false;
        UpdateUI(false);
    }

    public Sprite GetCurrentKeySprite()
    {
        LoadSpritesIfNeeded();

        Debug.Log($"[SpriteCheck] Map1:{map1Collected}, Map2:{map2Collected}, Bottom:{bottomKeySprite?.name}, Top:{topKeySprite?.name}, Complete:{completeKeySprite?.name}");

        if (map1Collected && map2Collected)
            return completeKeySprite;

        if (map1Collected)
            return bottomKeySprite;

        if (map2Collected)
            return topKeySprite;

        return null;
    }

    private void UpdateUI(bool playEffect)
    {
        LoadSpritesIfNeeded();
        OnKeyCountChanged?.Invoke(CollectedCount, totalFragments);

        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.UpdateKeyProgress(
                CollectedCount,
                totalFragments,
                HasAllFragments,
                GetCurrentKeySprite(),
                playEffect
            );
        }
    }

    private void LoadSpritesIfNeeded()
    {
        if (bottomKeySprite == null)
            bottomKeySprite = Resources.Load<Sprite>("QuestAssets/KeyFragment_1");
        if (topKeySprite == null)
            topKeySprite = Resources.Load<Sprite>("QuestAssets/KeyFragment_2");
        if (completeKeySprite == null)
            completeKeySprite = Resources.Load<Sprite>("QuestAssets/KeyComplete");
    }
}
