using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;
using CsvHelper;

namespace MyTest_Console.AppCode
{
    public class HanddayExcelParser
    {
        private string _filename = string.Empty;
        private List<string> _requiredCols = null;
        private DataTable _dataTable = null;
        private bool _required = false;
        public HanddayExcelParser(string filename)
        {
            _filename = filename;
        }

        public HanddayExcelParser(string filename, List<string> requiredCols)
        {
            _filename = filename;
            _requiredCols = requiredCols;
            _required = requiredCols != null && requiredCols.Count > 0;
        }

        private void ReadFile()
        {
            string sql = "select * from [$A1:IU65536]";// [Sheet1$]  [$A1:R65536]
            DataSet ds = new DataSet();
            OleDbConnection conn = null;
            try
            {
                string connStr = string.Format("Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=YES; IMEX=1'", _filename);
                conn = new OleDbConnection(connStr);
                conn.Open();
                OleDbDataAdapter cmd = new OleDbDataAdapter(sql, conn);
                ds = new DataSet("Data");
                cmd.Fill(ds);
            }
            finally
            {
                conn.Close();
            }
            if (ds.Tables.Count > 0)
            {
                _dataTable = ds.Tables[0];
                return;
            }
            throw new Exception("工作区不存在");
        }

        /// <summary>
        /// 验证默认列是否合法
        /// </summary>
        /// <returns></returns>
        private bool DefaultColumnsIsValidate()
        {
            if (_requiredCols != null && _requiredCols.Count > 0)
            {
                bool validate = true;
                foreach (var col in _dataTable.Columns)
                {
                    if (!_requiredCols.Contains(col))
                    {
                        validate = false;
                        break;
                    }
                }
                return validate;
            }
            return true;
        }

        private bool FirstRowIsValudate(Dictionary<int, string> dict)
        {
            if (dict != null && dict.Keys.Count > 0)
            {
                bool validate = true;
                foreach (var item in _requiredCols)
                {
                    if (!dict.Values.Contains(item))
                    {
                        validate = false;
                        break;
                    }
                }
                return validate;
            }
            return false;
        }

        /// <summary>
        /// 重新构建表格
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private DataTable ReBuildDataTable(Dictionary<int, string> dict)
        {
            if (_dataTable != null)
            {
                var dt = new DataTable();

                //构建列
                foreach (var item in dict)
                {
                    dt.Columns.Add(item.Value);
                }

                if (_dataTable.Rows.Count > 1)
                {

                    for (int i = 1; i < _dataTable.Rows.Count; i++)
                    {
                        DataRow row = _dataTable.Rows[i];
                        var new_row = dt.NewRow();
                        foreach (var item in dict)
                        {
                            new_row[item.Key] = row[item.Key];
                        }

                        dt.Rows.Add(new_row);
                    }
                }
                return dt;
            }
            return null;
        }

        /// <summary>
        /// 获取datatable第一行数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetDataTableFirstValue(DataTable dt)
        {
            var re = new Dictionary<int, string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    re.Add(i, row[i].ToString());
                }
            }
            return re;
        }

        public DataTable GetDataTable(out string errorMsg)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_filename) && System.IO.File.Exists(_filename))
                {
                    ReadFile();
                }
                else
                {
                    throw new Exception("导入文件不存在");
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return null;
            }

            if (_dataTable != null)
            {
                errorMsg = string.Empty;
                if (_required)
                {
                    var _isDef = DefaultColumnsIsValidate();
                    if (!_isDef)
                    {
                        var dict = GetDataTableFirstValue(_dataTable);

                        _isDef = FirstRowIsValudate(dict);
                        if (_isDef)
                        {
                            return ReBuildDataTable(dict);
                        }
                        else
                        {
                            errorMsg = "导入文件缺少必要列";
                        }
                    }
                }
                return _dataTable;
            }
            else
            {
                errorMsg = "导入的文件为空";
            }
            return _dataTable;
        }
    }

    public class HanddayCsvParser
    {
        private string _filename;
        private List<string> _requiredCols = null;
        private bool _required = false;
        public HanddayCsvParser(string filename)
        {
            _filename = filename;
        }

        public HanddayCsvParser(string filename, List<string> requiredCols)
        {
            _filename = filename;
            _requiredCols = requiredCols;
            _required = requiredCols != null && requiredCols.Count > 0;
        }




        public DataTable GetDataTable(out string errorMsg)
        {
            if (!string.IsNullOrWhiteSpace(_filename) && File.Exists(_filename))
            {
                errorMsg = string.Empty;
                var csvList = ParseCsvFile(_filename);
                if (csvList != null && csvList.Count > 0)
                {
                    var header = csvList[0];
                    bool _isDef = true;
                    int skip = 0;
                    if (_required)
                    {
                        _isDef = CheckColumnsIsValidate(header);
                        if (!_isDef && csvList.Count > 1)
                        {
                            header = csvList[1];
                            _isDef = CheckColumnsIsValidate(header);
                            if (_isDef)
                            {
                                skip = 2;
                            }
                        }
                    }

                    if (_isDef)
                    {
                        return ReadDataTableData(skip, header, csvList);
                    }
                    else
                    {
                        errorMsg = "缺少导入的必须列";
                        return null;
                    }
                }
                else
                {
                    errorMsg = "导入的文件为空";
                    return null;
                }
            }
            else
            {
                errorMsg = "导入的文件不存在";
                return null;
            }

        }

        private DataTable ReadDataTableData(int skip, string[] header, List<string[]> csvList)
        {
            var dt = BuildDataTable(header);

            if (csvList != null && csvList.Count > skip)
            {
                var data = csvList.Skip(skip);
                if (data != null && data.GetEnumerator().MoveNext())
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
            }
            return dt;
        }

        private bool CheckColumnsIsValidate(string[] header)
        {
            bool validate = true;
          
            foreach (var item in _requiredCols)
            {
                if (!header.Contains(item))
                {
                    validate = false;
                    break;
                }
            }
            return validate;
        }

        private static List<string[]> ParseCsvFile(string path)
        {
            List<string[]> result = null;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                result = new List<string[]>();
                using (TextReader reader = new StreamReader(path, Encoding.GetEncoding("gb2312")))
                {
                    var parser = new CsvParser(reader);

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


    }
}
