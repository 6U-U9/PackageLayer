using OfficeOpenXml;

namespace Refactor.Core
{
    public class Output
    {
        public static void HierarchiesOutput(string filepath, string sheetPrefix, List<string> descriptions, Hierarchies hierarchies)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(new FileInfo(filepath)))
            {
                {
                    var sheetName = sheetPrefix;
                    var hasSheet = p.Workbook.Worksheets[sheetName];
                    if (hasSheet != null)
                    {
                        p.Workbook.Worksheets.Delete(sheetName);
                    }
                    ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(sheetName);
                    for (int i = 0; i < hierarchies.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = "layer " + (i + 1).ToString();
                        int count = 2;
                        for (int j = 0; j < hierarchies[i].Count; j++)
                        {
                            worksheet.Cells[count, i + 1].Value = hierarchies[i][j].ToString();
                            count++;
                        }
                    }
                }

                {
                    var sheetName = sheetPrefix + "说明";
                    var hasSheet = p.Workbook.Worksheets[sheetName];
                    if (hasSheet != null)
                    {
                        p.Workbook.Worksheets.Delete(sheetName);
                    }
                    ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(sheetName);
                    worksheet.Cells[1, 1].Value = "处理步骤";
                    worksheet.Cells[1, 2].Value = "说明";
                    int count = 2;
                    for (int i = 0; i < descriptions.Count; i++)
                    {
                        worksheet.Cells[count, 1].Value = i+1;
                        worksheet.Cells[count, 2].Value = descriptions[i];
                        count++;
                    }
                }

                {
                    var sheetName = sheetPrefix + "对比";
                    var hasSheet = p.Workbook.Worksheets[sheetName];
                    if (hasSheet != null)
                    {
                        p.Workbook.Worksheets.Delete(sheetName);
                    }
                    ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(sheetName);
                    worksheet.Cells[1, 1].Value = "包名";
                    worksheet.Cells[1, 2].Value = "算法分层层数";
                    worksheet.Cells[1, 3].Value = "人工分层层数";
                    worksheet.Cells[1, 4].Value = "相同";
                    int count = 2;
                    for (int i = 0; i < hierarchies.Count; i++)
                    {
                        for (int j = 0; j < hierarchies[i].Count; j++)
                        {
                            foreach (Package package in hierarchies[i][j].packages)
                            {
                                worksheet.Cells[count, 1].Value = package.ToString();
                                worksheet.Cells[count, 2].Value = i + 1;
                                worksheet.Cells[count, 3].Value = package.human == -1 ? "null" : package.human;
                                worksheet.Cells[count, 4].Formula = String.Format("=IF(INT(B{0})=INT(C{1}),\"yes\",\"no\")", count, count);
                                count++;
                            }
                        }
                    }
                    
                    worksheet.Cells[2, 5].Value = "yes";
                    worksheet.Cells[3, 5].Value = "no";
                    worksheet.Cells[2, 6].Formula = String.Format("=COUNTIF(D:D,\"yes\")");
                    worksheet.Cells[3, 6].Formula = String.Format("=COUNTIF(D:D,\"no\")");
                }

                {
                    var sheetName = sheetPrefix + "对比2";
                    var hasSheet = p.Workbook.Worksheets[sheetName];
                    if (hasSheet != null)
                    {
                        p.Workbook.Worksheets.Delete(sheetName);
                    }
                    ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(sheetName);
                    for (int i = 0; i < hierarchies.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = "layer " + (i + 1).ToString();
                        int count = 2;
                        for (int j = 0; j < hierarchies[i].Count; j++)
                        {
                            foreach (Package package in hierarchies[i][j].packages)
                            {
                                worksheet.Cells[count, i + 1].Value = package.human == -1 ? "null" : package.human;
                                count++;
                            }
                        }
                    }
                }
                p.Save();
            }
        }
        public static void EdgesOutput(string filepath, string sheetPrefix, List<string> descriptions, List<(Package,Package,int)> output)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(new FileInfo(filepath)))
            {
                {
                    var sheetName = sheetPrefix;
                    var hasSheet = p.Workbook.Worksheets[sheetName];
                    if (hasSheet != null)
                    {
                        p.Workbook.Worksheets.Delete(sheetName);
                    }
                    ExcelWorksheet worksheet = p.Workbook.Worksheets.Add(sheetName);
                    worksheet.Cells[1, 1].Value = "边";
                    worksheet.Cells[1, 3].Value = "环的数量";
                    int count = 2;
                    foreach (var edge in output)
                    {
                        worksheet.Cells[count, 1].Value = edge.Item1.ToString();
                        worksheet.Cells[count, 2].Value = edge.Item2.ToString();
                        worksheet.Cells[count, 3].Value = edge.Item3.ToString();
                        count++;
                    }
                }

                p.Save();
            }
        }
    }
}
