using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndPotalTrigger : MonoBehaviour
{
    public bool isKey =false;

    [Header("탈출 성공 시 이동할 씬 이름")]
    [SerializeField] private string nextSceneName = "Map3";

    public PlayerMovement pm;
    //public GameObject KeySprite;

    void Start()
    {
        isKey = KeyFragmentManager.Instance.HasAllFragments;
    }
    void Update()
    {
        HaveKey();
    }
    
    public void HaveKey()
    {
       if(KeyFragmentManager.Instance.CollectedCount != 2)
            return;

        if(KeyFragmentManager.Instance.CollectedCount == 2)
            isKey = true;
    }

    public void OpenEndPotal()
    {
        EndScene();
    }
    public void EndScene()
    {
        pm.stopControll = true;
        StartCoroutine(Stay());
    }

    IEnumerator Stay()
    {
        if(isKey)
        {
           yield return new WaitForSeconds(1f);
            
            pm.stopControll = false;

            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene(nextSceneName);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
        else
            pm.stopControll = false;
    }

}
