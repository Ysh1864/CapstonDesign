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

    [Header("애니메이션")]
    public Animator leverAnimator;
    [Tooltip("Animator의 Trigger 파라미터 이름")]
    public string pullTriggerName = "Pull";
    [Tooltip("애니메이션 재생 후 벽 제거까지 대기 시간(초). 0이면 즉시 실행")]
    public float animationDelay = 0f;

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

            // 애니메이션도 당겨진 상태로 즉시 전환 (트랜지션 없이)
            if (leverAnimator != null)
                leverAnimator.Play("Pulled", 0, 1f); // "Pulled" 스테이트 이름은 실제 State 이름과 맞춰주세요
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

        // 레버 당기기 애니메이션 재생
        if (leverAnimator != null && !string.IsNullOrEmpty(pullTriggerName))
        {
            leverAnimator.SetTrigger(pullTriggerName);
            Debug.Log($"[Lever] SetTrigger 호출: {pullTriggerName}, Animator: {leverAnimator.name}");
        }
        else
        {
            Debug.LogWarning("[Lever] leverAnimator 또는 pullTriggerName이 비어있음!");
        }

        if (wallTilemap == null)
        {
            Debug.LogWarning("[Lever] wallTilemap이 연결되지 않았습니다.");
            return;
        }

        StartCoroutine(ActivateWallAfterDelay());
    }

    private IEnumerator ActivateWallAfterDelay()
    {
        if (animationDelay > 0f)
            yield return new WaitForSeconds(animationDelay);

        if (removeMode == WallRemoveMode.Disable)
        {
            wallTilemap.gameObject.SetActive(false);
            Debug.Log($"[Lever] 벽 비활성화 ({sceneKey})");
        }
        else
        {
            yield return StartCoroutine(FadeOutTilemap());
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