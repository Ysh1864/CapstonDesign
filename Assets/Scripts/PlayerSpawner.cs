using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    private static PlayerSpawner instance;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        rb = GetComponent<Rigidbody2D>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MoveToSpawnPoint();
    }

    private void MoveToSpawnPoint()
    {
        string previousScene = PortalTransitionData.PreviousScene;

        // 최초 시작이면 이동하지 않음
        if (string.IsNullOrEmpty(previousScene)) return;

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        if (spawnPoints.Length == 0)
        {            
            return;
        }

        SpawnPoint target = null;

        // 1순위: fromScene 이 previousScene 과 일치
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.fromScene == previousScene)
            {
                target = sp;
                break;
            }
        }

        // 2순위: fromScene 이 비어있는 기본 SpawnPoint
        if (target == null)
        {
            foreach (SpawnPoint sp in spawnPoints)
            {
                if (string.IsNullOrEmpty(sp.fromScene))
                {
                    target = sp;
                    break;
                }
            }
        }

        if (target != null)
        {
            Vector2 spawnPos = target.transform.position;

            // transform 과 Rigidbody2D 둘 다 설정 (물리 충돌 방지)
            transform.position = spawnPos;
            if (rb != null)
            {
                rb.position = spawnPos;
                rb.velocity = Vector2.zero;
            }

            Debug.Log($"[PlayerSpawner] '{previousScene}' → '{target.name}' ({spawnPos})");
        }
        
    }
}