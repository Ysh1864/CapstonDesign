using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NextArea : MonoBehaviour
{
    Collider2D col;
    public string nextSceneName;


    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene(nextSceneName);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}
