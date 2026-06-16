using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndImage : MonoBehaviour
{
    public GameObject image0;
    public GameObject image1;
    public GameObject image2;
    public GameObject button;

    void Start()
    {
        StartCoroutine(ChangeIMG());
    }
    
    IEnumerator ChangeIMG()
    {
        yield return new WaitForSeconds(1.5f);
        image0.SetActive(false);
        image1.SetActive(true);
   
        yield return new WaitForSeconds(1.5f);
        image1.SetActive(false);
        image2.SetActive(true);
        button.SetActive(true);
}   
}
