using UnityEngine;

public class BossTeleporter : MonoBehaviour
{
    private Animator animator;
    private BossPathfinding pathfinding;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pathfinding = GetComponent<BossPathfinding>();
    }

    public void RequestTeleport()
    {
        pathfinding.StopMoving();
        animator.SetTrigger("FadeOut");
    }
    public void OnFadeOutComplete()
    {
        Vector2 newPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * 7f;
        pathfinding.TeleportTo(newPos);
        animator.SetTrigger("FadeIn");
    }
}
