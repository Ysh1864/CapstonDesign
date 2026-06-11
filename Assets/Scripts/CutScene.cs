using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("PlayCutscene", 1);  //컷신을 위해 추가
        PlayerPrefs.Save(); //컷신을 위해 추가
    }
}
