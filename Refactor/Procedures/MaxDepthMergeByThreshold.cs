using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class MaxDepthMergeByThreshold : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        MaxDepthLayer maxDepthLayer;
        MergeLayerByThreshold mergeLayer;

        public MaxDepthMergeByThreshold(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "最大深度算法";

            int direction = 1;

            loadInput = new LoadInput();
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            maxDepthLayer = new MaxDepthLayer(direction);
            mergeLayer = new MergeLayerByThreshold(0, direction, 2, 0, 1, 1);
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
            Input input = new Input(environment);
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            mergeLayer.threshold = mergedGraph.nodeSet.Values.ToHashSet().Count() / 2;
            Hierarchies hierarchies = maxDepthLayer.Process(mergedGraph);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
