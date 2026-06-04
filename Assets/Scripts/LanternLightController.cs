using UnityEngine;
//using UnityEngine.Rendering.Universal;

//[RequireComponent(typeof(Light2D))]
public class LanternLightController : MonoBehaviour
{
    [Header("Battery Source")]
    [SerializeField] private BatteryController batteryController;

    [Header("Light Size By Battery")]
    [SerializeField] private float maxOuterRadius = 7f;
    [SerializeField] private float minOuterRadius = 1.2f;
    [SerializeField] private float maxInnerRadius = 2.4f;
    [SerializeField] private float minInnerRadius = 0.3f;

    [Header("Light Intensity")]
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float minIntensity = 0.2f;

    [Header("Low Battery Flicker")]
    [SerializeField] private bool useFlickerWhenLow = true;
    [SerializeField] private float lowBatteryThreshold = 20f;
    [SerializeField] private float flickerAmount = 0.08f;
    [SerializeField] private float flickerSpeed = 18f;

    //private Light2D lanternLight;
    private float currentBattery = 100f;
    private float baseIntensity;

    private void Awake()
    {
        //lanternLight = GetComponent<Light2D>();
        //baseIntensity = lanternLight.intensity;

        if (batteryController == null)
            batteryController = FindObjectOfType<BatteryController>();
    }

    private void OnEnable()
    {
        BatteryController.OnBatteryChanged += HandleBatteryChanged;
    }

    private void OnDisable()
    {
        BatteryController.OnBatteryChanged -= HandleBatteryChanged;
    }

    private void Start()
    {
        ApplyBatteryToLight(currentBattery);
    }

    private void Update()
    {
        if (!useFlickerWhenLow) return;
        if (currentBattery > lowBatteryThreshold) return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
       // lanternLight.intensity = baseIntensity + ((noise - 0.5f) * flickerAmount);
    }

    private void HandleBatteryChanged(float batteryValue, int stageIndex)
    {
        currentBattery = Mathf.Clamp(batteryValue, 0f, 100f);
        ApplyBatteryToLight(currentBattery);
    }

    private void ApplyBatteryToLight(float batteryValue)
    {
        float normalized = batteryValue / 100f;

        //lanternLight.pointLightOuterRadius = Mathf.Lerp(minOuterRadius, maxOuterRadius, normalized);
       // lanternLight.pointLightInnerRadius = Mathf.Lerp(minInnerRadius, maxInnerRadius, normalized);
        baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, normalized);
       // lanternLight.intensity = baseIntensity;
    }
}
