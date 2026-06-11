using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternBatteryLight : MonoBehaviour
{
    [Header("Battery")]
    public BatteryController batteryController;

    [Header("Light")]
    public Light2D lanternLight;

    [Header("Radius")]
    public float maxRadius = 8f;
    public float minRadius = 1f;

    [Header("Intensity")]
    public float maxIntensity = 2f;
    public float minIntensity = 0.2f;

    void Start()
    {
        if (lanternLight == null)
            lanternLight = GetComponent<Light2D>();

        if (batteryController == null)
            batteryController = FindObjectOfType<BatteryController>();
    }

    void Update()
    {
        if (batteryController == null || lanternLight == null)
            return;

        float batteryPercent =
            batteryController.CurrentBattery /
            batteryController.MaxBattery;

        lanternLight.pointLightOuterRadius =
            Mathf.Lerp(minRadius, maxRadius, batteryPercent);

        lanternLight.intensity =
            Mathf.Lerp(minIntensity, maxIntensity, batteryPercent);
    }
}