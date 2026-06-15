using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("다음 씬)")]
    [SerializeField] private string nextSceneName = "GameScene";
    /*

    [Header("Start ��ư")]
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartClicked);
    }*/

    private void OnStartClicked()
    {
        PlayerPrefs.SetInt("PlayCutscene", 1);  //컷신을 위해 추가
        PlayerPrefs.Save(); //컷신을 위해 추가

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            Debug.LogWarning("[MainMenuUI] SceneTransition �ν��Ͻ��� ã�� �� �����ϴ�.");
    }

    public void ButtonStart()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}