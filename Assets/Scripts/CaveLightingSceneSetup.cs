using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class CaveLightingSceneSetup : MonoBehaviour
{
    [Header("Dark Cave Preview")]
    [SerializeField] private bool createGlobalLight = true;
    [SerializeField, Range(0f, 1f)] private float globalLightIntensity = 0.15f;
    [SerializeField] private bool convertSpriteRenderersToLitMaterial = true;
    [SerializeField] private string globalLightObjectName = "GlobalLight2D_DarkCave";

    private Material runtimeLitMaterial;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        ApplyLightingSetup();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyLightingSetup();
    }

    private void ApplyLightingSetup()
    {
        if (convertSpriteRenderersToLitMaterial)
            ConvertSpritesToLitMaterial();

        if (createGlobalLight)
            EnsureGlobalLight();
    }

    private void ConvertSpritesToLitMaterial()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
        if (litShader == null)
        {
            Debug.LogWarning("[CaveLightingSceneSetup] Sprite-Lit-Default 셰이더를 찾지 못했습니다. 2D Renderer 설정을 확인하세요.");
            return;
        }

        if (runtimeLitMaterial == null)
            runtimeLitMaterial = new Material(litShader) { name = "Runtime_SpriteLitMaterial" };

        foreach (SpriteRenderer renderer in FindObjectsOfType<SpriteRenderer>())
        {
            if (renderer == null) continue;

            // 플레이어까지 Sprite-Lit으로 바꾸면 Global Light가 낮을 때 캐릭터가 거의 안 보일 수 있습니다.
            // 그래서 맵/배경만 어둠의 영향을 받게 하고, Player 스프라이트는 기본 표시를 유지합니다.
            if (renderer.GetComponentInParent<PlayerMovement>() != null) continue;
            if (renderer.GetComponentInParent<Canvas>() != null) continue;

            renderer.sharedMaterial = runtimeLitMaterial;
        }
    }

    private void EnsureGlobalLight()
    {
        Type lightType = FindLight2DType();
        if (lightType == null)
        {
            Debug.LogWarning("[CaveLightingSceneSetup] Light2D 타입을 찾지 못했습니다. Universal RP 패키지와 2D Renderer 설정을 확인하세요.");
            return;
        }

        GameObject globalLightObject = GameObject.Find(globalLightObjectName);
        if (globalLightObject == null)
            globalLightObject = new GameObject(globalLightObjectName);

        Component light2D = globalLightObject.GetComponent(lightType);
        if (light2D == null)
            light2D = globalLightObject.AddComponent(lightType);

        SetProperty(light2D, "lightType", 0); // Global Light
        SetProperty(light2D, "intensity", globalLightIntensity);
        SetProperty(light2D, "color", Color.white);
    }

    private Type FindLight2DType()
    {
        Type type = Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        if (type != null) return type;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType("UnityEngine.Rendering.Universal.Light2D");
            if (type != null) return type;
        }

        return null;
    }

    private void SetProperty(Component component, string propertyName, object value)
    {
        if (component == null) return;
        PropertyInfo property = component.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            object convertedValue = value;
            if (property.PropertyType.IsEnum && value is int intValue)
                convertedValue = Enum.ToObject(property.PropertyType, intValue);
            property.SetValue(component, convertedValue);
            return;
        }

        FieldInfo field = component.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            object convertedValue = value;
            if (field.FieldType.IsEnum && value is int intValue)
                convertedValue = Enum.ToObject(field.FieldType, intValue);
            field.SetValue(component, convertedValue);
        }
    }
}
