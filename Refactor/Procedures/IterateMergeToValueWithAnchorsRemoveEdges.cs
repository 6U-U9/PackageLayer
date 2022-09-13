using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class IterateMergeToValueWithAnchorsRemoveEdges : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInputAndRemove loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoListWithAnchors generateTopoList;
        IterateLayerWithAnchors iterateLayer;
        MergeLayerToCertainCount mergeLayer;

        public IterateMergeToValueWithAnchorsRemoveEdges(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "迭代算法";

            int length = -1;
            int direction = 1; //因为锚点的存在，算法不能正向运行
            int methodIndex = 0;
            List<string> anchors = new List<string>()
            {
                "glibc",
                "basesystem",
                "filesystem",
                "setup",
                "anolis-release",
                "coreutils",
                "gcc",
                "systemd",
            };
            List<string> removePackages = new List<string>()
            {

            };
            List<(string, string)> removeEdges = new List<(string, string)>()
            {
                ("glibc","libselinux"),
            };

            input = new Input(environment);
            removeEdges.AddRange(input.reverseEdges);
            loadInput = new LoadInputAndRemove(removePackages, removeEdges);
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoListWithAnchors(anchors, direction, methodIndex);
            iterateLayer = new IterateLayerWithAnchors(anchors, direction);
            mergeLayer = new MergeLayerToCertainCount(4, direction, 0, 0, 0, 1);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                buildIndirectEdges.ToString(),
                generateTopoList.ToString(),
                iterateLayer.ToString(),
                mergeLayer.ToString(),
            };
            return description;
        }

        public override void Execute()
        {
            Input input = new Input(environment);
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            buildIndirectEdges.Process(mergedGraph);
            List<Node> topolist = generateTopoList.Process(mergedGraph);
            Hierarchies hierarchies = iterateLayer.Process(topolist);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
