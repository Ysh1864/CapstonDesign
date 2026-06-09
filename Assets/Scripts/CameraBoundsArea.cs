using UnityEngine;

public class CameraBoundsArea : MonoBehaviour
{
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private void Start()
    {
        CameraBoundsLimiter limiter = FindObjectOfType<CameraBoundsLimiter>();

        if (limiter != null)
        {
            limiter.SetBounds(minBounds, maxBounds);
        }
    }
}