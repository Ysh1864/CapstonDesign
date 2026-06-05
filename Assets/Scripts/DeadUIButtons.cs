using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUIButtons : MonoBehaviour
{
    [Header("씬 설정")]
    [SerializeField] private string sceneName = "GameScene"; // 로드할 씬 이름

    public void RetryButton()
    {
        Time.timeScale = 1f;

        if (BatteryController.Instance != null)
        {
            BatteryController.Instance.ReviveAndResetBattery();
        }

        SceneManager.LoadScene(sceneName);
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}