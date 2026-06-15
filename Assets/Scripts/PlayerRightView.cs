using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRightView : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        if (player != null)
        {
            Vector3 currentScale = player.transform.localScale;
            player.transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }
}
