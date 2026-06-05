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
        BatteryController.OnBatteryEmpty += HandleBatteryEmpty;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryEmpty -= HandleBatteryEmpty;
    }

    private void HandleBatteryEmpty()
    {
        if (isGameOver) return;

        isGameOver = true;
        
        //if (freezeTimeOnGameOver)
        //    Time.timeScale = 0f;
        //현재 해당 부분은 라이트가 어두워지며 사망하는 연출에서 바로 타임 스케일 0이되면 
        //데드 애니메이션이 재생되지 않는 문제가 있어 일단 주석 처리했습니다. 
    }

/*
    public void TriggerSceneChangeAfterGameOver()
    {
        Time.timeScale = 1f;

        if (SceneExistsInBuildSettings(gameOverSceneName))  //
           SceneManager.LoadScene(gameOverSceneName);
        else if (reloadCurrentSceneIfMissing)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
*/
//위 매서드는 게임 오버 UI와 연동이 필요하여 일단 주석처리 했습니다
/*
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
*/
//위와 같은 이유입니다.
}
