using UnityEngine;

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Camera))]
public class CameraBoundsLimiter : MonoBehaviour
{
    public bool useBounds = true;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (!useBounds) return;

        Vector3 pos = transform.position;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

        transform.position = pos;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
    }
}