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
    [SerializeField] private float timePerStage = 10f;  // timePerStage초 마다 20(한 칸) 감소


    [Header("배터리가 닳지 않는 씬 목록")]
    [SerializeField] private List<string> nonDrainingScenes = new List<string> { "MainMenu", "SafeZone" };
    private float currentBattery;   //현재 배터리 잔량

    private int currentStage = 0;   //배터리 단계
    private Coroutine drainRoutine;

    public float TimePerStage => timePerStage;

    private void Awake()
    {
        // 최우선 순위 방어: 내가 진짜 첫 번째 인스턴스가 아니라면 이벤트 등록이고 뭐고 아무것도 못하게 즉시 완전히 파괴합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Undestroyed();
    }

    private void Start()
    {
        // 중복 복제본 차단
        if (Instance != this) return;

        ResetBattery();
    }

    private void OnEnable()
    {
        // 중복 복제본은 씬 로드 이벤트를 아예 수신하지 못하도록 완벽하게 차단합니다.
        if (Instance != null && Instance != this) return;

        SceneManager.sceneLoaded += OnSceneLoaded;  //씬이 로드될 때마다 배터리 유지
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void ResetBattery()  //배터리 초기화
    {
        if (drainRoutine != null) StopCoroutine(drainRoutine);

        currentBattery = maxBattery;
        currentStage = 0;

        OnBatteryChanged?.Invoke(currentBattery, currentStage);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (nonDrainingScenes.Contains(currentSceneName))
        {
            Debug.Log($"[BatteryController] 현재 씬 '{currentSceneName}'에서는 배터리가 닳지 않습니다.");
            return;
        }

        drainRoutine = StartCoroutine(DrainBatteryRoutine());
    }

    private IEnumerator DrainBatteryRoutine() //배터리 감소
    {
        while (currentStage < 5)
        {
            yield return new WaitForSeconds(timePerStage);

            currentStage++;
            currentBattery = Mathf.Max(0f, maxBattery - (currentStage * 20f));

            OnBatteryChanged?.Invoke(currentBattery, currentStage);
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
            currentStage = 5 - (int)(currentBattery / 20f);

            OnBatteryChanged?.Invoke(currentBattery, currentStage);
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
        currentStage = 0;
        OnBatteryChanged?.Invoke(currentBattery, currentStage);
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!nonDrainingScenes.Contains(currentSceneName))
        {
            drainRoutine = StartCoroutine(DrainBatteryRoutine());
            Debug.Log("[BatteryController] 인게임 스테이지 부활: 배터리 감소 코루틴 재개.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 진짜 인스턴스만 이 핵심 로직을 실행할 수 있습니다.
        if (Instance != this) return;

        if (drainRoutine != null) StopCoroutine(drainRoutine);  

        // 요청하신 가장 직관적인 조건문 형태 유지 및 반영
        if (nonDrainingScenes.Contains(scene.name)) 
        {
            // 안전 구역인 경우만 배터리 100% 초기화
            currentBattery = maxBattery;
            currentStage = 0;
            Debug.Log($"[BatteryController] 안전 구역 '{scene.name}' 진입: 배터리 100% 고정.");
        }
        else
        {
            // 안전 구역이 아니면 절대 데이터를 덮어쓰지 않고 기존 상태 완벽 유지!
            Debug.Log($"[BatteryController] 일반 구역 '{scene.name}' 진입: 기존 배터리 상태({currentBattery} / {currentStage}단계) 유지.");
        }

        OnBatteryChanged?.Invoke(currentBattery, currentStage);

        // 안전 구역이 아닐 때만 타이머를 재개합니다.
        if (!nonDrainingScenes.Contains(scene.name))
        {
            drainRoutine = StartCoroutine(DrainBatteryRoutine());   
        }
    }
}