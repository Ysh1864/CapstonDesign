using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }
    [SerializeField] private float fadeDuration = 0.6f;

    private Image fadePanel;
    private Canvas canvas;
    private bool isFading = false;
    private bool isFirstScene = true;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateFadePanel();
    }

    private void CreateFadePanel()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        
        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadePanel = panel.AddComponent<Image>();
        fadePanel.color = new Color(0f, 0f, 0f, 0f);
        fadePanel.raycastTarget = false;
        panel.SetActive(false);
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
        if (isFirstScene)
        {
            isFirstScene = false; // 이후 씬부터는 FadeIn 적용
            return;
        }
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
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}