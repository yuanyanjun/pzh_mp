﻿using Point.Common.DBMaper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Point.WebUI
{
    public class MpMenuDAL : Point.Common.DataAccessBase
    {
        private static MpMenuDAL _dal = null;
        public static MpMenuDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new MpMenuDAL();
                return _dal;
            }
        }

        public long Add(MpMenuLocationDetailsInfo info)
        {
            var sqlTxt = @"insert into mp_menu (ParentId,Name,`Type`,`Key`,`Url`,SortOrder) 
                                     values
                                    (@ParentId,@Name,@Type,@Key,@Url,@SortOrder);
                                     select last_insert_id();";

            var sortOrder = GetSortOrder(info.ParentId);
            long id;
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "ParentId", DbType.Int64, info.ParentId);
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "Type", DbType.String, info.Type);
                SetCommandParameter(cmd, "Key", DbType.String, info.Key);
                SetCommandParameter(cmd, "Url", DbType.String, info.Url);
                SetCommandParameter(cmd, "SortOrder", DbType.Int32, sortOrder);

                id = GetLong(cmd);
            }

            if (!IsEmptyCollection(info.CategoryIds))
            {
                sqlTxt = "insert into mp_menu_relation (MenuId,CategoryId) values ";
                var buf = new List<string>();
                foreach (var item in info.CategoryIds)
                {
                    buf.Add(string.Format("({0},{1})", id, item));
                }

                sqlTxt += string.Join(",", buf);

                ExecSql(sqlTxt);
            }

            return id;
        }

        public void Edit(MpMenuLocationDetailsInfo info)
        {
            var sqlTxt = @"update mp_menu set Name=@Name,`Type`=@Type,`Key`=@Key,`Url`=@Url where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, info.Id);
                SetCommandParameter(cmd, "Name", DbType.String, info.Name);
                SetCommandParameter(cmd, "Type", DbType.String, info.Type);
                SetCommandParameter(cmd, "Key", DbType.String, info.Key);
                SetCommandParameter(cmd, "Url", DbType.String, info.Url);
                ExecSql(cmd);
            }

            if (!IsEmptyCollection(info.CategoryIds))
            {
                sqlTxt = @"delete from mp_menu_relation where MenuId="+info.Id+";"+
                                  "insert into mp_menu_relation (MenuId,CategoryId) values ";
                var buf = new List<string>();
                foreach (var item in info.CategoryIds)
                {
                    buf.Add(string.Format("({0},{1})", info.Id, item));
                }

                sqlTxt += string.Join(",", buf);

                ExecSql(sqlTxt);
            }
        }
        public void Remove(long id)
        {
            var sqlTxt = @"delete from mp_menu where Id=@Id;
                                    delete from mp_menu_relation where MenuId=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                ExecSql(cmd);
            }
        }

        public MpMenuLocationInfo Get(long id)
        {
            var sqlTxt = "select * from mp_menu where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                return GetDataRow(cmd).Fill<MpMenuLocationInfo>();
            }
        }

        public MpMenuLocationDetailsInfo GetDetails(long id)
        {
            var data = Get(id).Copy<MpMenuLocationDetailsInfo>();

            if (data != null)
            {
                var sqlTxt = "select CategoryId from mp_menu_relation where MenuId=@MenuId;";

                using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
                {
                    SetCommandParameter(cmd, "MenuId", DbType.Int64, id);

                    var dt = GetDataTable(cmd);

                    if (!IsEmptyDataTable(dt))
                        data.CategoryIds = dt.AsEnumerable().Select(i => Convert.ToInt64(i[0]));
                }
            }
            return data;
        }

        public IEnumerable<MpMenuLocationInfo> GetList()
        {
            var sqlTxt = "select * from mp_menu order by SortOrder asc;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                return GetDataTable(cmd).ToList<MpMenuLocationInfo>();
            }
        }

        public bool ExistChildMenu(long parnetId)
        {
            var sqlTxt = "select count(1) from mp_menu where ParentId=@ParentId;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "ParentId", DbType.Int64, parnetId);
                return GetInt(cmd) > 0;
            }
        }

        public IEnumerable<CategoryInfo> GetRelationCategoryListByMenuId(long mid)
        {
            var sqlTxt = @"select b.* from mp_menu_relation a inner join category b on a.CategoryId=b.Id
                                    where a.MenuId=@MenuId;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "MenuId", DbType.Int64, mid);
                return GetDataTable(cmd).ToList<CategoryInfo>();
            }
        }

        private int GetSortOrder(long? parentId)
        {
            var sqlTxt = "select ifnull(max(SortOrder),0)+1 from mp_menu where 1=1";

            if (parentId.HasValue)
                sqlTxt += " and ParentId=" + parentId;

            return GetInt(sqlTxt);
        }
    }
}