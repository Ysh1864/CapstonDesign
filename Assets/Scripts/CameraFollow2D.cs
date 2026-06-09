using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Smooth Follow")]
    [SerializeField] private bool useSmoothFollow = true;
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 1.2f;
    [SerializeField] private float lookAheadSmooth = 3f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private bool autoFindBounds = true;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] private float boundsPadding = 0.2f;

    private static CameraFollow2D instance;
    private Camera cam;
    private Vector3 velocity;
    private float currentLookAhead;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        DontDestroyCamera();
    }

    private void Start()
    {
        if (target == null)
            FindPlayerTarget();

        RecalculateBounds();
        SnapToTarget();
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
        if (instance != this) return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Camera otherCamera in root.GetComponentsInChildren<Camera>(true))
            {
                if (otherCamera.gameObject != gameObject)
                    Destroy(otherCamera.gameObject);
            }
        }

        FindPlayerTarget();
        RecalculateBounds();
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetLookAhead = horizontalInput * lookAheadDistance;
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSmooth * Time.deltaTime);

        Vector3 desiredPosition = target.position + offset + new Vector3(currentLookAhead, 0f, 0f);

        if (useSmoothFollow)
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        else
            transform.position = desiredPosition;

        ClampToBounds();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        ClampToBounds();
    }

    public void RecalculateBounds()
    {
        if (!useBounds || !autoFindBounds) return;

        Bounds? foundBounds = null;

        foreach (TilemapRenderer tilemapRenderer in FindObjectsOfType<TilemapRenderer>())
        {
            if (!tilemapRenderer.enabled) continue;
            Bounds rendererBounds = tilemapRenderer.bounds;
            if (rendererBounds.size.x <= 0.01f || rendererBounds.size.y <= 0.01f) continue;

            if (foundBounds == null)
                foundBounds = rendererBounds;
            else
            {
                Bounds combined = foundBounds.Value;
                combined.Encapsulate(rendererBounds.min);
                combined.Encapsulate(rendererBounds.max);
                foundBounds = combined;
            }
        }

        if (foundBounds == null)
        {
            foreach (Renderer renderer in FindObjectsOfType<Renderer>())
            {
                if (!renderer.enabled) continue;
                if (!(renderer is SpriteRenderer) && !(renderer is TilemapRenderer)) continue;

                Bounds rendererBounds = renderer.bounds;
                if (rendererBounds.size.x <= 0.01f || rendererBounds.size.y <= 0.01f) continue;

                if (foundBounds == null)
                    foundBounds = rendererBounds;
                else
                {
                    Bounds combined = foundBounds.Value;
                    combined.Encapsulate(rendererBounds.min);
                    combined.Encapsulate(rendererBounds.max);
                    foundBounds = combined;
                }
            }
        }

        if (foundBounds == null) return;

        Bounds mapBounds = foundBounds.Value;
        minBounds = new Vector2(mapBounds.min.x + boundsPadding, mapBounds.min.y + boundsPadding);
        maxBounds = new Vector2(mapBounds.max.x - boundsPadding, mapBounds.max.y - boundsPadding);
    }

    private void ClampToBounds()
    {
        if (!useBounds) return;
        if (cam == null) cam = GetComponent<Camera>();

        Vector3 clamped = transform.position;

        float halfHeight = 0f;
        float halfWidth = 0f;

        if (cam != null && cam.orthographic)
        {
            halfHeight = cam.orthographicSize;
            halfWidth = halfHeight * cam.aspect;
        }

        float minX = minBounds.x + halfWidth;
        float maxX = maxBounds.x - halfWidth;
        float minY = minBounds.y + halfHeight;
        float maxY = maxBounds.y - halfHeight;

        if (minX <= maxX)
            clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        else
            clamped.x = (minBounds.x + maxBounds.x) * 0.5f;

        if (minY <= maxY)
            clamped.y = Mathf.Clamp(clamped.y, minY, maxY);
        else
            clamped.y = (minBounds.y + maxBounds.y) * 0.5f;

        transform.position = clamped;
    }

    private void FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    private void DontDestroyCamera()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != this)
            Destroy(gameObject);
    }
}
