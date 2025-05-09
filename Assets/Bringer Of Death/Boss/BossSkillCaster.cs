using System.Collections;
using UnityEngine;

public class BossSkillCaster : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject aoeSpellPrefab;
    [SerializeField] private GameObject summonEnemyPrefab;

    private BossAI bossAI;
    private Animator animator;
    private enum CastType { Spell, AOE, Summon }
    private CastType pendingCastType;
    private bool isCasting;

    private void Awake()
    {
        bossAI = GetComponent<BossAI>();
        animator = GetComponent<Animator>();
    }

    public bool IsCasting => isCasting;

    public void RequestCastSpell()
    {
        if (isCasting) return;
        pendingCastType = CastType.Spell;
        StartCast();
    }

    public void RequestCastAOE()
    {
        if (isCasting) return;
        pendingCastType = CastType.AOE;
        StartCast();
    }

    public void RequestCastSummon()
    {
        if (isCasting) return;
        pendingCastType = CastType.Summon;
        StartCast();
    }

    private void StartCast()
    {
        isCasting = true;
        GetComponent<BossPathfinding>().StopMoving();
        animator.SetTrigger("Cast");
    }

    // Called by Animation Event
    public void OnCastEvent()
    {
        switch (pendingCastType)
        {
            case CastType.Spell:
                StartCoroutine(DoSpellLogic());
                break;
            case CastType.AOE:
                DoAOELogic();
                break;
            case CastType.Summon:
                DoSummonLogic();
                break;
        }

        isCasting = false;
    }

    private IEnumerator DoSpellLogic()
    {
        int type = Random.Range(0, 2);

        if (type == 0)
        {
            float[] angles = { -15f, 0f, 15f };
            foreach (float angle in angles)
            {
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                Vector2 rotated = Quaternion.Euler(0, 0, angle) * dir;
                SpawnBullet(rotated);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                float randomAngle = Random.Range(-25f, 25f);
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                Vector2 rotated = Quaternion.Euler(0, 0, randomAngle) * dir;
                SpawnBullet(rotated);
                yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
            }
        }
    }

    private void DoAOELogic()
    {
        Vector2 pos = Player.Instance.transform.position;
        Instantiate(aoeSpellPrefab, pos, Quaternion.identity);
    }

    private void DoSummonLogic()
    {
        Vector2 offset1 = new Vector2(1.5f, 0);
        Vector2 offset2 = new Vector2(-1.5f, 0);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset1, Quaternion.identity);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset2, Quaternion.identity);
    }

    private void SpawnBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.right = direction;
    }
}
