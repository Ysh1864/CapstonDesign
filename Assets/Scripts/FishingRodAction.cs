using UnityEngine;

public class FishingRodAction : ToolAction
{
    [Header("낚싯대 설정")]
    [SerializeField] private GameObject hookPrefab;      // 발사할 낚시찌 프리팹
    [SerializeField] private float throwForceX = 8f;     // 수평 발사 힘
    [SerializeField] private float throwForceY = 6f;     // 수직 포물선 힘

    private GameObject currentHookGo;                     // 현재 생성된 찌 오브젝트

    public override void ExecuteAction(PlayerMovement player)
    {
        // 이미 던져진 찌가 있는 경우
        if (currentHookGo != null)
        {
            if (currentHookGo.TryGetComponent(out FishingHookProjectile hook))
            {
                // 찌에게 플레이어 쪽으로 돌아오며 물체를 당기라고 명령
                hook.PullBack();
            }
            return;
        }

        // 찌가 없는 경우 새로운 찌를 포물선으로 던짐
        Vector3 spawnPos = player.transform.position + new Vector3(player.FacingDirection * 0.5f, 0.5f, 0f);
        currentHookGo = Instantiate(hookPrefab, spawnPos, Quaternion.identity);

        if (currentHookGo.TryGetComponent(out FishingHookProjectile newHook))
        {
            // 초기 발사 속도 계산 (바라보는 방향 반영)
            Vector2 launchVelocity = new Vector2(player.FacingDirection * throwForceX, throwForceY);
            newHook.Launch(launchVelocity, player.transform, this);
        }
    }

    // 찌가 프리팹 초기화
    public void ClearHook()
    {
        currentHookGo = null;
    }
}