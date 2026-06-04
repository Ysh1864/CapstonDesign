using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BatteryController : MonoBehaviour
{
    public static event Action<float, int> OnBatteryChanged;
    public static event Action OnBatteryRecharged; // 충전 연출용 이벤트

    [Header("배터리 설정")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float timePerStage = 10f;

    [Header("배터리가 닳지 않는 씬 목록")]
    [SerializeField] private List<string> nonDrainingScenes = new List<string> { "MainMenu", "SafeZone" };

    float currentBattery;   //현재 배터리 잔량
    private int currentStage = 0;
    private Coroutine drainRoutine;

    public float TimePerStage => timePerStage;

    private void Start()
    {
        ResetBattery();
    }

    public void ResetBattery()
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

    private IEnumerator DrainBatteryRoutine()
    {
        while (currentStage < 5)
        {
            yield return new WaitForSeconds(timePerStage);

            currentStage++;
            currentBattery = Mathf.Max(0f, maxBattery - (currentStage * 20f));

            OnBatteryChanged?.Invoke(currentBattery, currentStage);
        }

        Debug.Log("[BatteryController] 배터리가 완전히 방전되었습니다.");
    }

    public void Recharge(float amount)
    {
        // [수정] 배터리가 이미 풀(100)이더라도 먹는 연출을 주기 위해 리턴 조건 제거 및 분기 처리
        bool isAlreadyFull = (currentBattery >= maxBattery);

        if (!isAlreadyFull)
        {
            currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
            currentStage = 5 - (int)(currentBattery / 20f);

            // 배터리 수치/이미지 변경 이벤트 전송
            OnBatteryChanged?.Invoke(currentBattery, currentStage);
        }

        // [수정] 배터리가 꽉 차있었든 아니든, 배터리 아이템을 작동시켰으므로 충전 점멸 신호는 무조건 전송!
        OnBatteryRecharged?.Invoke();

        // 소모 타이머 리셋 처리 (기존 유지)
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!nonDrainingScenes.Contains(currentSceneName))
        {
            if (drainRoutine != null) StopCoroutine(drainRoutine);
            drainRoutine = StartCoroutine(DrainBatteryRoutine());
        }
    }
}