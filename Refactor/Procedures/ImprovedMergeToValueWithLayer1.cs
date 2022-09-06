using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class ImprovedMergeToValueWithLayer1 : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        GenerateFixedNode generateFixedNode;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoListWithLayer1 generateTopoList;
        ImprovedLayerWithLayer1 improvedLayer;
        MergeLayerToCertainCount mergeLayer;

        public ImprovedMergeToValueWithLayer1(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "改进算法";

            int length = -1;
            int direction = 1;
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

            input = new Input(environment);
            loadInput = new LoadInput();
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            generateFixedNode = new GenerateFixedNode(anchors, direction);
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoListWithLayer1(anchors, direction, methodIndex);
            improvedLayer = new ImprovedLayerWithLayer1(anchors, direction);
            mergeLayer = new MergeLayerToCertainCount(4, direction, 0, 0, 0, 1);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                generateFixedNode.ToString(),
                buildIndirectEdges.ToString(),
                generateTopoList.ToString(),
                improvedLayer.ToString(),
                mergeLayer.ToString(),
            };
            return description;
        }

        public override void Execute()
        {
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            Graph fixNodeGraph = generateFixedNode.Process(mergedGraph);
            buildIndirectEdges.Process(fixNodeGraph);
            List<Node> topolist = generateTopoList.Process(fixNodeGraph);
            Hierarchies hierarchies = improvedLayer.Process(topolist);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
