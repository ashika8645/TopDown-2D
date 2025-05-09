using UnityEngine;
using System.Collections;

public class BossSkillActions : MonoBehaviour
{
    [Header("Skill Prefabs")]
    public GameObject bulletPrefab, aoeSpellPrefab, summonEnemyPrefab;

    [Header("Ranges")]
    public float attackRange = 8f;
    private Animator animator;
    private BossPathfinding pathfinding;
    private bool isBusy = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pathfinding = GetComponent<BossPathfinding>();
    }

    public float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, Player.Instance.transform.position);
    }
    public bool ShootSpell()
    {
        if (isBusy) return false;
        StartCoroutine(ShootRoutine());
        return true;
    }

    private IEnumerator ShootRoutine()
    {
        isBusy = true;
        pathfinding.StopMoving();
        animator.SetTrigger("Cast");

        yield return WaitForAnimation("Cast");

        int type = Random.Range(0, 2);

        if (type == 0)
        {
            float[] angles = { -15f, 0f, 15f };
            foreach (float angle in angles)
            {
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                Vector2 rotated = Quaternion.Euler(0, 0, angle) * dir;

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.right = rotated;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                float randomAngle = Random.Range(-25f, 25f);
                Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
                Vector2 rotated = Quaternion.Euler(0, 0, randomAngle) * dir;

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.right = rotated;

                yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
            }
        }

        yield return new WaitForSeconds(0.5f);
        isBusy = false;
    }

    public bool CastAOE()
    {
        if (isBusy) return false;
        StartCoroutine(AOERoutine());
        return true;
    }

    private IEnumerator AOERoutine()
    {
        isBusy = true;
        pathfinding.StopMoving();
        animator.SetTrigger("Cast");

        yield return WaitForAnimation("Cast");

        Instantiate(aoeSpellPrefab, Player.Instance.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        isBusy = false;
    }

    public bool SummonMinions()
    {
        if (isBusy) return false;
        StartCoroutine(SummonRoutine());
        return true;
    }

    private IEnumerator SummonRoutine()
    {
        isBusy = true;
        pathfinding.StopMoving();
        animator.SetTrigger("Cast");

        yield return WaitForAnimation("Cast");

        Vector2 offset1 = new Vector2(1.5f, 0);
        Vector2 offset2 = new Vector2(-1.5f, 0);
        Vector2 offset3 = new Vector2(0, 1.5f);
        Vector2 offset4 = new Vector2(0, -1.5f);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset1, Quaternion.identity);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset2, Quaternion.identity);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset3, Quaternion.identity);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset4, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        isBusy = false;
    }

    public bool TeleportAway()
    {
        if (isBusy) return false;
        StartCoroutine(TeleportRoutine());
        return true;
    }

    private IEnumerator TeleportRoutine()
    {
        isBusy = true;
        pathfinding.StopMoving();
        animator.SetTrigger("FadeOut");

        yield return WaitForAnimation("FadeOut");

        Vector2 newPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * 10f;
        pathfinding.TeleportTo(newPos);

        animator.SetTrigger("FadeIn");
        yield return WaitForAnimation("FadeIn");

        isBusy = false;
    }
    private IEnumerator WaitForAnimation(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(stateName))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
        while (stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
