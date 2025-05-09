using UnityEngine;

public class BossPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private bool isDashing = false;

    private Knockback knockback;
    private SpriteRenderer spriteRenderer;
    public float CurrentSpeed => moveDir.magnitude;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (knockback != null && knockback.GettingKnockedBack) return;
        if (isDashing) return;

        Move();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }

    public void MoveToDirection(Vector2 direction)
    {
        moveDir = direction.normalized;
    }

    public void MoveToPoint(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;
        MoveToDirection(direction);
    }

    public void StopMoving()
    {
        moveDir = Vector2.zero;
    }
    public void DashTo(Vector2 target, float dashForce, float duration)
    {
        StartCoroutine(DashRoutine(target, dashForce, duration));
    }

    private System.Collections.IEnumerator DashRoutine(Vector2 target, float force, float duration)
    {
        isDashing = true;
        Vector2 dir = (target - rb.position).normalized;
        float timer = 0f;

        while (timer < duration)
        {
            rb.MovePosition(rb.position + dir * force * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void TeleportTo(Vector2 position)
    {
        transform.position = position;
        StopMoving(); 
    }
}
