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
    public static event Action OnBatteryEmpty; // 방전 시 발생할 이벤트

    [Header("배터리 설정")]
    [SerializeField] private float maxBattery = 100f;   //최대 배터리 양
    [SerializeField] private float timePerStage = 10f;  // timePerStage초 마다 20(한 칸) 감소

    [Header("배터리가 닳지 않는 씬 목록")]
    [SerializeField] private List<string> nonDrainingScenes = new List<string> { "MainMenu", "SafeZone" };

    float currentBattery;   //현재 배터리 잔량
    private int currentStage = 0;   //배터리 단계
    private Coroutine drainRoutine;

    public float TimePerStage => timePerStage;

    private void Awake()
    {
        Undestroyed();
    }
    private void Start()
    {
        ResetBattery();
    }

    private void OnEnable()
    {
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (drainRoutine != null) StopCoroutine(drainRoutine);  //씬 전환 시 코루틴 중지(배터리 감소)

        if (nonDrainingScenes.Contains(scene.name)) //안전 구역 확인 및 배터리 100% 고정
        {
            currentBattery = maxBattery;
            currentStage = 0;
            OnBatteryChanged?.Invoke(currentBattery, currentStage);
            Debug.Log($"[BatteryController] 안전 구역 '{scene.name}' 진입: 배터리 100% 고정.");
            return; 
        }
        drainRoutine = StartCoroutine(DrainBatteryRoutine());   //안전 구역 아니라면 배터리 감소 재개
    }
}