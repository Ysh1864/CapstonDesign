using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BatteryController : MonoBehaviour
{
    public static BatteryController Instance { get; private set; }
    public static event Action<float, int> OnBatteryChanged;
    public static event Action OnBatteryRecharged; // 충전 시 이벤트
    public static event Action OnBatteryEmpty; // 방전 시 발생,

    [Header("배터리 설정")]
    [SerializeField] private float maxBattery = 100f;   //최대 배터리 양
    [SerializeField] private float drainBattery = 1f;   //초당 소모되는 배터리 양
    [SerializeField] private float currentBattery = 100f; //현재 배터리 양

    [Header("배터리가 닳지 않는 씬 목록")]
    [SerializeField] private List<string> nonDrainingScenes = new List<string> { "MainMenu", "SafeZone" };
    
     
    private Coroutine drainRoutine;

    public float DrainBatteryAmount => drainBattery;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Undestroyed();

        if (Instance == this)
        {
            currentBattery = maxBattery;
        }
    }

    private void OnEnable()
    {
        if (Instance != null && Instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this) return;

        if (drainRoutine != null) StopCoroutine(drainRoutine);  

        if (nonDrainingScenes.Contains(scene.name)) 
        {
            currentBattery = maxBattery;
        }
        else
        {
            if (currentBattery <= 0f && scene.buildIndex == 0) 
            {
                currentBattery = maxBattery;
            }
        }

        OnBatteryChanged?.Invoke(currentBattery, GetCurrentStage());

        if (!nonDrainingScenes.Contains(scene.name))
        {
            drainRoutine = StartCoroutine(DrainBatteryRoutine());   
        }
    }

    public void ResetBattery()  //배터리 초기화
    {
        if (drainRoutine != null) StopCoroutine(drainRoutine);

        currentBattery = maxBattery;
        OnBatteryChanged?.Invoke(currentBattery, GetCurrentStage());

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (nonDrainingScenes.Contains(currentSceneName))
        {
            Debug.Log($"[BatteryController] 현재 씬 '{currentSceneName}'에서는 배터리가 닳지 않습니다.");
            return;
        }

        drainRoutine = StartCoroutine(DrainBatteryRoutine());
    }

    private IEnumerator DrainBatteryRoutine() // 초당 배터리 감소
    {
        while (currentBattery > 0f)
        {
            currentBattery -= drainBattery * Time.deltaTime;
            currentBattery = Mathf.Max(0f, currentBattery);

            OnBatteryChanged?.Invoke(currentBattery, GetCurrentStage());

            yield return null; 
        }

        Debug.Log("[BatteryController] 배터리가 완전히 방전되었습니다.");
        OnBatteryEmpty?.Invoke(); //방전 이벤트
    }

    public void Recharge(float amount)  //배터리 충전
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (nonDrainingScenes.Contains(currentSceneName)) return;
        
        bool isAlreadyFull = (currentBattery >= maxBattery);

        if (!isAlreadyFull)
        {
            currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
            OnBatteryChanged?.Invoke(currentBattery, GetCurrentStage());
        }

        OnBatteryRecharged?.Invoke();

        if (drainRoutine != null) StopCoroutine(drainRoutine);
        drainRoutine = StartCoroutine(DrainBatteryRoutine());
    }

    public void Undestroyed()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //GameManager 씬전환 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReviveAndResetBattery() // 부활 시 배터리 초기화
    {
        if (drainRoutine != null) StopCoroutine(drainRoutine);

        currentBattery = maxBattery; // 배터리 100% 완충
        OnBatteryChanged?.Invoke(currentBattery, GetCurrentStage());
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!nonDrainingScenes.Contains(currentSceneName))
        {
            drainRoutine = StartCoroutine(DrainBatteryRoutine());
            Debug.Log("[BatteryController] 인게임 스테이지 부활: 배터리 감소 코루틴 재개.");
        }
    }

    private int GetCurrentStage()
    {
        if (currentBattery <= 0f) return 0;
        if (currentBattery < 20f) return 1;
        if (currentBattery < 40f) return 2;
        if (currentBattery < 60f) return 3;
        if (currentBattery < 80f) return 4; 
        
        return 5;
    }
}