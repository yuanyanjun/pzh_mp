using Point.Common.DBMaper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Point.WebUI
{
    public class CategoryDAL : Point.Common.DataAccessBase
    {
        private static CategoryDAL _dal = null;
        public static CategoryDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new CategoryDAL();
                return _dal;
            }
        }

        public long Add(CategoryInfo info)
        {
            var sqlTxt = @"insert into category (Name,SortOrder) values
                             (@Name,SortOrder);select last_insert_id();";

            if (string.IsNullOrWhiteSpace(info.Name))
                throw new Exception("栏目名称不能为空");

            if (GetCountByName(info.Name, null) > 0)
                throw new Exception("栏目名称已存在");

            info.SortOrder = GetSortOrder();
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "SortOrder", DbType.Int32, info.SortOrder);

                return GetLong(cmd);
            }
        }

        public void Edit(CategoryInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.Name))
                throw new Exception("栏目名称不能为空");

            if (GetCountByName(info.Name, info.Id) > 0)
                throw new Exception("栏目名称已存在");

            var sqlTxt = "update category set Name=@Name where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "Id", DbType.Int64, info.Id);

                ExecSql(cmd);
            }
        }

        public IEnumerable<CategoryInfo> GetList()
        {
            var sqlTxt = "select * from category;";

            return GetDataTable(sqlTxt).ToList<CategoryInfo>();
        }

        public CategoryInfo Get(long id)
        {
            var sqlTxt = "select * from category where Id=@Id;";


            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                return GetDataRow(cmd).Fill<CategoryInfo>();
            }
        }

        private int GetSortOrder()
        {
            var sqlTxt = @"select ifnull(max(SortOrder),0)+1 from category;";

            return GetInt(sqlTxt);
        }
        private int GetCountByName(string name, long? id)
        {
            var sqlTxt = "select Count(1) from category where Name=@Name ";

            if (id.HasValue)
                sqlTxt += " and Id<>@Id";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Name", DbType.String, name);
                if (id.HasValue)
                    SetCommandParameter(cmd, "Id", DbType.Int64, id);

                return GetInt(cmd);
            }
        }
    }
}