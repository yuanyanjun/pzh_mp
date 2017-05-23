using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Web.Helpers;

namespace MyTest_Console
{
    class Program
    {
        static void Main(string[] args)
        {

            //WebImage webImg = new WebImage("http://files.handday.cn:9020/74010/201705/8c5a40c8-a60d-456d-92c8-98fe7c9df303.jpg");
            
            Console.ReadKey();
        }

        private static void ParseLanguage(string language, ref List<string> data)
        {
            if (data == null)
                data = new List<string>();

            if (data.Count >= 3)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                var pos = language.IndexOf(",");
                if (pos != -1)
                {
                    data.Add(language.Substring(0, pos));

                    ParseLanguage(language.Substring(pos + 1), ref data);
                }
                else
                {
                    data.Add(language);
                }
            }
        }

        public static DataTable FromXLS(string file)
        {
            string connStr = string.Format("Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=YES; IMEX=1'", file);
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
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }

            return null;
        }

        public static void SetDataRowValue(DataRow row, string cloumnName, string value,ref DataTable dt)
        {
            if (!string.IsNullOrWhiteSpace(cloumnName))
            {
               
                if (row != null)
                {
                    var newRow = dt.NewRow();
                    newRow.ItemArray = row.ItemArray;

                    if (!newRow.Table.Columns.Contains(cloumnName))
                        newRow.Table.Columns.Add(cloumnName);

                    newRow[cloumnName] = value;

                    dt.Rows.Add(newRow);
                }
            }
        }
    }
}
