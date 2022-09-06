using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures;

public class HumanAnalysis : Procedure
{
    public string environment;
    public string filepath;
    public string sheetname;

    Input input;
    LoadInput loadInput;
    HumanEdgesAnalyse humanEdgesAnalyse;
    HumanVertexAnalyse humanVertexAnalyse;

    public HumanAnalysis(string environment, string outputPath)
    {
        this.environment = environment;
        this.filepath = outputPath + ".xlsx";
        this.sheetname = "分析人工分层";

        input = new Input(environment);
        loadInput = new LoadInput();
        humanEdgesAnalyse = new HumanEdgesAnalyse();
        humanVertexAnalyse = new HumanVertexAnalyse();
    }
    public override List<string> Description()
    {
        List<string> description = new List<string>
        {
            loadInput.ToString(),
            humanEdgesAnalyse.ToString(),
            humanVertexAnalyse.ToString(),
        };
        return description;
    }

    public override void Execute()
    {
        IEnumerable<Package> packages = loadInput.Process(input);
        Dictionary<(Package, Package), int> edges = humanEdgesAnalyse.Process(packages);
        Dictionary<Package,double> vertex = humanVertexAnalyse.Process(packages);
        Output.HumanAnalysisOutput(filepath, sheetname, Description(), vertex, edges);
    }
}