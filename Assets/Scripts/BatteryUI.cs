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
    private Coroutine currentEffectRoutine; 
    private Color originalColor;
    private int currentStageIndex = 0;

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
        if (currentEffectRoutine != null) StopCoroutine(currentEffectRoutine);
        if (batteryImage != null) batteryImage.color = originalColor;

        currentStageIndex = Mathf.Clamp(stage, 0, batterySprites.Length - 1);

        if (batteryImage != null && batterySprites != null && currentStageIndex < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[currentStageIndex];
        }

        if (controller != null && currentStageIndex < 5)
        {
            currentEffectRoutine = StartCoroutine(ReadyForWarningBlink());
        }
    }

    private IEnumerator ReadyForWarningBlink()
    {
        float timePerStage = controller.TimePerStage;
        float waitTime = timePerStage - blinkBeforeSeconds;

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        float elapsed = 0f;
        bool isToggled = false;

        int currentSpriteIdx = currentStageIndex;
        int nextSpriteIdx = Mathf.Clamp(currentStageIndex + 1, 0, batterySprites.Length - 1);

        while (elapsed < blinkBeforeSeconds)
        {
            isToggled = !isToggled;
            
            if (batteryImage != null && batterySprites != null)
            {
                batteryImage.sprite = isToggled ? batterySprites[nextSpriteIdx] : batterySprites[currentSpriteIdx];
                
                if (currentStageIndex == 4)
                {
                    batteryImage.color = isToggled ? warningFlashColor : originalColor;
                }
                else
                {
                    batteryImage.color = originalColor;
                }
            }

            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        if (batteryImage != null && batterySprites != null)
        {
            batteryImage.color = originalColor;
            batteryImage.sprite = batterySprites[nextSpriteIdx];
        }
    }

    private void PlayRechargeEffect()
    {
        if (currentEffectRoutine != null) StopCoroutine(currentEffectRoutine);

        if (currentStageIndex == 0)
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
            batteryImage.color = originalColor;
            if (batterySprites != null && currentStageIndex < batterySprites.Length)
            {
                batteryImage.sprite = batterySprites[currentStageIndex];
            }
        }

        if (currentStageIndex < 5)
        {
            currentEffectRoutine = StartCoroutine(ReadyForWarningBlink());
        }
    }
}