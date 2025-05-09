using System.Collections.Generic;

public class Selector : BTNode
{
    private List<BTNode> nodes;
    public Selector(List<BTNode> nodes) => this.nodes = nodes;

    public override bool Tick()
    {
        foreach (var node in nodes)
        {
            if (node.Tick()) return true;
        }
        return false;
    }
}
