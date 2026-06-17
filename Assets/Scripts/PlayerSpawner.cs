using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    private static PlayerSpawner instance;

    public PlayerMovement pm;
    public GameObject player;
    public Vector3 startPoint;
    public Animator am;
    public bool isStartcut = false;
    public float moveDuration = 3f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        CheckAndRunSpawnLogic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        if (pm == null)
            pm = FindObjectOfType<PlayerMovement>();
        if (am == null && pm != null)
            am = pm.GetComponent<Animator>();


        CheckAndRunSpawnLogic();
    }

    private void CheckAndRunSpawnLogic()
    {
        if (PlayerPrefs.GetInt("PlayCutscene", 0) == 1)
        {
            isStartcut = true;
        }

        if (isStartcut)
        {
            GameObject spawnPointObj = GameObject.FindGameObjectWithTag("StartPoint");
            if (spawnPointObj != null)
            {
                startPoint = spawnPointObj.transform.position;
            }
            
            StartCutScene();
        }
        else
        {
            MoveToSpawnPoint();
        }
    }

    private void MoveToSpawnPoint()
    {
        string previousScene = PortalTransitionData.PreviousScene;

        if (string.IsNullOrEmpty(previousScene)) return;

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        if (spawnPoints.Length == 0) return;

        SpawnPoint target = null;

        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.fromScene == previousScene)
            {
                target = sp;
                break;
            }
        }

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
            Vector3 spawnPos = target.transform.position;

            player.transform.position = spawnPos;

            if (pm != null && pm.rb != null)
            {
                pm.rb.position = spawnPos;
                pm.rb.velocity = Vector2.zero;
            }
        }
    }

    public void StartCutScene()
    {
        StartCoroutine(CutSceneRoutine());
    }

    private IEnumerator CutSceneRoutine()
    {
        pm.stopControll = true;

        if (pm.rb != null)
        {
            pm.rb.simulated = false;
            pm.rb.velocity = Vector2.zero;
            pm.rb.bodyType = RigidbodyType2D.Kinematic;
            pm.rb.position = startPoint;
        }
        player.transform.position = startPoint;

        yield return null;
        yield return new WaitForEndOfFrame();

        Vector3 endPos = new Vector3(-13f, player.transform.position.y, player.transform.position.z);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            am.SetBool("isRunning", true);
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            
            Vector3 currentTargetPos = Vector3.Lerp(startPoint, endPos, t);
            
            if (pm.rb != null)
            {
                pm.rb.position = currentTargetPos;
            }
            player.transform.position = currentTargetPos;
            
            yield return null;
        }

        if (pm.rb != null)
        {
            pm.rb.position = endPos;
        }
        player.transform.position = endPos;

        StartCutSceneEnd();
    }

    public void StartCutSceneEnd()
    {
        if (pm.rb != null)
        {
            pm.rb.bodyType = RigidbodyType2D.Dynamic;
            pm.rb.simulated = true;
            pm.rb.velocity = Vector2.zero;
        }

        am.SetBool("isRunning", false);
        pm.stopControll = false;
        isStartcut = false;

        PlayerPrefs.SetInt("PlayCutscene", 0);
        PlayerPrefs.Save();
    }
}