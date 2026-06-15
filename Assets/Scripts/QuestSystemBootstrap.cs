using UnityEngine;

public class QuestSystemBootstrap : MonoBehaviour
{
    private void Awake()
    {
        EnsureManager();
        EnsureUI();
    }

    private void EnsureManager()
    {
        if (KeyFragmentManager.Instance != null) return;
        if (FindObjectOfType<KeyFragmentManager>() != null) return;
        gameObject.AddComponent<KeyFragmentManager>();
    }

    private void EnsureUI()
    {
        if (QuestUI.Instance != null) return;
        if (FindObjectOfType<QuestUI>() != null) return;

        GameObject ui = new GameObject("QuestUI_Manager");
        ui.AddComponent<QuestUI>();
    }
}
