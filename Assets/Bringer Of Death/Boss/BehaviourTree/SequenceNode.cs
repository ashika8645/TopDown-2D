using System.Collections.Generic;

public class Sequence : BTNode
{
    private List<BTNode> nodes;
    public Sequence(List<BTNode> nodes) => this.nodes = nodes;

    public override bool Tick()
    {
        foreach (var node in nodes)
        {
            if (!node.Tick()) return false;
        }
        return true;
    }
}
