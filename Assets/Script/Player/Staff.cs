using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    [SerializeField] private Transform SpellSpawnPoint;
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float attackCooldown = 0.5f;

    private Player playerController;
    private Animator myAnimator;

    private Transform activeWeapon;
    private Transform spellspawnpoint;
    private bool canAttack = true;

    private Vector3 lastMousePosition;
    private float detectionRange = 10f;
    private Transform autoTarget;

    private void Start()
    {
        spellspawnpoint = SpellSpawnPoint;
    }

    private void Awake()
    {
        playerController = GetComponentInParent<Player>();
        myAnimator = GetComponent<Animator>();
        activeWeapon = transform.parent;
        spellspawnpoint = transform.parent;
    }

    private void Update()
    {
        TrackMouseIdle();

        RotateSpellSpawnPoint();

        if (Input.GetButtonDown("Attack") && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        myAnimator.SetTrigger("castSpell");
        StartCoroutine(HandleCooldown());
    }

    private IEnumerator HandleCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void EndCastSpellEvent()
    {
        Instantiate(spellPrefab, SpellSpawnPoint.position, SpellSpawnPoint.rotation);
    }

    public void ResetState()
    {
        StopAllCoroutines();
        canAttack = true;
    }

    private void TrackMouseIdle()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            autoTarget = null;
        }
        else
        {
            FindNearestEnemy();
        }
    }

    private void FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerController.transform.position, detectionRange);

        float closestDistance = detectionRange;
        Transform nearest = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(hit.transform.position, playerController.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nearest = hit.transform;
                }
            }
        }

        if (nearest != null)
        {
            autoTarget = nearest;
        }
    }

    private void RotateSpellSpawnPoint()
    {
        Vector3 targetPos;

        if (autoTarget != null)
        {
            targetPos = autoTarget.position;
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            targetPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        }

        Vector2 direction = (targetPos - playerController.transform.position).normalized;
        SpellSpawnPoint.position = playerController.transform.position + new Vector3(direction.x, direction.y, 0) * 3f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        SpellSpawnPoint.rotation = Quaternion.Euler(0, 0, angle);
    }
}
