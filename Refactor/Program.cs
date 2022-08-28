using Refactor;
using Refactor.Procedures;

List<string> envs = new List<string> { "mysql" };//, "nginx", "redis"
foreach (string e in envs)
{
    Procedure origin = new Origin(e,e);
    Procedure iterate = new Iterate(e, e);
    Procedure improved = new Improved(e, e);
    Procedure maxDepth = new MaxDepth(e, e);

    Procedure iterateMerge = new IterateMergeByThreshold(e, e + "_merge3");
    Procedure improvedMerge = new ImprovedMergeByThreshold(e, e + "_merge3");
    Procedure maxDepthMerge = new MaxDepthMergeByThreshold(e, e + "_merge3");

    Procedure iterateMergeTo = new IterateMergeToValue(e, e + "_merge4");
    Procedure improvedMergeTo = new ImprovedMergeToValue(e, e + "_merge4");
    Procedure maxDepthMergeTo = new MaxDepthMergeToValue(e, e + "_merge4");

    iterateMerge.Execute();
    improvedMerge.Execute();
    maxDepthMerge.Execute();

}

Console.WriteLine("FUCK FUCK FUCK");