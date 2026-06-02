using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Fade 패널 (전체화면 검정 Image)")]
    [SerializeField] private Image fadePanel;

    [Header("Fade 지속 시간 (초)")]
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isFading = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // fadePanel 없이 자동 생성된 경우 건너뜀
        if (fadePanel == null) return;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadePanel == null) return;
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
        if (fadePanel != null)
            yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Transition(int sceneIndex)
    {
        isFading = true;
        if (fadePanel != null)
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