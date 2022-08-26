namespace Refactor.Steps;

public class MergeCircleNodes : Step<Graph, Graph>
{
    public static MergeCircleNodes Function = new();

    public override string StepDescription
    {
        get { return "Modify"; }
    }

    public override string DetailDescription
    {
        get { return "Merge nodes in circles to one node"; }
    }

    private void dfs(Graph graph)
    {
        foreach (Node node in graph.nodeSet.Values)
        {
            node.color = 0;
            node.forward = null;
        }

        foreach (Node node in graph.nodeSet.Values)
        {
            if (node.color == 0)
                dfs_search(graph, node);
        }
    }

    private void dfs_search(Graph graph, Node node)
    {
        node.color = 1;
        for (int i = 0; i < node.dependencies.Count; i++)
        {
            if (!graph.nodeSet.Values.Contains(node)) 
                break;
            Node next = node.dependencies[i];
            if (next.color == 0)
            {
                next.forward = node;
                dfs_search(graph, next);
            }
            else if (next.color == 1)
            {
                List<Node> circle = new List<Node>();
                while (node != next)
                {
                    circle.Add(node);
                    node = node.forward;
                }

                circle.Add(node);
                Node newNode = graph.UnionNodes(circle);
                newNode.forward = next.forward;
                dfs_search(graph, newNode);
            }
            else
            {
                continue;
            }
        }

        node.color = 2;
    }

    public override Graph Process(Graph input)
    {
        Graph graph = input.Copy();
        dfs(input);
        return graph;
    }
}