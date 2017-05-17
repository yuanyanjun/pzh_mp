using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Data;
using CsvHelper.Configuration;

namespace MyTest_Console
{
    public class CsvManager
    {


        public static List<string[]> ParseCsvFile(string path)
        {
            List<string[]> result = null;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                result = new List<string[]>();
                using (TextReader reader = new StreamReader(path, Encoding.GetEncoding("gb2312")))
                {
                    var parser = new CsvParser(reader);
                    parser.Configuration.SkipEmptyRecords = true;
                    while (true)
                    {
                        var row = parser.Read();
                        if (row == null)
                            break;

                        result.Add(row);
                    }
                    
                }
            }
            return result;
        }

        public static DataTable PaserCsvFileToDataTable(string path)
        {
            var csvList = ParseCsvFile(path);
            if (csvList != null && csvList.Count > 0)
            {
                var head = csvList.ElementAt(0);

                DataTable dt = BuildDataTable(head);

                var data = csvList.Skip(1);
                if (dt != null && data != null && data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        var count = item.Length;
                        var d_count = dt.Columns.Count;

                        var row = dt.NewRow();
                        for (int i = 0; i < d_count; i++)
                        {
                            if (i > count - 1)
                            {
                                row[i] = string.Empty;
                            }
                            else
                            {
                                row[i] = item[i];
                            }
                        }

                        dt.Rows.Add(row);
                    }
                }

                return dt;
            }

            return null;
        }

        private static DataTable BuildDataTable(string[] head)
        {
            if (head != null && head.Length > 0)
            {
                var dt = new DataTable();

                for (int i = 0; i < head.Length; i++)
                {
                    var name = head[i];
                    if (string.IsNullOrWhiteSpace(name))
                        continue;
                    var exist = dt.Columns.Contains(name);
                    DataColumn col = new DataColumn()
                    {
                        ColumnName = exist ? (name + i.ToString()) : name
                    };

                    dt.Columns.Add(col);
                }

                return dt;
            }
            return null;
        }


        public static string WriteCsvFile(List<List<string>> list)
        {
            
            var textWriter = new StringWriter();
            var csv = new CsvWriter(textWriter);

            foreach (var item in list)
            {
                if (item != null && item.Count > 0)
                {
                    foreach (var f in item)
                    {
                        csv.WriteField(f);
                        
                    }
                    csv.NextRecord();
                }
               
            }

            string str = textWriter.ToString();//将流中数据写到字符串中
            textWriter.Close();

            return str;
        }
    }
}
