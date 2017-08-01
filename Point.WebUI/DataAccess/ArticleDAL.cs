using Point.Common.DBMaper;
using Point.Common.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Point.WebUI
{
    public class ArticleDAL : Point.Common.DataAccessBase
    {
        private static ArticleDAL _dal = null;
        public static ArticleDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new ArticleDAL();
                return _dal;
            }
        }

        public long Add(ArticleDetailInfo info)
        {
            var sql = @"insert into article 
                                (ThirdId,Title,Summary,Cover,CategoryId,ThirdCategoryId,CreateDate) 
                                values 
                                (@ThirdId,@Title,@Summary,@Cover,@CategoryId,@ThirdCategoryId,@CreateDate);
                                select last_insert_id(); ";

            long aid;
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "ThirdId", DbType.Int64, info.ThirdId);
                SetCommandParameter(cmd, "Title", DbType.String, info.Title);
                SetCommandParameter(cmd, "Summary", DbType.String, info.Summary);
                SetCommandParameter(cmd, "Cover", DbType.String, info.Cover);
                SetCommandParameter(cmd, "CategoryId", DbType.Int64, info.CategoryId);
                SetCommandParameter(cmd, "ThirdCategoryId", DbType.Int64, info.ThirdCategoryId);
                SetCommandParameter(cmd, "CreateDate", DbType.DateTime, info.CreateDate);

                aid = GetLong(cmd);
            }

            AddContent(aid, info.Content);

            return aid;
        }

        public void Add(IEnumerable<ArticleDetailInfo> datas)
        {

            if (!IsEmptyCollection(datas))
            {
                foreach (var item in datas)
                {
                    try
                    {
                        var articleId = Add(item);
                    }
                    catch (Exception ex)
                    {
                        Point.Common.Core.SystemLoger.Current.Write("ex:" + ex.Message + "  object:" + Newtonsoft.Json.JsonConvert.SerializeObject(item));
                    }
                }

            }
        }

        public IEnumerable<long> GetThirdIdList()
        {
            var sql = "select ThirdId from article where ThirdId is not null;";

            var dt = GetDataTable(sql);

            if (!IsEmptyDataTable(dt))
            {
                return dt.AsEnumerable().Select(i => Convert.ToInt64(i["ThirdId"]));
            }
            return null;
        }

        public long GetMaxThirdId(long thirdCateId)
        {
            var sql = "select Ifnull(Max(ThirdId),0) maxId from article where ThirdCategoryId=@ThirdCategoryId and  ThirdId is not null;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "ThirdCategoryId", DbType.Int64, thirdCateId);

                return GetLong(cmd);
            }

        }

        public IEnumerable<ArticleInfo> GetList(ArticleQueryFilter filter)
        {

            StringBuilder sbBuff = new StringBuilder();
            sbBuff.Append("select sql_calc_found_rows * from (");
            sbBuff.Append("select a.* from article a  where 1=1");


            if (filter.CategoryIds != null && filter.CategoryIds.GetEnumerator().MoveNext())
            {
                if (filter.CategoryIds.Count() > 1)
                    sbBuff.AppendFormat(" and a.CategoryId in ({0})", string.Join(",", filter.CategoryIds));
                else
                    sbBuff.AppendFormat(" and a.CategoryId={0}", filter.CategoryIds.ElementAt(0));
            }

            if (!string.IsNullOrWhiteSpace(filter.Keywords))
                sbBuff.AppendFormat(" and a.Title like '%{0}%'", filter.Keywords.ReplaceSqlInjectChar());

            if (filter.IsCover.HasValue)
            {
                if (filter.IsCover.Value)
                    sbBuff.Append(" and a.Cover is not null and length(a.Cover)>0");
                else
                    sbBuff.Append(" and a.Cover is null");
            }

            sbBuff.Append(" order by a.CreateDate desc");
            sbBuff.Append(") as temp");
            //分页
            sbBuff.Append(" limit " + filter.StartRowNo + "," + filter.PageSize + ";select found_rows();");

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sbBuff.ToString()))
            {

                var ds = GetDataSet(cmd);
                filter.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                return ds.Tables[0].ToList<ArticleInfo>();
            }
        }

        public ArticleDetailInfo Get(long articleId)
        {
            var sql = "select a.*,b.Content from article a inner join article_content b on a.Id=b.ArticleId where a.Id=@Id";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, articleId);

                return GetDataRow(cmd).Fill<ArticleDetailInfo>();
            }
        }

        private void AddContent(long articleId, string content)
        {
            var sql = "insert into article_content (ArticleId,Content) values (@ArticleId,@Content);";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "ArticleId", DbType.Int64, articleId);
                SetCommandParameter(cmd, "Content", DbType.String, content);


                ExecSql(cmd);
            }
        }
    }
}