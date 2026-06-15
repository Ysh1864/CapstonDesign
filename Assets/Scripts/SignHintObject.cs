using UnityEngine;

public class SignHintObject : MonoBehaviour
{
    [TextArea]
    public string message;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("표지판 트리거 감지됨: " + other.name);

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log("플레이어 감지 성공");

            if (SignHintUIManager.Instance != null)
                SignHintUIManager.Instance.Show(message);
            else
                Debug.LogWarning("SignHintUIManager가 없습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("표지판 트리거 나감: " + other.name);

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            if (SignHintUIManager.Instance != null)
                SignHintUIManager.Instance.Hide();
        }
    }
}