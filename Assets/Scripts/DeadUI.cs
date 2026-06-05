using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject deadPanel;

    private void Awake()
    {
        InitializeDeadPanel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeDeadPanel();
    }

    public void ShowDeadPanel()
    {
        if (deadPanel == null)
        {
            InitializeDeadPanel();
        }

        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
        }
    }

    private void InitializeDeadPanel()
    {
        deadPanel = FindDeadPanel();

        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }
    }

    private GameObject FindDeadPanel()
    {
        GameObject uiCanvas = GameObject.Find("Canvas"); 
        
        if (uiCanvas != null)
        {
            Transform panelTransform = uiCanvas.transform.Find("DeadUI");
            if (panelTransform != null)
            {
                return panelTransform.gameObject;
            }
        }

        //실패 대비 예외 처리 (오브젝트가 켜져 있을 때만 작동)
        var panel = GameObject.Find("DeadUI");
        if (panel != null) return panel;

        return GameObject.FindWithTag("DeadUI");
    }
}