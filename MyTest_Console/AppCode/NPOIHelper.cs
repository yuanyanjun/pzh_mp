using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
namespace MyTest_Console
{
    public class NPOIHelper
    {
        public static DataTable ReadExcel(string file)
        {
            if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
            {
                DataTable dt = new DataTable();

                IWorkbook workbook;
                var extend = Path.GetExtension(file);

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (extend.ToLower().Contains("xlsx"))
                        workbook = new XSSFWorkbook(fs);
                    else
                        workbook = new HSSFWorkbook(fs);
                }

                ISheet sheet = workbook.GetSheetAt(0);
                var rows = sheet.GetRowEnumerator();

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                for (int i = 0; i < cellCount; i++)
                {
                    ICell cell = headerRow.GetCell(i);
                    dt.Columns.Add(cell.ToString());
                }

                for (int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    var datarow = dt.NewRow();


                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell != null)
                        {
                            var cellStyle = cell.CellStyle;

                            datarow[j] = cell.ToString();
                        }
                        else
                        {
                            datarow[j] = null;
                        }
                    }

                    dt.Rows.Add(datarow);
                }

                return dt;
            }

            return null;
        }
    }
}
