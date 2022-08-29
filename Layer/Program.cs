using Layer;
using System.Text.RegularExpressions;

# region input
//Build Graph
string env = "mysql";
//"glibc", "basesystem", "filesystem", "setup", "anolis-release", "coreutils", "gcc", "systemd" 
List<string> exclude = new List<string> { };
List<string> excludeIn = new List<string> { };
List<string> excludeOut = new List<string> { };
string output_prefix = "single_edge_merge_threshold";
string excel = output_prefix+"_"+env+".xlsx";
//Console.WriteLine("0：原始算法；1：迭代分层；2：改进层间依赖；3：最大深度");
//string? input = Console.ReadLine();
//int algorithm = input == null ? 0 : int.Parse(input);

Console.WriteLine("0：正向(从最上层的包开始)；1：反向(从最底层的包开始)");
string? input = Console.ReadLine();
int t = int.Parse(input);
int direction = t == 0 ? 0 : 1;

string Format(string s)
{
    return s.Trim().Trim('"', '“', '”', '‘', '’', '\'').Trim();
}

NodeSet set = new NodeSet();
try
{
    using (StreamReader sr = new StreamReader("input_new_"+env+".txt"))
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            //Console.WriteLine(line);
            line = line.Trim();
            if (!line.Contains("->"))
            {
                continue;
            }
            string[] sArray = Regex.Split(line, "->");
            for (int i = 0; i < sArray.Length; i++)
            {
                sArray[i] = Format(sArray[i]);
            }
            if (exclude.Contains(sArray[0]) || exclude.Contains(sArray[1]))
                continue;
            if (!excludeIn.Contains(sArray[0]) && excludeIn.Contains(sArray[1]))
                continue;
            if (direction == 0)
                set.addDependency(sArray[1], sArray[0]);
            else
                set.addDependency(sArray[0], sArray[1]);
        }
    }
    using (StreamReader sr = new StreamReader("human_" + env + ".txt"))
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            //Console.WriteLine(line);
            line = line.Trim();
            string[] sArray = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
            for (int i = 0; i < sArray.Length; i++)
            {
                sArray[i] = Format(sArray[i]);
            }
            int l;
            if(int.TryParse(sArray[2], out l))
                set.addHumanLayer(sArray[0], l);
        }
    }
    using (StreamReader sr = new StreamReader("category_" + env + ".txt"))
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            //Console.WriteLine(line);
            line = line.Trim();
            string[] sArray = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
            for (int i = 0; i < sArray.Length; i++)
            {
                sArray[i] = Format(sArray[i]);
            }
            set.addpackageCategory(sArray[0], sArray[1]);
        }
    }
}
catch (Exception e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
}
# endregion

var temp = set.humanLayers.Keys.Except(set.packages);

//set.dfs_circle_count();
//set.buildEdgeSet();
Console.WriteLine(set.getNode("glibc").dependency.Remove(new Package("libselinux")));
//set.removeKeyEdges(60000);

# region Output ciecle edges count
//using (StreamWriter sw = new StreamWriter("circles_" + env + ".txt"))
//{
//    foreach (var edge in set.circleEdgeCount)
//    {
//        string cs = edge.Item1.name + "->" + edge.Item2.name + " " + edge.Item3;
//        sw.WriteLine(cs);
//    }
//    sw.Flush();
//}
# endregion

//DFS
set.dfs();

void printLayers(List<List<Node>> layerList)
{
    for (int i = 0; i < layerList.Count; i++)
    {
        Console.WriteLine("-----------------------------");
        Console.WriteLine("Layer : " + i);
        foreach (Node node in layerList[i])
            Console.WriteLine(node.ToString());
    }
}


#region origin
void origin(NodeSet set)
{
    var topoList = set.generateTopoList();
    //foreach (var n in topoList)
    //{
    //    Console.WriteLine(n);
    //}
    var layerList = set.generateLayer(topoList);
    set.genAlgorithmLayer(layerList);
    set.algorithm = "原始复现";
    set.layerList = layerList;
    Excel.Output(excel, set);
    //printLayers(layerList);
}
#endregion

# region 迭代
void iterate(NodeSet set)
{
    var topoList = set.generateTopoList();
    var layerList = set.generateLayer(topoList);
    int layerCount = 0;
    while (layerList.Count != layerCount)
    {
        layerCount = layerList.Count;
        var nextLayerList = new List<List<Node>>();
        for (int i = 0; i < layerList.Count; i++)
        {
            var subTopoList = set.generateTopoList(layerList[i].ToHashSet());
            var subLayerList = set.generateLayer(subTopoList);
            foreach (List<Node> layer in subLayerList)
            {
                nextLayerList.Add(layer);
            }
        }
        layerList = nextLayerList;
    }
    //layerList = set.mergeLayerWithoutZero(layerList, set.nodeSet.Values.ToHashSet().Count / 2);
    layerList = set.mergeLayerv2WithoutZero(layerList);
    //layerList = set.mergeLayerv2Max(layerList);

    set.genAlgorithmLayer(layerList);
    set.algorithm = "迭代分层";
    set.layerList = layerList;
    Excel.Output(excel, set);
    //printLayers(layerList);
}
# endregion

#region 改进型依赖
void improved(NodeSet set)
{
    var topoList = set.generateTopoList(set.nodeSet.Values.ToHashSet());
    //var testIn = set.generateLayerIn(topoList);
    var layerList = set.generateNewLayers(topoList);
    //layerList = set.mergeLayerWithoutZero(layerList, set.nodeSet.Values.ToHashSet().Count / 2);
    layerList = set.mergeLayerv2WithoutZero(layerList);
    //layerList = set.mergeLayerv2Max(layerList);

    set.genAlgorithmLayer(layerList);
    set.algorithm = "改进依赖";
    set.layerList = layerList;
    Excel.Output(excel, set);
    //printLayers(layerList);
}
#endregion

#region MaxDepth
void MaxDepth(NodeSet set)
{
    var layerList = set.generateLayerBasedMaxDepth();
    //layerList = set.mergeLayerWithoutZero(layerList, set.nodeSet.Values.ToHashSet().Count / 2);
    layerList = set.mergeLayerv2WithoutZero(layerList);
    //layerList = set.mergeLayerv2Max(layerList);
    //set.relayerCirclePackage(layerList);

    set.genAlgorithmLayer(layerList);
    set.algorithm = "最大深度";
    set.layerList = layerList;
    Excel.Output(excel, set);
    //printLayers(layerList);
}
#endregion


//switch (algorithm)
//{
//    case 0:
//        origin(set);
//        break;
//    case 1:
//        iterate(set);
//        break;
//    case 2:
//        improved(set);
//        break;
//    case 3:
//        MaxDepth(set);
//        break;
//    default:
//        break;
//}

origin(set);
iterate(set);
improved(set);
MaxDepth(set);
Console.WriteLine();