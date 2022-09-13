using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class MaxDepthMergeToValueWithAnchors : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        MaxDepthLayerWithAnchors maxDepthLayer;
        MergeLayerToCertainCount mergeLayer;

        public MaxDepthMergeToValueWithAnchors(string environment, string outputPath)
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
            maxDepthLayer = new MaxDepthLayerWithAnchors(anchors, direction);
            mergeLayer = new MergeLayerToCertainCount(4, direction, 0, 0, 0, 1);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
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
            Hierarchies hierarchies = maxDepthLayer.Process(mergedGraph);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
