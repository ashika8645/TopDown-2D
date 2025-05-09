using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    public int damage = 15;
    public float delay = 0.5f;

    private void Start()
    {
        Invoke("Explode", delay);
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Gây damage
            }
        }

        Destroy(gameObject);
    }
}
