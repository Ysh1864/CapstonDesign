using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyFragmentObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string fragmentId = "Map1";
    [SerializeField] private GameObject collectEffectPrefab;

    [Header("Collect Motion")]
    [SerializeField] private float collectDuration = 0.55f;
    [SerializeField] private float jumpHeight = 1.1f;
    [SerializeField] private float rotateSpeed = 540f;
    [SerializeField] private Vector3 playerArriveOffset = new Vector3(0f, 1.1f, 0f);

    private bool collected;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (col != null)
            col.isTrigger = true;

        if (KeyFragmentManager.Instance != null && KeyFragmentManager.Instance.IsCollected(fragmentId))
            Destroy(gameObject);
    }

    public void Interact(PlayerMovement player)
    {
        if (collected) return;
        if (player == null) return;

        collected = true;

        // 중복 상호작용 방지를 위해 현재 오브젝트와 자식 오브젝트의 모든 Collider를 끕니다.
        DisableAllColliders();

        StartCoroutine(CollectMotion(player.transform));
    }

    private IEnumerator CollectMotion(Transform playerTransform)
    {
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < collectDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / collectDuration);

            Vector3 endPosition = playerTransform != null
                ? playerTransform.position + playerArriveOffset
                : startPosition + Vector3.up;

            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, t);
            nextPosition.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = nextPosition;
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        FinishCollect();
    }

    private void FinishCollect()
    {
        if (KeyFragmentManager.Instance != null)
        {
            KeyFragmentManager.Instance.CollectFragment(fragmentId);
        }
        else
        {
            Debug.LogWarning("[KeyFragmentObject] KeyFragmentManager.Instance가 없습니다.");
        }

        // 실제 보이는 이미지가 자식 오브젝트에 있어도 남지 않도록 전부 끕니다.
        HideAllRenderers();
        DisableAllColliders();

        SpawnCollectEffect();

        // 한 프레임 정도 뒤에 삭제해서 UI/이펙트 호출이 안정적으로 끝나게 합니다.
        Destroy(gameObject, 0.1f);
    }

    private void HideAllRenderers()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // 기존 단일 SpriteRenderer 참조도 같이 처리합니다.
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    private void DisableAllColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        // 기존 단일 Collider 참조도 같이 처리합니다.
        if (col != null)
            col.enabled = false;
    }

    private void SpawnCollectEffect()
    {
        if (collectEffectPrefab != null)
        {
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            return;
        }

        GameObject effect = new GameObject("KeyCollectSparkle_Effect");
        effect.transform.position = transform.position;
        SimpleSparkleEffect sparkle = effect.AddComponent<SimpleSparkleEffect>();
        sparkle.PlayOnce();
    }
}
