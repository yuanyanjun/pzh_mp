using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using Point.Common.DBMaper;
using Point.Common.Util;

namespace Point.WebUI
{
    public class DAL : Point.Common.DataAccessBase
    {
        private static DAL _dal = null;
        public static DAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new DAL();
                return _dal;
            }
        }
        public long InsertArticle(ArticleInfo info)
        {
            var sql = @"insert into pzh_article (RefId,Title,Cover,Type,CreateDate) values (@RefId,@Title,@Cover,@Type,@CreateDate);
                                select last_insert_id(); ";
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "RefId", DbType.Int64, info.RefId);
                SetCommandParameter(cmd, "Title", DbType.String, info.Title);
                SetCommandParameter(cmd, "Cover", DbType.String, info.Cover);
                SetCommandParameter(cmd, "Type", DbType.Int32, info.ArticleType);
                SetCommandParameter(cmd, "CreateDate", DbType.DateTime, info.CreateDate);


                return GetLong(cmd);
            }
        }


        public void InsertArticleAnnex(long articleId, IEnumerable<string> annexs)
        {
            if (!IsEmptyCollection(annexs))
            {
                var sql = "insert into pzh_article_annex (ArticleId,AnnexPath) values ";

                var buf = new List<string>();
                foreach (var item in annexs)
                {
                    buf.Add(string.Format("({0},'{1}')", articleId, item));
                }

                sql += string.Join(",", buf);

                ExecSql(sql);
            }
        }

        public void InsertArticleContent(long articleId, string content)
        {
            var sql = "insert into pzh_article_content (ArticleId,Content) values (@ArticleId,@Content);";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "ArticleId", DbType.Int64, articleId);
                SetCommandParameter(cmd, "Content", DbType.String, content);


                ExecSql(cmd);
            }
        }

        public void InsertArticle(IEnumerable<ArticleDetailInfo> datas)
        {

            if (!IsEmptyCollection(datas))
            {
                foreach (var item in datas)
                {
                    try
                    {
                        var articleId = InsertArticle(item);

                        InsertArticleContent(articleId, item.Content);



                        if (item.AnnexList != null && item.AnnexList.Count > 0)
                        {
                            InsertArticleAnnex(articleId, item.AnnexList);
                        }
                    }
                    catch (Exception ex)
                    {
                        Point.Common.Core.SystemLoger.Current.Write("ex:" + ex.Message + "  object:" + Newtonsoft.Json.JsonConvert.SerializeObject(item));
                    }
                }

            }
        }


        public IEnumerable<ArticleConfigInfo> SelectConfigList()
        {
            var sql = @"select * from  pzh_article_config;";

            return GetDataTable(sql).ToList<ArticleConfigInfo>();
        }

        public ArticleConfigInfo SelectConfig(long refId)
        {
            var sql = @"select * from  pzh_article_config where RefId=@RefId;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "RefId", DbType.Int64, refId);

                return GetDataRow(cmd).Fill<ArticleConfigInfo>();
            }

        }

        public long SelectMaxRefId(long acticleType)
        {
            var sql = @"select ifnull(Max(RefId),0) from  pzh_article where Type=@ActicleType;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "ActicleType", DbType.Int64, acticleType);

                return GetLong(cmd);
            }


        }

        public IEnumerable<ArticleInfo> SelectArticleList(ArticleQueryFilter filter)
        {
            StringBuilder sbBuff = new StringBuilder();
            sbBuff.Append("select sql_calc_found_rows * from (");
            sbBuff.Append("select a.* from pzh_article a  where 1=1");

            if (filter.ArticleType.HasValue)
                sbBuff.Append(" and a.Type=@ArticleType");

            if (filter.ArticleTypeIds != null && filter.ArticleTypeIds.GetEnumerator().MoveNext())
            {
                if (filter.ArticleTypeIds.Count() > 1)
                    sbBuff.AppendFormat(" and a.Type in ({0})", string.Join(",", filter.ArticleTypeIds));
                else
                    sbBuff.AppendFormat(" and a.Type={0}", filter.ArticleTypeIds.ElementAt(0));
            }

            if (!string.IsNullOrWhiteSpace(filter.Keywords))
                sbBuff.AppendFormat(" and a.Title like %{0}%", filter.Keywords.ReplaceSqlInjectChar());

            if (filter.IsCover.HasValue)
            {
                if (filter.IsCover.Value)
                    sbBuff.Append(" and a.Cover is not null and length(a.Cover)>0");
                else
                    sbBuff.Append(" and a.Cover is null");
            }

            sbBuff.Append(" order by a.RefId desc");
            sbBuff.Append(") as temp");
            //分页
            sbBuff.Append(" limit " + filter.StartRowNo + "," + filter.PageSize + ";select found_rows();");

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sbBuff.ToString()))
            {
                if (filter.ArticleType.HasValue)
                    SetCommandParameter(cmd, "ArticleType", DbType.Int64, filter.ArticleType);

                var ds = GetDataSet(cmd);
                filter.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                return ds.Tables[0].ToList<ArticleInfo>();
            }
        }

        public ArticleDetailInfo SelectArticleDetails(long articleId)
        {
            var sql = "select a.*,b.Content from pzh_article a inner join pzh_article_content b on a.Id=b.ArticleId where a.Id=@Id";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, articleId);

                return GetDataRow(cmd).Fill<ArticleDetailInfo>();
            }
        }

        #region 菜单

        public IEnumerable<MpMenuItem> SelectMenuList()
        {
            var sqlTxt = "select * from pzh_mp_menu order by SortOrder asc;";

            return GetDataTable(sqlTxt).ToList<MpMenuItem>();
        }
        #endregion
    }
}