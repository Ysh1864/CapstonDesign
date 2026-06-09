using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndPotalTrigger : MonoBehaviour
{
    public bool isKey = false;

    [Header("탈출 성공 시 이동할 씬 이름")]
    [SerializeField] private string targetSceneName = "MainMenu";

    public PlayerMovement pm;
    public GameObject KeySprite;

    void Update()
    {
        HaveKey();
    }
    
    public void HaveKey()
    {
        if(isKey)
            KeySprite.SetActive(true);
        else
            KeySprite.SetActive(false);
    }

    public void OpenEndPotal()
    {
        if(isKey)
        {
            //포탈과 상호작용 가능.
            Debug.Log("[EndPotalTrigger] 열쇠 확인 완료! 탈출 포탈 작동합니다.");

            StartCoroutine(EndScene());
        }
    }
    IEnumerator EndScene()
    {
        //포탈 해금 연출
        pm.stopControll = true;
        yield return new WaitForSeconds(3f);
        
        //페이드 아웃 연출
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(targetSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
    }

}
