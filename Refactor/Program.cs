using Refactor;
using Refactor.Core;
using Refactor.Procedures;

List<string> envs = new List<string> {"mysql" , "nginx", "redis"};//
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

    Procedure iterateMergeToAnchor = new IterateMergeToValueWithAnchors(e, e + "_merge4_anchor");
    Procedure improvedMergeToAnchor = new ImprovedMergeToValueWithAnchors(e, e + "_merge4_anchor");
    Procedure maxDepthMergeToAnchor = new MaxDepthMergeToValueWithAnchors(e, e + "_merge4_anchor");

    Procedure iterateMergeRemoveEdges = new IterateMergeToValueRemoveEdges(e, e + "_merge4_remove_edges");
    Procedure improvedMergeRemoveEdges = new ImprovedMergeToValueRemoveEdges(e, e + "_merge4_remove_edges");
    Procedure maxDepthMergeRemoveEdges = new MaxDepthMergeToValueRemoveEdges(e, e + "_merge4_remove_edges");

    Procedure iterateMergeWithAnchorsRemoveEdges = new IterateMergeToValueWithAnchorsRemoveEdges(e, e + "_merge4_anchor_remove_edges");
    Procedure improvedMergeWithAnchorsRemoveEdges = new ImprovedMergeToValueWithAnchorsRemoveEdges(e, e + "_merge4_anchor_remove_edges");
    Procedure maxDepthMergeWithAnchorsRemoveEdges = new MaxDepthMergeToValueWithAnchorsRemoveEdges(e, e + "_merge4_anchor_remove_edges");

    Procedure findKeyEdges = new KeyEdges(e, e);

    iterateMergeWithAnchorsRemoveEdges.Execute();
    improvedMergeWithAnchorsRemoveEdges.Execute();
    maxDepthMergeWithAnchorsRemoveEdges.Execute();
}

Console.WriteLine("Finished");
