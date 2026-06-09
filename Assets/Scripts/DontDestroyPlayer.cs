using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPlayer : MonoBehaviour
{
    public static DontDestroyPlayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
