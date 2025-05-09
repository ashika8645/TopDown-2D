using System.Collections;
using UnityEngine;

public class Charger : MonoBehaviour, IEnemy
{
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float stopDuration = 2f;

    private EnemyPathfinding enemyPathfinding;
    private bool isCharging = false;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
    }

    public void Attack()
    {
        if (isCharging) return;

        StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;

        Vector2 chargeDirection = (Player.Instance.transform.position - transform.position).normalized;

        enemyPathfinding.MoveTo(chargeDirection * chargeSpeed);

        yield return new WaitForSeconds(0.3f);

        enemyPathfinding.StopMoving();

        yield return new WaitForSeconds(stopDuration);

        isCharging = false;
    }
}
