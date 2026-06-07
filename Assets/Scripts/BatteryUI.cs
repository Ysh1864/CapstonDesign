using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image batteryImage;
    [SerializeField] private Sprite[] batterySprites;

    [Header("칸 감소 피드백 설정 (점멸기능)")]
    [SerializeField] private bool useDrainFeedback = true;      // 연출 사용 여부
    [SerializeField] private float drainBlinkDuration = 0.6f; // 총 깜빡임 시간
    [SerializeField] private float drainBlinkSpeed = 0.15f;   // 깜빡임 속도

    [Header("감소 경고 설정 (10f 이하)")]
    [SerializeField] private float warningThreshold = 10f; 
    [SerializeField] private Color warningFlashColor = Color.red;
    [SerializeField] private float blinkSpeed = 0.4f;

    [Header("획득 충전 설정 (완충)")]
    [SerializeField] private Color rechargeFlashColor = Color.green;
    [SerializeField] private float rechargeBlinkDuration = 0.8f;
    [SerializeField] private float rechargeBlinkSpeed = 0.2f;

    private BatteryController controller;
    private Coroutine currentEffectRoutine; 
    private Color originalColor;
    
    private int currentStageIndex = 5;
    private bool isWarningBlinking = false; 
    private bool isDrainBlinking = false;

    private void Awake()
    {
        controller = FindObjectOfType<BatteryController>();

        if (batteryImage != null)
        {
            originalColor = batteryImage.color;
        }
    }

    private void OnEnable()
    {
        BatteryController.OnBatteryChanged += UpdateBatteryUI;
        BatteryController.OnBatteryRecharged += PlayRechargeEffect;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryChanged -= UpdateBatteryUI;
        BatteryController.OnBatteryRecharged -= PlayRechargeEffect;
    }

    private void UpdateBatteryUI(float currentBattery, int stage)
    {
        int newStageIndex = Mathf.Clamp(stage, 0, batterySprites.Length - 1);

        if (useDrainFeedback && newStageIndex < currentStageIndex && 
            !isDrainBlinking && currentEffectRoutine == null && !isWarningBlinking)
        {
            currentEffectRoutine = StartCoroutine(DrainStageBlinkRoutine(currentStageIndex, newStageIndex));
        }

        currentStageIndex = newStageIndex;

        if (!isDrainBlinking && !isWarningBlinking && currentEffectRoutine == null)
        {
            if (batteryImage != null && batterySprites != null)
            {
                batteryImage.sprite = batterySprites[currentStageIndex];
                batteryImage.color = originalColor;
            }
        }

        if (currentBattery <= warningThreshold && currentBattery > 0f)
        {
            if (!isWarningBlinking && !isDrainBlinking)
            {
                if (currentEffectRoutine != null) StopCoroutine(currentEffectRoutine);
                currentEffectRoutine = StartCoroutine(RealtimeWarningBlinkRoutine());
            }
        }
        else
        {
            if (currentBattery <= 0f || currentBattery > warningThreshold)
            {
                if (isWarningBlinking)
                {
                    isWarningBlinking = false;
                    if (currentEffectRoutine != null) StopCoroutine(currentEffectRoutine);
                    currentEffectRoutine = null;
                    batteryColorReset();
                }
            }
        }
    }

    private IEnumerator DrainStageBlinkRoutine(int oldIndex, int newIndex)
    {
        isDrainBlinking = true;
        bool isToggled = false;
        float elapsed = 0f;

        while (elapsed < drainBlinkDuration)
        {
            isToggled = !isToggled;

            if (batteryImage != null && batterySprites != null)
            {
                batteryImage.sprite = isToggled ? batterySprites[newIndex] : batterySprites[oldIndex];
                batteryImage.color = originalColor; // 색상은 유지
            }

            yield return new WaitForSeconds(drainBlinkSpeed);
            elapsed += drainBlinkSpeed;
        }

        isDrainBlinking = false;
        currentEffectRoutine = null;
        if (batteryImage != null && batterySprites != null)
        {
            batteryImage.sprite = batterySprites[currentStageIndex];
        }
    }

    private IEnumerator RealtimeWarningBlinkRoutine()
    {
        isWarningBlinking = true;
        bool isToggled = false;

        int currentSpriteIdx = 1; 
        int nextSpriteIdx = 0;     

        while (isWarningBlinking)
        {
            isToggled = !isToggled;

            if (batteryImage != null && batterySprites != null)
            {
                batteryImage.sprite = isToggled ? batterySprites[nextSpriteIdx] : batterySprites[currentSpriteIdx];
                batteryImage.color = isToggled ? warningFlashColor : originalColor;
            }

            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void PlayRechargeEffect()
    {
        isWarningBlinking = false;
        isDrainBlinking = false;
        if (currentEffectRoutine != null) StopCoroutine(currentEffectRoutine);

        if (currentStageIndex == 5)
        {
            currentEffectRoutine = StartCoroutine(BlinkRechargeEffect());
        }
    }

    private IEnumerator BlinkRechargeEffect()
    {
        float elapsed = 0f;
        bool isToggled = false;

        while (elapsed < rechargeBlinkDuration)
        {
            isToggled = !isToggled;
            if (batteryImage != null)
            {
                batteryImage.color = isToggled ? rechargeFlashColor : originalColor;
            }

            yield return new WaitForSeconds(rechargeBlinkSpeed);
            elapsed += rechargeBlinkSpeed;
        }

        if (batteryImage != null)
        {
            batteryColorReset();
        }

        currentEffectRoutine = null;
    }

    private void batteryColorReset()
    {
        batteryImage.color = originalColor;
        if (batterySprites != null && currentStageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[currentStageIndex];
        }
    }
}