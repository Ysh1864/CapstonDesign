
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WallSwitch : MonoBehaviour
{
    [Header("연결 설정")]
    public Tilemap hiddenPlatformTilemap;

    [Header("고유 ID (씬 내에서 겹치지 않게 설정)")]
    public string switchID = "switch_01";

    [Header("카메라 컷신")]
    public Transform cameraFocusPoint;

    private bool isActivated = false;
    private bool playerInRange = false;
    private string sceneKey;

    private void Start()
    {
        sceneKey = SceneManager.GetActiveScene().name + "_" + switchID;

        if (FunctionalData.IsActivated(sceneKey))
        {
            isActivated = true;
            if (hiddenPlatformTilemap != null)
                hiddenPlatformTilemap.gameObject.SetActive(true);
        }
        else
        {
            if (hiddenPlatformTilemap != null)
                hiddenPlatformTilemap.gameObject.SetActive(false);
        }
       
    }

    private void Update()
    {
        if (playerInRange && !isActivated && Input.GetKeyDown(KeyCode.F))
        {
            Activate();
        }
    }

    private void Activate()
    {
        isActivated = true;
        FunctionalData.SetActivated(sceneKey, true);

        if (hiddenPlatformTilemap != null)
            hiddenPlatformTilemap.gameObject.SetActive(true);

        if (CameraCutsceneController.Instance != null && cameraFocusPoint != null)
            CameraCutsceneController.Instance.PlayCutscene(cameraFocusPoint);        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
