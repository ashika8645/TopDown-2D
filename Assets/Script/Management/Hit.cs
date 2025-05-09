using UnityEngine;

public class Hit : MonoBehaviour
{
    private Animator animator;
    private float destroyTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        destroyTime = animator.GetCurrentAnimatorStateInfo(0).length; 
        Destroy(gameObject, destroyTime);
    }
}
