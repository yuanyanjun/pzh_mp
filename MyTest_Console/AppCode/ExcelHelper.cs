using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace MyTest_Console.AppCode
{
    public class ExcelHelper
    {

        public static void ReadExcel(string path, bool firstHeader = true)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                string connStr = string.Format("Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR={1}; IMEX=1'", path, firstHeader ? "YES" : "no");
                OleDbConnection conn = new OleDbConnection(connStr);
                string sql = "select * from [$A1:IU65536]";// [Sheet1$]  [$A1:R65536]
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    OleDbDataAdapter cmd = new OleDbDataAdapter(sql, conn);
                    ds = new DataSet("Data");
                    cmd.Fill(ds);
                }
                finally
                {
                    conn.Close();
                }
            }
        }


    }
}
