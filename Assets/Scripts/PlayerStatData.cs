using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Player/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
    [Header("이동")]
    [Tooltip("좌우 이동 속도 (units/sec)")]
    public float moveSpeed = 5f;

    [Tooltip("이동 가속도 (값이 클수록 즉각 반응)")]
    public float acceleration = 20f;

    [Tooltip("이동 감속도 (값이 클수록 빠르게 정지)")]
    public float deceleration = 25f;

    [Header("점프")]
    [Tooltip("점프 초기 속도")]
    public float jumpForce = 12f;

    [Tooltip("점프 버튼을 누르는 동안 중력을 줄이는 배율 (1 = 효과 없음)")]
    [Range(0.1f, 1f)]
    public float jumpHoldGravityScale = 0.5f;

    [Tooltip("점프 홀드 최대 지속 시간 (초)")]
    public float jumpHoldDuration = 0.2f;

    [Tooltip("기본 중력 배율")]
    public float gravityScale = 3f;

    [Tooltip("낙하 시 중력 배율 (더 빠르게 떨어짐)")]
    public float fallGravityScale = 4.5f;

    [Header("코요테 타임 / 점프 버퍼")]
    [Tooltip("절벽 끝에서 벗어난 후에도 점프 가능한 유예 시간 (초)")]
    public float coyoteTime = 0.12f;

    [Tooltip("착지 직전 점프 입력을 미리 받아두는 버퍼 시간 (초)")]
    public float jumpBufferTime = 0.12f;

    [Header("지면 감지")]
    [Tooltip("Ground 레이어 마스크")]
    public LayerMask groundLayer;

    [Tooltip("GroundCheck Transform에서 쏘는 원형 감지 반경")]
    public float groundCheckRadius = 0.15f;
}