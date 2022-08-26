namespace Refactor.Steps;

public class GenerateTopoList:Step<Graph,List<Node>>
{
    public static GenerateTopoList Function = new();
    public override string StepDescription
    {
        get { return "Main"; }
    }
    public override string DetailDescription 
    {
        get { return "Generate nodes topo list Down-Top sorted by direct dependency and indirect dependencies"; }
    }

    public override List<Node> Process(Graph input)
    {

    }
}