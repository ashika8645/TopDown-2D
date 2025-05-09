using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private Transform ArrowSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float idleThreshold = 2f;

    private Player playerController;
    private bool canAttack = true;

    private Vector3 lastMousePosition;
    private float mouseIdleTimer = 0f;
    private Transform nearestEnemy;

    private float mouseIde;
    private float mouseIdecd = 1.5f;
    private Vector3 previousMousePos;

    private Rigidbody2D rb;
    private void Awake()
    {
        playerController = GetComponentInParent<Player>();
        lastMousePosition = Input.mousePosition;
        rb = GetComponentInParent<Rigidbody2D>();
    }
    private void Start()
    {
        previousMousePos = Input.mousePosition;
    }

    private void Update()
    {
        TrackMouseIdle();
        AdjustPlayerFacingDirection();
        if (mouseIdleTimer >= idleThreshold)
        {
            FindNearestEnemy();
            RotateToward(nearestEnemy != null ? nearestEnemy.position : GetMouseWorldPosition());
        }
        else
        {
            RotateToward(GetMouseWorldPosition());
        }

        if (Input.GetButtonDown("Attack") && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Instantiate(arrowPrefab, ArrowSpawnPoint.position, ArrowSpawnPoint.rotation);
        StartCoroutine(HandleCooldown());
    }

    private IEnumerator HandleCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        canAttack = true;
    }

    private void RotateToward(Vector3 targetPos)
    {
        Vector2 direction = (targetPos - playerController.transform.position).normalized;

        transform.position = playerController.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void TrackMouseIdle()
    {
        Vector3 currentMousePosition = Input.mousePosition;

        if ((currentMousePosition - lastMousePosition).sqrMagnitude < 0.01f)
        {
            mouseIdleTimer += Time.deltaTime;
        }
        else
        {
            mouseIdleTimer = 0f;
        }

        lastMousePosition = currentMousePosition;
    }

    private void FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerController.transform.position, detectionRange);

        float closestDistance = detectionRange;
        Transform closest = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(playerController.transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = hit.transform;
                }
            }
        }

        nearestEnemy = closest;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos == previousMousePos)
        {
            mouseIde += Time.deltaTime;
            if (mouseIde >= mouseIdecd)
            {
                if (rb.velocity.x < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (rb.velocity.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            mouseIde = 0;
            if (mousePos.x < playerScreenPoint.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        previousMousePos = mousePos;
    }
}
