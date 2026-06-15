using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WallSwitch : MonoBehaviour
{
    [Header("연결 설정")]
    public Tilemap hiddenPlatformTilemap;

    [Header("고유 ID (씬 내에서 겹치지 않게 설정)")]
    public string switchID = "switch_01";

    [Header("비주얼 (선택)")]
    public SpriteRenderer switchRenderer;
    public Sprite offSprite;
    public Sprite onSprite;

    private bool isActivated = false;
    private bool playerInRange = false;
    private string sceneKey;

    private void Start()
    {
        sceneKey = SceneManager.GetActiveScene().name + "_" + switchID;

        // 이전에 활성화된 적 있으면 복원
        if (FunctionalData.IsActivated(sceneKey))
        {
            isActivated = true;
            if (hiddenPlatformTilemap != null)
                hiddenPlatformTilemap.gameObject.SetActive(true);
        }
        else
        {
            if (hiddenPlatformTilemap != null)
                hiddenPlatformTilemap.gameObject.SetActive(false);
        }

        UpdateVisual();
    }

    private void Update()
    {
        if (playerInRange && !isActivated && Input.GetKeyDown(KeyCode.F))
        {
            Activate();
        }
    }

    private void Activate()
    {
        isActivated = true;
        FunctionalData.SetActivated(sceneKey, true);

        if (hiddenPlatformTilemap != null)
            hiddenPlatformTilemap.gameObject.SetActive(true);

        UpdateVisual();
        Debug.Log($"[WallSwitch] 활성화 → 발판 생성 ({sceneKey})");
    }

    private void UpdateVisual()
    {
        if (switchRenderer == null) return;
        if (isActivated && onSprite != null)
            switchRenderer.sprite = onSprite;
        else if (!isActivated && offSprite != null)
            switchRenderer.sprite = offSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}