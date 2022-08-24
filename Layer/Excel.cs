using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Layer
{
    public static class Excel
    {
        public static void Output(string filepath, NodeSet set)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(new FileInfo(filepath)))
            {
                var hasSheet = p.Workbook.Worksheets[set.algorithm];
                if (hasSheet != null)
                {
                    p.Workbook.Worksheets.Delete(set.algorithm);
                }
                ExcelWorksheet worksheetIn = p.Workbook.Worksheets.Add(set.algorithm);
                for (int i = 0; i < set.layerList.Count; i++)
                {
                    worksheetIn.Cells[1, i + 1].Value = "layer " + (i + 1).ToString();
                    int count = 2;
                    for (int j = 0; j < set.layerList[i].Count; j++)
                    {
                        string node = "";
                        foreach (Package package in set.layerList[i][j].packages)
                        {
                            node += package.name+";";
                        }
                        worksheetIn.Cells[count, i + 1].Value = node;
                        count++;
                    }
                }

                var hasSheet2 = p.Workbook.Worksheets[set.algorithm + "对比"];
                if (hasSheet2 != null)
                {
                    p.Workbook.Worksheets.Delete(set.algorithm + "对比");
                }
                ExcelWorksheet worksheetIn2 = p.Workbook.Worksheets.Add(set.algorithm+"对比");
                worksheetIn2.Cells[1, 1].Value = "包名";
                worksheetIn2.Cells[1, 2].Value = "算法分层层数";
                worksheetIn2.Cells[1, 3].Value = "人工分层层数";
                int count2 = 2;
                foreach (Package package in set.packages)
                {
                    worksheetIn2.Cells[count2, 1].Value = package.name;
                    worksheetIn2.Cells[count2, 2].Value = set.algorithmLayers[package];
                    worksheetIn2.Cells[count2, 3].Value = set.humanLayers.ContainsKey(package)? set.humanLayers[package] : "null";
                    worksheetIn2.Cells[count2, 4].Formula = String.Format("=IF(INT(B{0})=INT(C{1}),\"yes\",\"no\")",count2,count2);
                    count2++;
                }
                worksheetIn2.Cells[2, 5].Value = "yes";
                worksheetIn2.Cells[3, 5].Value = "no";
                worksheetIn2.Cells[2, 6].Formula = String.Format("=COUNTIF(D:D,\"yes\")");
                worksheetIn2.Cells[3, 6].Formula = String.Format("=COUNTIF(D:D,\"no\")");

                var hasSheet3 = p.Workbook.Worksheets[set.algorithm + "对比2"];
                if (hasSheet3 != null)
                {
                    p.Workbook.Worksheets.Delete(set.algorithm + "对比2");
                }
                ExcelWorksheet worksheetIn3 = p.Workbook.Worksheets.Add(set.algorithm + "对比2");
                for (int i = 0; i < set.layerList.Count; i++)
                {
                    worksheetIn3.Cells[1, i + 1].Value = "layer " + (i + 1).ToString();
                    int count = 2;
                    for (int j = 0; j < set.layerList[i].Count; j++)
                    {
                        foreach (Package package in set.layerList[i][j].packages)
                        {
                            worksheetIn3.Cells[count, i + 1].Value = set.humanLayers.ContainsKey(package) ? set.humanLayers[package] : "null";
                            count++;
                        }
                    }
                }
                
                p.Save();
            }
        }
    }
}
