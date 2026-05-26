using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneTransition : MonoBehaviour
{

    public static SceneTransition Instance { get; private set; }

    [Header("Fade 패널 (전체 화면 검정 Image)")]
    [SerializeField] private Image fadePanel;

    [Header("Fade 속도 (초)")]
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isFading = false;
    private void Awake()
    {
        // 싱글톤 처리 — 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 시작 시 패널 투명하게
        SetAlpha(0f);
        fadePanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 완료 시 Fade In
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (isFading) return;
        StartCoroutine(Transition(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        if (isFading) return;
        StartCoroutine(Transition(sceneIndex));
    }

    private IEnumerator Transition(string sceneName)
    {
        isFading = true;
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
        // Fade In 은 OnSceneLoaded 에서 자동 실행
    }

    private IEnumerator Transition(int sceneIndex)
    {
        isFading = true;
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }

        SetAlpha(1f);
    }

    private IEnumerator FadeIn()
    {
        SetAlpha(1f);
        fadePanel.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }

        SetAlpha(0f);
        fadePanel.gameObject.SetActive(false);
        isFading = false;
    }
    private void SetAlpha(float alpha)
    {
        if (fadePanel == null) return;
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}