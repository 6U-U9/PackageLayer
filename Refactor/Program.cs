using Refactor;
using Refactor.Steps;

Input input = new Input("mysql");
LoadInput loadInput = new(); var p = loadInput.Process(input);
BuildGraph buildGraph = new(); Graph g = buildGraph.Process(p);
MergeCircleNodes mergeCircleNodes = new MergeCircleNodes(); g = mergeCircleNodes.Process(g);
BuildIndirectEdges buildIndirectEdges = new BuildIndirectEdges(); buildIndirectEdges.Process(g);
GenerateTopoList generateTopoList = new GenerateTopoList(); var l = generateTopoList.Process(g);
OriginalLayer OriginalLayer = new OriginalLayer(); var result = OriginalLayer.Process(l);
foreach (var n in l)
{
    Console.WriteLine(n);
}
Console.WriteLine(result);

List<string> envs = new List<string> { "mysql", "nginx", "redis" };
foreach (string e in envs)
{
    //RUN

}

Console.WriteLine("FUCK FUCK FUCK");