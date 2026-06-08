using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private static CameraFollow2D instance; //추가 싱글톤
    private Vector3 velocity;
    private float currentLookAhead;
    private void Awake()
    {
        noDestroy();    //추가 파괴되지 않음
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
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
            foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
            {
                if (cam.gameObject != gameObject)
                {
                    Destroy(cam.gameObject);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetLookAhead = horizontalInput * lookAheadDistance;
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSmooth * Time.deltaTime);

        Vector3 desiredPosition = target.position + offset + new Vector3(currentLookAhead, 0f, 0f);

        if (useSmoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = desiredPosition;
        }

        if (useBounds)
        {
            Vector3 clamped = transform.position;
            clamped.x = Mathf.Clamp(clamped.x, minBounds.x, maxBounds.x);
            clamped.y = Mathf.Clamp(clamped.y, minBounds.y, maxBounds.y);
            transform.position = clamped;
        }
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;

        if (useBounds)
        {
            Vector3 clamped = transform.position;
            clamped.x = Mathf.Clamp(clamped.x, minBounds.x, maxBounds.x);
            clamped.y = Mathf.Clamp(clamped.y, minBounds.y, maxBounds.y);
            transform.position = clamped;
        }
    }

    void noDestroy()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
