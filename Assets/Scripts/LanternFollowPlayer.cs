using UnityEngine;

public class LanternFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.7f, 0f);
    [SerializeField] private bool flipOffsetWithFacing = true;
    [SerializeField] private float sideOffset = 0.25f;

    private PlayerMovement playerMovement;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (target != null)
            playerMovement = target.GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 finalOffset = offset;

        if (flipOffsetWithFacing && playerMovement != null)
        {
            finalOffset.x += sideOffset * playerMovement.FacingDirection;
        }

        transform.position = target.position + finalOffset;
    }
}
