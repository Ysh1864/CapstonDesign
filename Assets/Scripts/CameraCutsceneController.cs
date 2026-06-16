using System.Collections;
using UnityEngine;

public class CameraCutsceneController : MonoBehaviour
{
    public static CameraCutsceneController Instance;

    [Header("Target")]
    public Transform player;

    [Header("Camera Settings")]
    public float moveDuration = 1.2f;
    public float stayTime = 2f;

    private bool isPlaying;
    private CameraFollow2D cameraFollow;

    private void Awake()
    {
        Instance = this;
        cameraFollow = GetComponent<CameraFollow2D>();
    }

    public void PlayCutscene(Transform focusPoint)
    {
        if (isPlaying) return;
        if (focusPoint == null) return;

        StartCoroutine(CutsceneRoutine(focusPoint));
    }

    private IEnumerator CutsceneRoutine(Transform focusPoint)
    {
        isPlaying = true;

        // 기존 카메라 따라가기 잠시 끄기
        if (cameraFollow != null)
            cameraFollow.enabled = false;

        Vector3 startPos = transform.position;

        Vector3 focusPos = new Vector3(
            focusPoint.position.x,
            focusPoint.position.y,
            transform.position.z
        );

        // 카메라가 기믹 위치로 이동
        float time = 0f;
        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;

            transform.position = Vector3.Lerp(startPos, focusPos, t);
            yield return null;
        }

        transform.position = focusPos;

        // 기믹 위치 보여주기
        yield return new WaitForSeconds(stayTime);

        // 플레이어 위치로 돌아가기
        Vector3 returnStartPos = transform.position;

        Vector3 playerPos = transform.position;

        if (player != null)
        {
            playerPos = new Vector3(
                player.position.x,
                player.position.y + 1.5f,
                transform.position.z
            );
        }

        time = 0f;
        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = time / moveDuration;

            transform.position = Vector3.Lerp(returnStartPos, playerPos, t);
            yield return null;
        }

        // 기존 카메라 따라가기 다시 켜기
        if (cameraFollow != null)
            cameraFollow.enabled = true;

        isPlaying = false;
    }
}