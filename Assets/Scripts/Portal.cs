using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum PortalType { MapPortal, EscapePortal }

    [Header("포탈 설정")]
    [SerializeField] private PortalType portalType = PortalType.MapPortal;

    [Tooltip("이동할 씬 이름 (Build Settings 에 등록 필수)")]
    [SerializeField] private string targetSceneName;

    public bool IsUnlocked { get; private set; } = false;
    private bool playerInside = false;
    private bool isGrounded = false;

    private void Awake()
    {
        if (portalType == PortalType.MapPortal) Unlock();
        else Lock();
    }

    private void Update()
    {
        if (!playerInside) return;
        if (!IsUnlocked) return;
        if (!isGrounded) return;
        if (!Input.GetKeyDown(KeyCode.UpArrow)) return;

        Use();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        Debug.Log($"[Portal] 플레이어 진입 → {gameObject.name}");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        isGrounded = false;
    }

    public void SetPlayerGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    private void Use()
    {
        switch (portalType)
        {
            case PortalType.MapPortal: UseMapPortal(); break;
            case PortalType.EscapePortal: UseEscapePortal(); break;
        }
    }

    private void UseMapPortal()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[Portal] '{gameObject.name}' — Target Scene Name 이 비어 있습니다.");
            return;
        }

        // ── 핵심: 현재 씬 이름을 저장 후 이동 ──────────
        PortalTransitionData.PreviousScene =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // ────────────────────────────────────────────────

        Debug.Log($"[Portal] 씬 이동 → {targetSceneName}");

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(targetSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
    }

    private void UseEscapePortal()
    {
        Debug.Log("[Portal] EscapePortal — 추후 구현");
    }

    public void Unlock()
    {
        IsUnlocked = true;
    }

    public void Lock()
    {
        IsUnlocked = false;
    }
}