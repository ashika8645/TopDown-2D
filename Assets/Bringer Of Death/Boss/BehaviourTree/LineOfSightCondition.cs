using UnityEngine;

public class LineOfSightCondition : BTNode
{
    private Transform source;
    private LayerMask obstacleMask;
    private float maxDistance;

    public LineOfSightCondition(Transform source, LayerMask mask, float maxDistance = 20f)
    {
        this.source = source;
        this.obstacleMask = mask;
        this.maxDistance = maxDistance;
    }

    public override bool Tick()
    {
        Vector2 dir = Player.Instance.transform.position - source.position;
        RaycastHit2D hit = Physics2D.Raycast(source.position, dir.normalized, maxDistance, obstacleMask);

        return hit.collider == null || hit.collider.CompareTag("Player");
    }
}
