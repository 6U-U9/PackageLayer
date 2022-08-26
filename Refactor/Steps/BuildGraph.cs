namespace Refactor.Steps;

public class BuildGraph : Step<bool, Graph>
{
    public static BuildGraph Function = new();
    public override string StepDescription
    {
        get { return "Load"; }
    }
    public override string DetailDescription 
    {
        get { return "Build Graph"; }
    }

    public override Graph Process(bool input)
    {
        Graph graph = new Graph();
        foreach (var package in Package.packages.Values)
        {
            graph.AddNode(package);
        }
        graph.BuildEdges();
        return graph;
    }
}