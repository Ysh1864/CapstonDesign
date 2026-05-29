using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수 추가!

public class BatteryController : MonoBehaviour
{
    public static event Action<float, int> OnBatteryChanged;

    [Header("배터리 설정")]
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float timePerStage = 10f;

    [Header("배터리가 닳지 않는 씬 목록")]
    [SerializeField] private List<string> nonDrainingScenes = new List<string> { "MainMenu", "SafeZone" };
    // ↑ 여기에 인스펙터나 코드로 배터리가 멈춰야 할 씬 이름을 등록.

    private float currentBattery;
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

        // 현재 로드된 씬 이름 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 배터리 소모 예외 씬 확인
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
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
        currentStage = 5 - (int)(currentBattery / 20f);
        OnBatteryChanged?.Invoke(currentBattery, currentStage);
    }
}