using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image batteryImage;
    [SerializeField] private Sprite[] batterySprites;

    [Header("감소 경고 설정")]
    [SerializeField] private Color warningFlashColor = Color.red;
    [SerializeField] private float blinkBeforeSeconds = 2f;
    [SerializeField] private float blinkSpeed = 0.4f;

    [Header("획득 충전 설정")]
    [SerializeField] private Color rechargeFlashColor = Color.green;
    [SerializeField] private float rechargeBlinkDuration = 0.8f;
    [SerializeField] private float rechargeBlinkSpeed = 0.2f;

    private BatteryController controller;
    private Coroutine warningBlinkRoutine;
    private Coroutine effectBlinkRoutine;
    private float timePerStage = 10f;
    private Color originalColor;
    private int currentStageIndex = 0;
    private int previousStageIndex = 0;

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

    private void HandleBatteryChanged(float currentBattery, int stageIndex) //배터리 단계 변경 시 UI 업데이트
    {
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

    private void HandleBatteryRecharged()   //배터리 충전 시 효과
    {
        if (warningBlinkRoutine != null) StopCoroutine(warningBlinkRoutine);
        if (effectBlinkRoutine != null) StopCoroutine(effectBlinkRoutine);

        if (batteryImage != null) batteryImage.color = originalColor;

        effectBlinkRoutine = StartCoroutine(BlinkRechargeEffect());
    }

    private void ResetAllVisuals()  //모든 시각적 효과 초기화
    {
        if (warningBlinkRoutine != null) StopCoroutine(warningBlinkRoutine);
        if (effectBlinkRoutine != null) StopCoroutine(effectBlinkRoutine);

        if (batteryImage != null)
        {
            batteryImage.color = originalColor;
            batteryImage.enabled = true;
        }
    }

    private IEnumerator BlinkWarningSchedule()  //배터리 감소 경고 이펙트
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

    private IEnumerator BlinkRechargeEffect()   //배터리 충전 이펙트
    {
        float elapsed = 0f;
        bool isToggled = false;

        while (elapsed < rechargeBlinkDuration)
        {
            isToggled = !isToggled;

            if (currentStageIndex == 0 && previousStageIndex == 0)
            {
                batteryImage.color = isToggled ? rechargeFlashColor : originalColor;
            }
            else
            {
                batteryImage.color = originalColor;

                if (batterySprites != null)
                {
                    batteryImage.sprite = isToggled ? batterySprites[previousStageIndex] : batterySprites[currentStageIndex];
                }
            }

            yield return new WaitForSeconds(rechargeBlinkSpeed);
            elapsed += rechargeBlinkSpeed;
        }

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