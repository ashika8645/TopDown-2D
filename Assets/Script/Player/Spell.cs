using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float detectionAngle = 45f;
    [SerializeField] private GameObject spellHit;

    private Vector3 targetDirection;
    private Transform closestEnemy;
    private float lifeTime = 3f;

    private void Start()
    {
        targetDirection = transform.right;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        lifeCheck();

        DetectEnemyInFront();

        if (closestEnemy != null)
        {
            Vector3 directionToEnemy = (closestEnemy.position - transform.position).normalized;
            targetDirection = Vector3.Lerp(targetDirection, directionToEnemy, Time.deltaTime * 5f);
        }

        transform.position += targetDirection * speed * Time.deltaTime;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void DetectEnemyInFront()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        closestEnemy = null;
        float closestDistance = detectionRadius;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = (hit.transform.position - transform.position).normalized;

                float angleToEnemy = Vector3.Angle(targetDirection, directionToEnemy);
                if (angleToEnemy < detectionAngle)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
                    if (distanceToEnemy < closestDistance)
                    {
                        closestEnemy = hit.transform;
                        closestDistance = distanceToEnemy;
                    }
                }
            }
        }
    }

    private void lifeCheck()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Destructible") || collision.CompareTag("Environment"))
        {
            Instantiate(spellHit, transform.position, Quaternion.identity);
            Debug.Log($"Hit {collision.tag}!");
            Destroy(gameObject);
        }
    }
}
