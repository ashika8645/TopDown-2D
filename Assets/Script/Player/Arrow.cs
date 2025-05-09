
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private Vector3 direction;
    private float lifeTime = 3f;
    private CapsuleCollider2D myCollider;
    private void Awake()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        direction = transform.right;
    }


    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) Destroy(gameObject);

        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Environment"))
        {
            speed = 0f;
            myCollider.enabled = false;
        }
    }
}
