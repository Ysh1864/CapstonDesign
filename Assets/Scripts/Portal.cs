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
    private bool isGrounded = false;  // 플레이어 지면 여부 수신


    private void Awake()
    {
        // MapPortal 은 처음부터 열림
        if (portalType == PortalType.MapPortal)
            Unlock();
        else
            Lock();
    }

    private void Update()
    {
        // 조건: 플레이어가 포탈 안 + 포탈 열림 + 지면 위 + ↑ 키
        if (!playerInside) return;

        Debug.Log($"playerInside:{playerInside} | isUnlocked:{IsUnlocked} | isGrounded:{isGrounded} | UpKey:{Input.GetKeyDown(KeyCode.UpArrow)}");

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

    // 해결: Portal과 PlayerMovement가 모두 포탈 진입/퇴장을 감지하면 중복 충돌 이벤트가 발생할 수 있습니다.
    // 하나의 컴포넌트에서만 포탈 진입 상태를 관리하도록 설계하세요.
    // 예) Portal에서는 playerInside/isGrounded 상태만 관리하고, PlayerMovement에서는 포탈 감지 코드를 제거합니다.

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        isGrounded = false;
        Debug.Log($"[Portal] 플레이어 퇴장 → {gameObject.name}");
    }

    // 해결: 포탈 퇴장 감지 역시 Portal에서만 유지해야 합니다. PlayerMovement에서 이중으로 처리될 경우 충돌 처리 순서에 따라 오작동이 발생할 수 있습니다.

    public void SetPlayerGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    private void Use()
    {
        switch (portalType)
        {
            case PortalType.MapPortal:
                UseMapPortal();
                break;

            case PortalType.EscapePortal:
                Debug.Log("[Portal] EscapePortal — 추후 구현");
                break;
        }
    }

    private void UseMapPortal()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[Portal] '{gameObject.name}' — Target Scene Name 이 비어 있습니다.");
            return;
        }

        Debug.Log($"[Portal] 씬 이동 → {targetSceneName}");

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(targetSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
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