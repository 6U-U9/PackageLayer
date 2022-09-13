using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class MaxDepthMergeToValueWithLayer1 : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        GenerateFixedNode generateFixedNode;
        MaxDepthLayerWithLayer1 maxDepthLayer;
        MergeLayerToCertainCount mergeLayer;

        public MaxDepthMergeToValueWithLayer1(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "最大深度算法";

            int direction = 1; //因为锚点的存在，算法不能正向运行
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
            maxDepthLayer = new MaxDepthLayerWithLayer1(anchors, direction);
            mergeLayer = new MergeLayerToCertainCount(4, direction, 1, 0, 0, 1);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                generateFixedNode.ToString(),
                maxDepthLayer.ToString(),
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
            Hierarchies hierarchies = maxDepthLayer.Process(fixNodeGraph);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
