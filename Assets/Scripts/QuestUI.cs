using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    private Canvas canvas;
    private GameObject signPanel;
    private Text signText;

    private Image keySlotImage;
    private Image keyIconImage;
    private Text keyCountText;
    private Coroutine keyEffectRoutine;
    [SerializeField] private string endSceneName = "EndScene";

    private readonly Color slotEmptyColor = new Color(1f, 1f, 1f, 0.92f);
    private readonly Color iconHiddenColor = new Color(1f, 1f, 1f, 0f);
    private readonly Color iconVisibleColor = new Color(1f, 1f, 1f, 1f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        HideSign();
        UpdateKeySlotVisibility(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateKeySlotVisibility(scene.name);
    }

    private void UpdateKeySlotVisibility(string sceneName)
    {
        BuildUI();

        bool isEndScene = sceneName == endSceneName;

        if (keySlotImage != null)
        {
            keySlotImage.gameObject.SetActive(!isEndScene);
        }
    }

    public void ShowSign(string message)
    {
        BuildUI();
        signPanel.SetActive(true);
        signText.text = message;
    }

    public void HideSign()
    {
        BuildUI();
        signPanel.SetActive(false);
    }

    // 이전 코드 호환용
    public void UpdateKeyCount(int current, int total, bool complete, Sprite completeSprite)
    {
        UpdateKeyProgress(current, total, complete, completeSprite, false);
    }

    public void UpdateKeyProgress(int current, int total, bool complete, Sprite currentSprite, bool playEffect)
    {
        BuildUI();

        Debug.Log(
            $"[UI] currentSprite = {(currentSprite != null ? currentSprite.name : "NULL")}"
        );

        if (keyCountText != null)
            keyCountText.text = complete ? "완성" : $"{current}/{total}";

        if (keyIconImage != null)
        {
            keyIconImage.sprite = currentSprite;
            keyIconImage.color = currentSprite == null
                ? iconHiddenColor
                : iconVisibleColor;
        }

        if (playEffect && keyIconImage != null && keySlotImage != null)
        {
            PlayKeyUISparkleEffect();
        }
    }

    private void BuildUI()
    {
        if (canvas != null && keySlotImage != null && keyIconImage != null && keyCountText != null && signPanel != null && signText != null)
            return;

        if (canvas != null)
        {
            Destroy(canvas.gameObject);
            canvas = null;
        }

        GameObject canvasObject = new GameObject("QuestUI_Canvas");
        canvasObject.transform.SetParent(transform);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 왼쪽 위 흰색 네모칸: 처음에는 비어 있고, 조각 획득 시 조각 이미지, 2개 획득 시 완성 열쇠 이미지 표시
        GameObject keySlot = CreatePanel("KeyCompleteSlot_WhiteBox", canvasObject.transform, new Vector2(96, 96), new Vector2(0, 1), new Vector2(0, 1), new Vector2(70, -70));
        keySlotImage = keySlot.GetComponent<Image>();
        keySlotImage.color = slotEmptyColor;

        GameObject keyIconObj = new GameObject("KeyImage");
        keyIconObj.transform.SetParent(keySlot.transform, false);
        keyIconImage = keyIconObj.AddComponent<Image>();
        keyIconImage.preserveAspect = true;
        keyIconImage.color = iconHiddenColor;
        RectTransform keyIconRt = keyIconObj.GetComponent<RectTransform>();
        keyIconRt.anchorMin = new Vector2(0.5f, 0.5f);
        keyIconRt.anchorMax = new Vector2(0.5f, 0.5f);
        keyIconRt.pivot = new Vector2(0.5f, 0.5f);
        keyIconRt.anchoredPosition = Vector2.zero;
        keyIconRt.sizeDelta = new Vector2(70, 70);

        GameObject keyCountObj = new GameObject("KeyCountSmallText");
        keyCountObj.transform.SetParent(keySlot.transform, false);
        keyCountText = keyCountObj.AddComponent<Text>();
        keyCountText.font = font;
        keyCountText.fontSize = 18;
        keyCountText.alignment = TextAnchor.LowerRight;
        keyCountText.color = Color.black;
        keyCountText.text = "0/2";
        RectTransform countRt = keyCountObj.GetComponent<RectTransform>();
        countRt.anchorMin = new Vector2(0, 0);
        countRt.anchorMax = new Vector2(1, 1);
        countRt.offsetMin = new Vector2(5, 3);
        countRt.offsetMax = new Vector2(-7, -5);

        signPanel = CreatePanel("SignHintPanel", canvasObject.transform, new Vector2(620, 250), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 145));
        signPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);

        GameObject signTextObj = new GameObject("SignText");
        signTextObj.transform.SetParent(signPanel.transform, false);
        signText = signTextObj.AddComponent<Text>();
        signText.font = font;
        signText.fontSize = 30;
        signText.alignment = TextAnchor.MiddleCenter;
        signText.color = Color.white;
        signText.horizontalOverflow = HorizontalWrapMode.Wrap;
        signText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform signTextRt = signTextObj.GetComponent<RectTransform>();
        signTextRt.anchorMin = new Vector2(0, 0);
        signTextRt.anchorMax = new Vector2(1, 1);
        signTextRt.offsetMin = new Vector2(35, 25);
        signTextRt.offsetMax = new Vector2(-35, -25);
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.65f);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = size;
        return panel;
    }

    private void PlayKeyUISparkleEffect()
    {
        if (keyEffectRoutine != null)
            StopCoroutine(keyEffectRoutine);
        keyEffectRoutine = StartCoroutine(KeyUISparkleRoutine());
    }

    private IEnumerator KeyUISparkleRoutine()
    {
        if (keyIconImage == null || keySlotImage == null)
            yield break;

        RectTransform iconRt = keyIconImage.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;
        Color originalSlotColor = keySlotImage.color;

        float duration = 0.45f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            float pulse = Mathf.Sin(t * Mathf.PI);

            iconRt.localScale = originalScale * (1f + pulse * 0.35f);
            keySlotImage.color = Color.Lerp(originalSlotColor, new Color(1f, 0.92f, 0.35f, 1f), pulse);
            yield return null;
        }

        iconRt.localScale = originalScale;
        keySlotImage.color = originalSlotColor;
        keyEffectRoutine = null;
    }
}
