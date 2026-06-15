using UnityEngine;

public class FloatingGlowEffect : MonoBehaviour
{
    [Header("Floating - Key fragments only")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.18f;

    [Header("Sparkle / Pulse")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float minScale = 0.96f;
    [SerializeField] private float maxScale = 1.06f;
    [SerializeField] private float minAlpha = 0.75f;
    [SerializeField] private float maxAlpha = 1f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private bool signOnlySparkle;

    private void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            startColor = spriteRenderer.color;

        // 표지판은 둥둥 움직이지 않고 반짝임만 적용합니다.
        signOnlySparkle = GetComponent<SignHintObject>() != null;
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        if (!signOnlySparkle)
        {
            float y = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = startPosition + new Vector3(0f, y, 0f);
        }
        else
        {
            transform.position = startPosition;
        }

        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = startScale * scale;

        if (spriteRenderer != null)
        {
            Color c = startColor;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            spriteRenderer.color = c;
        }
    }
}
