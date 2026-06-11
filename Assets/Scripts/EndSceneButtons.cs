using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneButtons : MonoBehaviour
{
    public string nextSceneName;
    public void nextBT()
    {
        PlayerPrefs.SetInt("PlayCutscene", 1);  //컷신을 위해 추가
        PlayerPrefs.Save(); //컷신을 위해 추가
        
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(nextSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}
