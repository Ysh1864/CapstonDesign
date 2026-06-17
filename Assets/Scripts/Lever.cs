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

    [Header("애니메이션")]
    public Animator leverAnimator;
    [Tooltip("Animator의 Trigger 파라미터 이름")]
    public string pullTriggerName = "Pull";
    [Tooltip("애니메이션 재생 후 벽 제거까지 대기 시간(초). 0이면 즉시 실행")]
    public float animationDelay = 0f;

    [Header("카메라 컷신")]
    public Transform cameraFocusPoint;

    private bool isActivated = false;
    private bool playerInRange = false;
    private string sceneKey;

    private void Start()
    {
        sceneKey = SceneManager.GetActiveScene().name + "_" + leverID;

        if (FunctionalData.IsActivated(sceneKey))
        {
            isActivated = true;
            if (wallTilemap != null)
                wallTilemap.gameObject.SetActive(false);

            if (leverAnimator != null)
                leverAnimator.Play("Lever_Pulled", 0, 1f);
        }
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

        if (leverAnimator != null && !string.IsNullOrEmpty(pullTriggerName))
            leverAnimator.SetTrigger(pullTriggerName);

        if (CameraCutsceneController.Instance != null && cameraFocusPoint != null)
            CameraCutsceneController.Instance.PlayCutscene(cameraFocusPoint);

        if (wallTilemap == null) return;

        StartCoroutine(ActivateWallAfterDelay());
    }

    private IEnumerator ActivateWallAfterDelay()
    {
        if (animationDelay > 0f)
            yield return new WaitForSeconds(animationDelay);

        if (removeMode == WallRemoveMode.Disable)
        {
            wallTilemap.gameObject.SetActive(false);
        }
        else
        {
            yield return StartCoroutine(FadeOutTilemap());
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