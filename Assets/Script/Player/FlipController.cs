using UnityEngine;

public class FlipController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Cho Player
    [SerializeField] private Transform[] objectsToFlip;     // Cho Sword, Staff, ...

    private Vector3 previousMousePos;
    private float idleTimer;
    [SerializeField] private float idleThreshold = 1.5f;

    public bool FacingLeft { get; private set; }

    private Rigidbody2D rb;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        previousMousePos = Input.mousePosition;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos == previousMousePos)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleThreshold)
            {
                if (rb != null)
                {
                    if (rb.velocity.x < 0)
                    {
                        SetFacing(true);
                    }
                    else if (rb.velocity.x > 0)
                    {
                        SetFacing(false);
                    }
                }
            }
        }
        else
        {
            idleTimer = 0;
            SetFacing(mousePos.x < screenPoint.x);
        }

        previousMousePos = mousePos;
    }

    private void SetFacing(bool faceLeft)
    {
        if (FacingLeft == faceLeft) return;

        FacingLeft = faceLeft;

        if (spriteRenderer != null)
            spriteRenderer.flipX = faceLeft;

        float direction = faceLeft ? -1f : 1f;
        foreach (Transform obj in objectsToFlip)
        {
            obj.localScale = new Vector3(direction, 1f, 1f);
        }
    }
}
