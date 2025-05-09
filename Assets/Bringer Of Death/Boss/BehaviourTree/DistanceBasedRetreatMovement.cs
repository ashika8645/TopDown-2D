using UnityEngine;

public class DistanceBasedRetreatMovement : BTNode
{
    private BossSkillActions actions;
    private BossPathfinding pathfinding;

    public DistanceBasedRetreatMovement(BossSkillActions actions)
    {
        this.actions = actions;
        this.pathfinding = actions.GetComponent<BossPathfinding>();
    }

    public override bool Tick()
    {
        float dist = actions.DistanceToPlayer();
        Vector2 myPos = actions.transform.position;
        Vector2 playerPos = Player.Instance.transform.position;
        Vector2 dir = (playerPos - myPos).normalized;

        if (dist <= 4f)
        {
            pathfinding.StopMoving();
        }
        else if (dist <= 6f)
        {
            Vector2 retreatPos = myPos - dir * 5f;
            pathfinding.MoveToPoint(retreatPos);
        }
        else if (dist > 10f)
        {
            Vector2 targetPos = playerPos - dir * 8f; 
            pathfinding.MoveToPoint(targetPos);
        }
        else
        {
            pathfinding.StopMoving();
        }

        return true;
    }
}
