using UnityEngine;
using UnityEngine.SceneManagement;

public class BatteryGameOverHandler : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "MainMenuScene";
    [SerializeField] private bool reloadCurrentSceneIfMissing = true;
    [SerializeField] private bool freezeTimeOnGameOver = true;

    private bool isGameOver;

    private void OnEnable()
    {
        BatteryController.OnBatteryChanged += HandleBatteryChanged;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryChanged -= HandleBatteryChanged;
    }

    private void HandleBatteryChanged(float currentBattery, int stageIndex)
    {
        if (isGameOver) return;
        if (currentBattery > 0f) return;

        isGameOver = true;

        if (freezeTimeOnGameOver)
            Time.timeScale = 0f;
    }

    public void TriggerSceneChangeAfterGameOver()
    {
        Time.timeScale = 1f;

        if (SceneExistsInBuildSettings(gameOverSceneName))
            SceneManager.LoadScene(gameOverSceneName);
        else if (reloadCurrentSceneIfMissing)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool SceneExistsInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (fileName == sceneName)
                return true;
        }
        return false;
    }
}
