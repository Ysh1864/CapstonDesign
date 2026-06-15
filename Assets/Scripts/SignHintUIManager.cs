using UnityEngine;
using UnityEngine.UI;

public class SignHintUIManager : MonoBehaviour
{
    public static SignHintUIManager Instance;

    public GameObject panel;
    public Text messageText;

    private void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (panel != null) panel.SetActive(true);
        if (messageText != null) messageText.text = message;
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}