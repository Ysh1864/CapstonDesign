using UnityEngine;

// 모든 도구 기능들의 부모가 될 추상 클래스
public abstract class ToolAction : MonoBehaviour
{
    // 이 도구를 사용할 때 실행되는 로직 (자식 클래스가 직접 구현)
    public abstract void ExecuteAction(PlayerMovement player);
}