using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image batteryImage;            // 배터리 UI Image
    [SerializeField] private Sprite[] batterySprites;       // 배터리 이미지 6개 (0: 100% ~ 5: 0%)

    [Header("깜빡임 연출 설정")]
    [SerializeField] private float blinkBeforeSeconds = 2f; // 단계가 바뀌기 몇 초 전부터 깜빡일 것인가?
    [SerializeField] private float blinkSpeed = 0.2f;       // 깜빡임 속도

    private BatteryController controller;
    private Coroutine blinkRoutine;
    private float timePerStage = 10f;

    private void Awake()
    {
        controller = FindObjectOfType<BatteryController>();
        if (controller != null)
        {
            timePerStage = controller.TimePerStage;
        }
    }

    private void OnEnable()
    {
        // 배터리 변경 이벤트 구독
        BatteryController.OnBatteryChanged += HandleBatteryChanged;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위한 구독 해제
        BatteryController.OnBatteryChanged -= HandleBatteryChanged;
    }

    // 배터리 데이터가 바뀌면 호출되는 메서드
    private void HandleBatteryChanged(float currentBattery, int stageIndex)
    {
        // 기존에 돌고 있던 깜빡임 연출이 있다면 안전하게 중지 및 복구
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            batteryImage.enabled = true;
        }

        // 이미지 교체
        if (batterySprites != null && stageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[stageIndex];
        }

        // 만약 완전히 방전된 상태(인덱스 5)가 아니라면, 다음 단계로 가기 전 깜빡임 타이머 예약 호출
        if (stageIndex < 5)
        {
            blinkRoutine = StartCoroutine(BlinkWarningSchedule());
        }
    }

    // 다음 이미지 단계로 넘어가기 전 대기했다가 깜빡임을 발동시키는 루틴
    private IEnumerator BlinkWarningSchedule()
    {
        // 10초 중 마지막 2초 전에 깜빡이려면 앞의 8초를 먼저 기다림
        float waitTime = timePerStage - blinkBeforeSeconds;
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        // 경고 깜빡임 연출 시작
        float elapsed = 0f;
        bool isVisible = true;

        while (elapsed < blinkBeforeSeconds)
        {
            isVisible = !isVisible;
            batteryImage.enabled = isVisible;

            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        // 다음 단계 이미지로 교체되기 직전 상태 복구
        batteryImage.enabled = true;
    }
}