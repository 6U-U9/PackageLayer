using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class ImprovedMergeToValueRemoveEdges : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInputAndRemove loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoList generateTopoList;
        ImprovedLayer improvedLayer;
        MergeLayerToCertainCount mergeLayer;

        public ImprovedMergeToValueRemoveEdges(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "改进算法";
            
            
            int length = -1;
            int direction = 1;
            int methodIndex = 0;
            List<string> removePackages =new List<string>()
            {
                
            };
            List<(string, string)> removeEdges=new List<(string,string)>()
            {
                ("glibc","libselinux"),
            };

            input = new Input(environment);
            loadInput = new LoadInputAndRemove(removePackages, removeEdges);
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoList(direction, methodIndex);
            improvedLayer = new ImprovedLayer(direction);
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
            buildIndirectEdges.Process(mergedGraph);
            List<Node> topolist = generateTopoList.Process(mergedGraph);
            Hierarchies hierarchies = improvedLayer.Process(topolist);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
