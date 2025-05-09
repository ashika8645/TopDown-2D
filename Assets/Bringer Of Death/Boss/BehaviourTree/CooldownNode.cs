using UnityEngine;

public class CooldownNode : BTNode
{
    private float cooldownTime;
    private float lastUsed;
    private BTNode child;

    public CooldownNode(float cooldownTime, BTNode child)
    {
        this.cooldownTime = cooldownTime;
        this.child = child;
    }

    public override bool Tick()
    {
        if (Time.time - lastUsed < cooldownTime) return false;
        if (child.Tick())
        {
            lastUsed = Time.time;
            return true;
        }
        return false;
    }
}
