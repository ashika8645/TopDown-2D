using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float detectedRange = 10f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    private bool canAttack = true;

    private enum State
    {
        Roaming,
        Chasing,
        Attacking
    }

    private Vector2 roamPosition;
    private float timeRoaming = 0f;

    private State state;
    private EnemyPathfinding enemyPathfinding;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.Instance.transform.position);

        switch (state)
        {
            default:
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
        timeRoaming += Time.deltaTime;

        enemyPathfinding.MoveTo(roamPosition);

        if (distanceToPlayer < detectedRange)
        {
            state = State.Chasing;
            return;
        }

        if (timeRoaming > roamChangeDirFloat)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Chasing(float distanceToPlayer)
    {
        Vector2 directionToPlayer = (Player.Instance.transform.position - transform.position).normalized;
        enemyPathfinding.MoveTo(directionToPlayer);

        if (distanceToPlayer < attackRange)
        {
            state = State.Attacking;
        }
        else if (distanceToPlayer > detectedRange)
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

        if (attackRange != 0 && canAttack)
        {
            canAttack = false;
            (enemyType as IEnemy).Attack();

            if (stopMovingWhileAttacking)
            {
                enemyPathfinding.StopMoving();
            }
            else
            {
                Vector2 directionToPlayer = (Player.Instance.transform.position - transform.position).normalized;
                enemyPathfinding.MoveTo(directionToPlayer);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
