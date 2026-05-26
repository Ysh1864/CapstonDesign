using UnityEngine;

[CreateAssetMenu(fileName = "NewToolData", menuName = "Tool/ToolData")]
public class ToolData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("도구 이름 (UI 표시용)")]
    public string toolName = "도구";

    [Tooltip("도구 설명 (UI 툴팁용)")]
    [TextArea(2, 4)]
    public string description = "";

    [Header("비주얼")]
    [Tooltip("인벤토리 슬롯에 표시할 아이콘")]
    public Sprite icon;
}