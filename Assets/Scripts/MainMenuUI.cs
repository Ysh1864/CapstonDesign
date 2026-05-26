using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("이동할 씬 이름 (Build Settings 에 등록된 이름)")]
    [SerializeField] private string nextSceneName = "GameScene";

    [Header("Start 버튼")]
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
    }

    private void OnStartClicked()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(nextSceneName);
        else
            Debug.LogWarning("[MainMenuUI] SceneTransition 인스턴스를 찾을 수 없습니다.");
    }
}