using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float detectedRange = 15f;
    [SerializeField] private float teleportRange = 5f;

    [Header("Skill Prefabs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject aoeSpellPrefab;
    [SerializeField] private GameObject summonEnemyPrefab;

    [Header("Cooldowns")]
    [SerializeField] private float spellCooldown = 5f;
    [SerializeField] private float aoeCooldown = 7f;
    [SerializeField] private float summonCooldown = 15f;
    [SerializeField] private float teleportCooldown = 7f;

    private float spellTimer, aoeTimer, summonTimer, teleportTimer;

    private enum State { Roaming, Chasing, Attacking }
    private State state;

    private Vector2 roamPosition;
    private BossPathfinding bossPathfinding;
    private BossHealth bossHealth;
    private Animator animator;
    private bool isAttacking = false;

    private void Awake()
    {
        bossPathfinding = GetComponent<BossPathfinding>();
        if (bossPathfinding == null)
            Debug.LogError("Missing BossPathfinding on " + gameObject.name);

        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
            Debug.LogError("Missing BossHealth on " + gameObject.name);

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Missing Animator on " + gameObject.name);

        state = State.Roaming;
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        animator.SetFloat("Speed", bossPathfinding.CurrentSpeed);

        MovementStateControl();

        spellTimer += Time.deltaTime;
        aoeTimer += Time.deltaTime;
        summonTimer += Time.deltaTime;
        teleportTimer += Time.deltaTime;
    }

    private void MovementStateControl()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);

        switch (state)
        {
            case State.Roaming:
                Roaming(distanceToPlayer);
                break;
            case State.Chasing:
                Chasing(distanceToPlayer);
                break;
            case State.Attacking:
                Attacking(distanceToPlayer);
                break;
        }
    }

    private void Roaming(float distanceToPlayer)
    {
        bossPathfinding.MoveToPoint(roamPosition);

        if (distanceToPlayer < detectedRange)
        {
            state = State.Chasing;
            return;
        }

        if (Vector2.Distance(transform.position, roamPosition) < 0.5f)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Chasing(float distanceToPlayer)
    {
        bossPathfinding.MoveToPoint(Player.Instance.transform.position);

        if (distanceToPlayer < attackRange)
        {
            state = State.Attacking;
        }
        else if (distanceToPlayer > detectedRange + 2f)
        {
            state = State.Roaming;
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            state = State.Chasing;
            return;
        }

        float healthPercent = bossHealth.GetHealthPercent();

        if (!isAttacking && spellTimer >= spellCooldown)
        {
            StartCoroutine(PerformRandomSpellAttack());
            spellTimer = 0;
        }

        if (healthPercent <= 0.7f && aoeTimer >= aoeCooldown)
        {
            StartCoroutine(CastAOERoutine());
            aoeTimer = 0;
        }

        if (healthPercent <= 0.5f && summonTimer >= summonCooldown)
        {
            StartCoroutine(SummonRoutine());
            summonTimer = 0;
        }

        if (distanceToPlayer < teleportRange && teleportTimer >= teleportCooldown)
        {
            StartCoroutine(Teleport());
            teleportTimer = 0;
        }
    }

    private IEnumerator PerformRandomSpellAttack()
    {
        isAttacking = true;

        yield return PlayCastAnimation(0.4f);

        int attackType = Random.Range(0, 2);

        if (attackType == 0)
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

        yield return new WaitForSeconds(1f);

        float distanceToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);
        state = (distanceToPlayer > attackRange) ? State.Chasing : State.Attacking;

        isAttacking = false;
    }

    private IEnumerator CastAOERoutine()
    {
        yield return PlayCastAnimation(0.5f);

        Vector2 playerPos = Player.Instance.transform.position;
        Instantiate(aoeSpellPrefab, playerPos, Quaternion.identity);
    }

    private IEnumerator SummonRoutine()
    {
        yield return PlayCastAnimation(0.5f);

        Vector2 offset1 = new Vector2(1.5f, 0);
        Vector2 offset2 = new Vector2(-1.5f, 0);

        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset1, Quaternion.identity);
        Instantiate(summonEnemyPrefab, (Vector2)transform.position + offset2, Quaternion.identity);
    }

    private IEnumerator PlayCastAnimation(float delayBeforeContinue)
    {
        bossPathfinding.StopMoving();
        animator.SetTrigger("Cast");
        yield return new WaitForSeconds(delayBeforeContinue);
    }

    private IEnumerator Teleport()
    {
        bossPathfinding.StopMoving();
        animator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(1f);

        Vector2 newPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * 7f);
        bossPathfinding.TeleportTo(newPos);

        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
    }

    private Vector2 GetRoamingPosition()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * 5f;
    }

    public void PlayDeathAnimation()
    {
        bossPathfinding.StopMoving();
        animator.SetTrigger("Death");
        enabled = false;
    }
}
