using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 5f;

    [SerializeField] private GameObject spellHit;
    [SerializeField] private float speed = 7f;

    private Vector3 moveDirection;

    private void Start()
    {
        moveDirection = transform.right.normalized;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Destructible") || collision.CompareTag("Environment"))
        {
            if (spellHit)
                Instantiate(spellHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
