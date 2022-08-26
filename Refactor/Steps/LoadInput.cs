namespace Refactor.Steps;

public class LoadInput : Step<Input, bool>
{
    public static LoadInput Function = new();
    public override string StepDescription
    {
        get { return "Load"; }
    }
    public override string DetailDescription 
    {
        get { return "Build Packages"; }
    }

    public override bool Process(Input input)
    {
        foreach (var edges in input.dependencies)
        {
            Package dependent = Package.Create(edges[0], input.humanLayers[edges[0]]);
            Package dependency = Package.Create(edges[1], input.humanLayers[edges[1]]);
            dependent.dependency.Add(dependency);
            dependency.dependent.Add(dependent);
        }
        return true;
    }
}