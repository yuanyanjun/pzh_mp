using Point.Common.DBMaper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Point.WebUI
{
    public class AutoCaptureDAL : Point.Common.DataAccessBase
    {
        private static AutoCaptureDAL _dal = null;
        public static AutoCaptureDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new AutoCaptureDAL();
                return _dal;
            }
        }

        public long Add(AutoCaptureInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            var sqlTxt = @"insert into auto_capture_config (Name,CategoryId,ThridCategoryId,ListUrl,ListXPath,DetailUrl,DetailXpath,LinkBaseUrl)
                                      values
                                      (@Name,@CategoryId,@ThridCategoryId,@ListUrl,@ListXPath,@DetailUrl,@DetailXpath,@LinkBaseUrl);
                                      select last_insert_id();";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "CategoryId", DbType.Int64, info.CategoryId);
                SetCommandParameter(cmd, "ThridCategoryId", DbType.UInt16, info.ThridCategoryId);
                SetCommandParameter(cmd, "ListUrl", DbType.String, info.ListUrl);
                SetCommandParameter(cmd, "ListXPath", DbType.String, info.ListXPath);
                SetCommandParameter(cmd, "DetailUrl", DbType.String, info.DetailUrl);
                SetCommandParameter(cmd, "DetailXpath", DbType.String, info.DetailXpath);
                SetCommandParameter(cmd, "LinkBaseUrl", DbType.String, info.LinkBaseUrl);
                return GetLong(cmd);
            }
        }

        public void Edit(AutoCaptureInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (!info.Id.HasValue)
                throw new Exception("id不能为空");

            var sqlTxt = @"update auto_capture_config set
                                     Name=@Name,
                                     CategoryId=@CategoryId,
                                     ThridCategoryId=@ThridCategoryId,
                                     ListUrl=@ListUrl,
                                     ListXPath=@ListXPath,
                                     DetailUrl=@DetailUrl,
                                     DetailXpath=@DetailXpath,
                                     LinkBaseUrl=@LinkBaseUrl
                                     where Id=@Id";

            var sql = string.Format("select CategoryId from auto_capture_config where Id={0}", info.Id);
            var cid = GetNullAbleLong(sql);


            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "CategoryId", DbType.Int64, info.CategoryId);
                SetCommandParameter(cmd, "ThridCategoryId", DbType.Int64, info.ThridCategoryId);
                SetCommandParameter(cmd, "ListUrl", DbType.String, info.ListUrl);
                SetCommandParameter(cmd, "ListXPath", DbType.String, info.ListXPath);
                SetCommandParameter(cmd, "DetailUrl", DbType.String, info.DetailUrl);
                SetCommandParameter(cmd, "DetailXpath", DbType.String, info.DetailXpath);
                SetCommandParameter(cmd, "LinkBaseUrl", DbType.String, info.LinkBaseUrl);

                SetCommandParameter(cmd, "Id", DbType.Int64, info.Id);
                ExecSql(cmd);
            }

            if (cid != info.CategoryId)
            {
                sql = string.Format("update article set CategoryId={0} where ThirdCategoryId={1};",
                    info.CategoryId.HasValue ? info.CategoryId.ToString() : "null",
                    info.ThridCategoryId);

                ExecSql(sql);
            }
        }

        public void Remove(long id)
        {
            var sqlTxt = "delete from auto_capture_config where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

            }
        }

        public void SetStatus(long id, AutoCatureStatus status)
        {
            var sql = "update auto_capture_config set Status=@Status where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);
                SetCommandParameter(cmd, "Status", DbType.Int32, (int)status);

                ExecSql(cmd);
            }
        }

        public AutoCaptureInfo Get(long id)
        {
            var sql = @"select * from auto_capture_config where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                return GetDataRow(cmd).Fill<AutoCaptureInfo>();
            }
        }

        public IEnumerable<AutoCaptureInfo> GetList()
        {
            var sql = @"select a.*,b.Name as CategoryName from auto_capture_config a 
                                left join category b on a.CategoryId=b.Id;";

            return GetDataTable(sql).ToList<AutoCaptureInfo>();
        }
    }
}