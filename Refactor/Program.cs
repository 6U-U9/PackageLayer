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

    Procedure originMergeWithLayer1 = new OriginMergeToValueWithLayer1(e, e + "_merge4_fixed_layer1");
    Procedure iterateMergeWithLayer1 = new IterateMergeToValueWithLayer1(e, e + "_merge4_fixed_layer1");
    Procedure improvedMergeWithLayer1 = new ImprovedMergeToValueWithLayer1(e, e + "_merge4_fixed_layer1");
    Procedure maxDepthMergeWithLayer1 = new MaxDepthMergeToValueWithLayer1(e, e + "_merge4_fixed_layer1");

    Procedure findKeyEdges = new KeyEdgesRemoveEdges(e, e);
    Procedure humanAnalysis = new HumanAnalysis(e, e+"_human_analysis");

    //findKeyEdges.Execute();

    //humanAnalysis.Execute();
    originMergeWithLayer1.Execute();
    iterateMergeWithLayer1.Execute();
    improvedMergeWithLayer1.Execute();
    maxDepthMergeWithLayer1.Execute();
}

Console.WriteLine("Finished");
