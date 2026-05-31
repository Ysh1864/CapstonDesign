using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image batteryImage;
    [SerializeField] private Sprite[] batterySprites;

    [Header("감소 경고 점멸 설정")]
    [SerializeField] private Color warningFlashColor = Color.red;    // [수정] 감소 직전 깜빡일 색상 (빨간색)
    [SerializeField] private float blinkBeforeSeconds = 2f;
    [SerializeField] private float blinkSpeed = 0.2f;

    [Header("획득 충전 점멸 설정")]
    [SerializeField] private Color rechargeFlashColor = Color.green;
    [SerializeField] private float rechargeBlinkDuration = 0.8f;
    [SerializeField] private float rechargeBlinkSpeed = 0.1f;

    private BatteryController controller;
    private Coroutine warningBlinkRoutine;
    private Coroutine effectBlinkRoutine;
    private float timePerStage = 10f;
    private Color originalColor;

    private void Awake()
    {
        controller = FindObjectOfType<BatteryController>();
        if (controller != null)
        {
            timePerStage = controller.TimePerStage;
        }

        if (batteryImage != null)
        {
            originalColor = batteryImage.color;
        }
    }

    private void OnEnable()
    {
        BatteryController.OnBatteryChanged += HandleBatteryChanged;
        BatteryController.OnBatteryRecharged += HandleBatteryRecharged;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryChanged -= HandleBatteryChanged;
        BatteryController.OnBatteryRecharged -= HandleBatteryRecharged;
    }

    private void HandleBatteryChanged(float currentBattery, int stageIndex)
    {
        // 수치가 바뀌면 돌고 있던 모든 연출을 초기화하고 원래 색상/상태로 복구
        ResetAllVisuals();

        if (batterySprites != null && stageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[stageIndex];
        }

        // 방전 상태(0%)가 아니라면 다음 단계 소모 경고 타이머 예약 가동
        if (stageIndex < 5)
        {
            warningBlinkRoutine = StartCoroutine(BlinkWarningSchedule());
        }
    }

    private void HandleBatteryRecharged()
    {
        ResetAllVisuals();

        effectBlinkRoutine = StartCoroutine(BlinkRechargeEffect());

        if (controller != null)
        {
            warningBlinkRoutine = StartCoroutine(BlinkWarningSchedule());
        }
    }

    // [수정] 경고/충전 이펙트가 겹치거나 잔상이 남지 않도록 색상과 활성화태를 깔끔하게 밀어주는 메서드
    private void ResetAllVisuals()
    {
        if (warningBlinkRoutine != null) StopCoroutine(warningBlinkRoutine);
        if (effectBlinkRoutine != null) StopCoroutine(effectBlinkRoutine);

        if (batteryImage != null)
        {
            batteryImage.color = originalColor;
            batteryImage.enabled = true;
        }
    }

    // [수정] 다음 이미지 단계로 넘어가기 전 빨간색으로 번쩍이는 루틴
    private IEnumerator BlinkWarningSchedule()
    {
        float waitTime = timePerStage - blinkBeforeSeconds;
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        float elapsed = 0f;
        bool isColorToggled = false;

        // 지정된 경고 시간(예: 2초) 동안 실행
        while (elapsed < blinkBeforeSeconds)
        {
            isColorToggled = !isColorToggled;

            // 이미지를 끄는 대신, 설정된 경고 색상(빨간색)과 원래 색상을 번갈아 적용합니다.
            batteryImage.color = isColorToggled ? warningFlashColor : originalColor;

            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        // 루틴이 끝나면 원래 색상으로 되돌림 (이후 HandleBatteryChanged에서 이미지가 바뀝니다)
        batteryImage.color = originalColor;
    }

    // 배터리 아이템을 먹었을 때 초록색으로 번쩍이는 루틴
    private IEnumerator BlinkRechargeEffect()
    {
        float elapsed = 0f;
        bool isColorToggled = false;

        while (elapsed < rechargeBlinkDuration)
        {
            isColorToggled = !isColorToggled;
            batteryImage.color = isColorToggled ? rechargeFlashColor : originalColor;

            yield return new WaitForSeconds(rechargeBlinkSpeed);
            elapsed += rechargeBlinkSpeed;
        }

        batteryImage.color = originalColor;
    }
}