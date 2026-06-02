using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image batteryImage;
    [SerializeField] private Sprite[] batterySprites;       // 배터리 이미지 5개 (0: 5칸 ~ 4: 1칸)

    [Header("감소 경고 설정")]
    [SerializeField] private Color warningFlashColor = Color.red;
    [SerializeField] private float blinkBeforeSeconds = 2f;
    [SerializeField] private float blinkSpeed = 0.4f;

    [Header("획득 충전 설정")]
    [SerializeField] private Color rechargeFlashColor = Color.green; // 5->5 상태에서만 쓰일 초록색
    [SerializeField] private float rechargeBlinkDuration = 0.8f;
    [SerializeField] private float rechargeBlinkSpeed = 0.2f;

    private BatteryController controller;
    private Coroutine warningBlinkRoutine;
    private Coroutine effectBlinkRoutine;
    private float timePerStage = 10f;
    private Color originalColor;
    private int currentStageIndex = 0;      // 현재 배터리 단계 (0: 5칸, 1: 4칸, 2: 3칸, 3: 2칸, 4: 1칸)
    private int previousStageIndex = 0;     // 충전 직전의 배터리 단계를 기억할 변수

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

    // 1. 배터리 데이터(수치)가 실제로 변했을 때 호출
    private void HandleBatteryChanged(float currentBattery, int stageIndex)
    {
        // [수정] 충전 시 직전 인덱스 연산을 정확히 하기 위해, 값이 바뀌기 전 상태를 고스란히 기억해둡니다.
        previousStageIndex = currentStageIndex;

        ResetAllVisuals();
        currentStageIndex = stageIndex;

        if (batterySprites != null && stageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[stageIndex];
        }

        if (stageIndex < 5)
        {
            warningBlinkRoutine = StartCoroutine(BlinkWarningSchedule());
        }
    }

    // 2. 플레이어가 배터리 아이템을 획득했을 때 연출 발동
    private void HandleBatteryRecharged()
    {
        if (warningBlinkRoutine != null) StopCoroutine(warningBlinkRoutine);
        if (effectBlinkRoutine != null) StopCoroutine(effectBlinkRoutine);

        if (batteryImage != null) batteryImage.color = originalColor;

        effectBlinkRoutine = StartCoroutine(BlinkRechargeEffect());
    }

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

    // [소모 경고] 10초가 지나기 전 줬던 이미지 스왑 루틴 (기존 유지)
    private IEnumerator BlinkWarningSchedule()
    {
        float waitTime = timePerStage - blinkBeforeSeconds;
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        float elapsed = 0f;
        bool isToggled = false;

        while (elapsed < blinkBeforeSeconds)
        {
            isToggled = !isToggled;

            if (currentStageIndex == 4)
            {
                batteryImage.color = isToggled ? warningFlashColor : originalColor;
            }
            else
            {
                batteryImage.color = originalColor;

                if (batterySprites != null && currentStageIndex + 1 < batterySprites.Length)
                {
                    batteryImage.sprite = isToggled ? batterySprites[currentStageIndex + 1] : batterySprites[currentStageIndex];
                }
            }

            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        batteryImage.color = originalColor;
    }

    // [오류 수정] 획득(충전) 시 직전 칸수와 바뀐 칸수 사이에서만 정확히 교차하도록 수정된 루틴
    private IEnumerator BlinkRechargeEffect()
    {
        float elapsed = 0f;
        bool isToggled = false;

        while (elapsed < rechargeBlinkDuration)
        {
            isToggled = !isToggled;

            // [5->5 예외] 이미 만땅(5칸, 인덱스 0)인데 또 먹었을 때
            if (currentStageIndex == 0 && previousStageIndex == 0)
            {
                batteryImage.color = isToggled ? rechargeFlashColor : originalColor;
            }
            else
            {
                batteryImage.color = originalColor;

                // [정밀 수정] 이미 값이 바뀐 상태(currentStageIndex)와 바뀌기 전 상태(previousStageIndex)를 번갈아 노출합니다.
                if (batterySprites != null)
                {
                    batteryImage.sprite = isToggled ? batterySprites[previousStageIndex] : batterySprites[currentStageIndex];
                }
            }

            yield return new WaitForSeconds(rechargeBlinkSpeed);
            elapsed += rechargeBlinkSpeed;
        }

        // 연출 마감 후 최종 복구 및 소모 예약
        batteryImage.color = originalColor;
        if (batterySprites != null && currentStageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[currentStageIndex];
        }

        if (controller != null)
        {
            if (warningBlinkRoutine != null) StopCoroutine(warningBlinkRoutine);
            warningBlinkRoutine = StartCoroutine(BlinkWarningSchedule());
        }
    }
}