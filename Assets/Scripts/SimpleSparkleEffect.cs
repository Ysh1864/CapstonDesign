using UnityEngine;

public class SimpleSparkleEffect : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.55f;
    [SerializeField] private float startScale = 0.4f;
    [SerializeField] private float endScale = 1.4f;

    private SpriteRenderer spriteRenderer;
    private float timer;

    public void PlayOnce()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("QuestAssets/KeyComplete");
        spriteRenderer.sortingOrder = 50;
        transform.localScale = Vector3.one * startScale;
        timer = 0f;
    }

    private void Update()
    {
        if (spriteRenderer == null)
        {
            PlayOnce();
            return;
        }

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifeTime);
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
        Color c = Color.white;
        c.a = 1f - t;
        spriteRenderer.color = c;

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
