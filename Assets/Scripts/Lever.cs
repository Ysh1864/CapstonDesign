using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Lever : MonoBehaviour
{
    public enum WallRemoveMode
    {
        Disable,
        FadeOut
    }

    [Header("연결 설정")]
    public Tilemap wallTilemap;

    [Header("고유 ID (씬 내에서 겹치지 않게 설정)")]
    public string leverID = "lever_01";

    [Header("제거 방식")]
    public WallRemoveMode removeMode = WallRemoveMode.FadeOut;

    [Header("페이드 설정 (FadeOut 모드)")]
    public float fadeDuration = 1.0f;

    [Header("비주얼 (선택)")]
    public SpriteRenderer leverRenderer;
    public Sprite offSprite;
    public Sprite onSprite;

    private bool isActivated = false;
    private bool playerInRange = false;
    private string sceneKey;

    private void Start()
    {
        sceneKey = SceneManager.GetActiveScene().name + "_" + leverID;

        // 이전에 활성화된 적 있으면 복원
        if (FunctionalData.IsActivated(sceneKey))
        {
            isActivated = true;
            if (wallTilemap != null)
                wallTilemap.gameObject.SetActive(false);
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
        UpdateVisual();

        if (wallTilemap == null)
        {
            Debug.LogWarning("[Lever] wallTilemap이 연결되지 않았습니다.");
            return;
        }

        if (removeMode == WallRemoveMode.Disable)
        {
            wallTilemap.gameObject.SetActive(false);
            Debug.Log($"[Lever] 벽 비활성화 ({sceneKey})");
        }
        else
        {
            StartCoroutine(FadeOutTilemap());
            Debug.Log($"[Lever] 벽 페이드아웃 시작 ({sceneKey})");
        }
    }

    private IEnumerator FadeOutTilemap()
    {
        TilemapCollider2D col = wallTilemap.GetComponent<TilemapCollider2D>();
        if (col != null) col.enabled = false;

        CompositeCollider2D composite = wallTilemap.GetComponent<CompositeCollider2D>();
        if (composite != null) composite.enabled = false;

        float elapsed = 0f;
        Color startColor = wallTilemap.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            wallTilemap.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        wallTilemap.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        if (leverRenderer == null) return;
        if (isActivated && onSprite != null)
            leverRenderer.sprite = onSprite;
        else if (!isActivated && offSprite != null)
            leverRenderer.sprite = offSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Lever] 트리거 감지: {other.name}");
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}