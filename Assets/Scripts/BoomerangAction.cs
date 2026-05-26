using UnityEngine;

public class BoomerangAction : ToolAction
{
    [Header("부메랑 투사체 설정")]
    [SerializeField] private GameObject boomerangProjectilePrefab; // 날아갈 투사체 프리팹
    [SerializeField] private float throwSpeed = 12f;                 // 던지는 속도

    public override void ExecuteAction(PlayerMovement player)
    {
        // 이미 날아가 있는 부메랑이 있다면 던지지 못함
        if (GameObject.FindObjectOfType<BoomerangProjectile>() != null) return;

        // 플레이어가 바라보는 방향(FacingDirection: 1 또는 -1) 앞쪽에 투사체 스폰
        Vector3 spawnPos = player.transform.position + new Vector3(player.FacingDirection * 0.5f, 0.5f, 0f);
        GameObject projectileGo = Instantiate(boomerangProjectilePrefab, spawnPos, Quaternion.identity);

        // 투사체에게 날아갈 방향 벡터와 추적할 플레이어(Target)의 위치 전달
        if (projectileGo.TryGetComponent(out BoomerangProjectile projectile))
        {
            Vector2 launchDirection = new Vector2(player.FacingDirection, 0f).normalized;
            projectile.Launch(launchDirection * throwSpeed, player.transform);
        }
    }
}