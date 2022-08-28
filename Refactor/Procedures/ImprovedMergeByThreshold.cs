using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Procedures
{
    public class ImprovedMergeByThreshold : Procedure
    {
        public string environment;
        public int direction;
        public string filepath;
        public string sheetname;

        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoList generateTopoList;
        ImprovedLayer improvedLayer;
        MergeLayerByThreshold mergeLayer;

        public ImprovedMergeByThreshold(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "改进算法";

            int length = -1;
            int direction = 1;
            int methodIndex = 0;

            loadInput = new LoadInput();
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoList(direction, methodIndex);
            improvedLayer = new ImprovedLayer(direction);
            mergeLayer = new MergeLayerByThreshold(0, direction, 0, 0, 1, 1);
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
                improvedLayer.ToString(),
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
            Hierarchies hierarchies = improvedLayer.Process(topolist);
            mergeLayer.threshold = graph.nodeSet.Values.ToHashSet().Count() / 2;
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
