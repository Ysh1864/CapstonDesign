using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ClearUI : MonoBehaviour
{
    public GameObject clearPanel;

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
        if (scene.name == "EndScene")
        {   
            StartCoroutine(ViewBackGrownd());   //배경 감상 후 클리어 UI
        }
        else
        {
            clearPanel.SetActive(false);
        }
    }
    private IEnumerator ViewBackGrownd() 
    {
        yield return new WaitForSeconds(5f);
        clearPanel.SetActive(true);
    }
}
