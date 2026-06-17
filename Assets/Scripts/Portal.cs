using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum PortalType { MapPortal, EscapePortal }

    [SerializeField] private PortalType portalType = PortalType.MapPortal;
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
        }
    }

    private void UseMapPortal()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            return;
        }

        PortalTransitionData.PreviousScene =
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;


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