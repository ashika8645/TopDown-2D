using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BossSkillActions), typeof(BossHealth))]
public class BossBTController : MonoBehaviour
{
    private BTNode rootNode;
    private BossSkillActions skills;
    private BossHealth health;
    private BossPathfinding pathfinding;
    private Animator animator;

    private void Awake()
    {
        skills = GetComponent<BossSkillActions>();
        health = GetComponent<BossHealth>();
        pathfinding = GetComponent<BossPathfinding>();
        animator = GetComponent<Animator>();           
    }


    private void Start()
    {
        LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

        rootNode = new Selector(new List<BTNode>
        {
            new Sequence(new List<BTNode> {
                new ConditionNode(() => health.GetHealthPercent() < 0.3f),
                new CooldownNode(15f, new ActionNode(skills.SummonMinions))
            }),
            new Sequence(new List<BTNode> {
                new ConditionNode(() => health.GetHealthPercent() < 0.7f),
                new CooldownNode(7f, new ActionNode(skills.CastAOE))
            }),
            new Sequence(new List<BTNode> {
                new ConditionNode(() => skills.DistanceToPlayer() <= 5f),
                new CooldownNode(7f, new ActionNode(skills.TeleportAway))
            }),
            new Sequence(new List<BTNode> {
                new ConditionNode(() => skills.DistanceToPlayer() <= skills.attackRange),
                new LineOfSightCondition(skills.transform, LayerMask.GetMask("Obstacle")),
                new CooldownNode(3f, new ActionNode(skills.ShootSpell))
            }),
            new DistanceBasedRetreatMovement(skills) 
        });
    }

    private void Update()
    {
        rootNode?.Tick();

        float moveSpeed = pathfinding.CurrentSpeed;
        animator.SetFloat("Speed", moveSpeed);

        Vector2 toPlayer = Player.Instance.transform.position - transform.position;
        if (toPlayer.x < 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }
}
