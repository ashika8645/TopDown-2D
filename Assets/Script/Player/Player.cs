using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;

    [Header("Stamina")]
    [SerializeField] private int maxStamina = 3;
    [SerializeField] private Transform staminaUIParent;
    private int currentStamina;
    private Coroutine staminaRegenCoroutine;
    private float staminaRegenDelay = 3f;
    private CapsuleCollider2D myCollider;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private Knockback knockback;

    private float startingMoveSpeed;
    private float mouseIde;
    private float mouseIdecd = 1.5f;

    public bool FacingLeft { get; private set; }

    private bool isDashing = false;
    private Vector3 previousMousePos;

    private Sword sword;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        knockback = GetComponent<Knockback>();
        myCollider = GetComponent<CapsuleCollider2D>();
        if (staminaUIParent == null)
        {
            var go = GameObject.Find("Stamina Container/Stamina");
            if (go != null)
                staminaUIParent = go.transform;
        }
    }

    private void Start()
    {
        startingMoveSpeed = moveSpeed;
        previousMousePos = Input.mousePosition;

        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Dash"))
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        if (knockback.GettingKnockedBack) return;

        Move();
        AdjustPlayerFacingDirection();
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * moveSpeed;
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
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
                    FacingLeft = true;
                    transform.localScale = new Vector3(-1, 1, 1); 
                }
                else if (rb.velocity.x > 0)
                {
                    FacingLeft = false;
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else
        {
            mouseIde = 0;
            if (mousePos.x < playerScreenPoint.x)
            {

                FacingLeft = true;
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else
            {
                FacingLeft = false;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        previousMousePos = mousePos;
    }

    private void Dash()
    {
        if (isDashing || currentStamina <= 0) return;

        isDashing = true;
        moveSpeed *= dashSpeed;
        myTrailRenderer.emitting = true;
        myCollider.enabled = false;
        UseStamina();

        StartCoroutine(EndDashRoutine());
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = 1f;

        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        myCollider.enabled = true;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    private void UseStamina()
    {
        currentStamina--;
        UpdateStaminaUI();

        if (staminaRegenCoroutine == null)
        {
            staminaRegenCoroutine = StartCoroutine(RecoverStaminaOverTime());
        }
    }

    private IEnumerator RecoverStaminaOverTime()
    {
        while (currentStamina < maxStamina)
        {
            yield return new WaitForSeconds(staminaRegenDelay);
            currentStamina++;
            UpdateStaminaUI();
        }

        staminaRegenCoroutine = null;
    }

    private void UpdateStaminaUI()
    {
        if (staminaUIParent == null) return;

        for (int i = 0; i < staminaUIParent.childCount; i++)
        {
            staminaUIParent.GetChild(i).gameObject.SetActive(i < currentStamina);
        }
    }
}
