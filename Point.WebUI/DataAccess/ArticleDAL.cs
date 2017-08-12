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
                var addList = datas.Where(i => !i.Id.HasValue);
                var editList = datas.Where(i => i.Id.HasValue);

                if (addList != null && addList.GetEnumerator().MoveNext())
                {
                    foreach (var item in addList)
                    {
                        try
                        {
                            var articleId = Add(item);
                        }
                        catch (Exception ex)
                        {
                            Point.Common.Core.SystemLoger.Current.Write("add article error, ex:" + ex.Message + "  object:" + Newtonsoft.Json.JsonConvert.SerializeObject(item));
                        }
                    }
                }

                if (editList != null && editList.GetEnumerator().MoveNext())
                {
                    foreach (var item in editList)
                    {
                        try
                        {
                            Edit(item);
                        }
                        catch (Exception ex)
                        {
                            Point.Common.Core.SystemLoger.Current.Write("edit article error, ex:" + ex.Message + "  object:" + Newtonsoft.Json.JsonConvert.SerializeObject(item));
                        }
                    }
                }
            }
        }

        public void Edit(ArticleDetailInfo info)
        {

            if (info == null)
                throw new ArgumentNullException("info");

            if (string.IsNullOrWhiteSpace(info.Title))
                throw new Exception("标题不能为空");

            if (string.IsNullOrWhiteSpace(info.Content))
                throw new Exception("内容不能为空");

            var sqlTxt = @"update article 
                            set Title=@Title,
                                CategoryId=@CaetegoryId,
                                ThirdCategoryId=@ThirdCategoryId,
                                Cover=@Cover
                            where Id=@Id;
                          update article_content Content=@Content where ArticleId=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Title", DbType.String, info.Title);
                SetCommandParameter(cmd, "Content", DbType.String, info.Content);
                SetCommandParameter(cmd, "Cover", DbType.String, info.Cover);
                SetCommandParameter(cmd, "CategoryId", DbType.Int64, info.CategoryId);
                SetCommandParameter(cmd, "ThirdCategoryId", DbType.Int64, info.ThirdCategoryId);
                SetCommandParameter(cmd, "Id", DbType.Int64, info.Id);

                ExecSql(cmd);
            }
        }

        public void Remove(long id)
        {
            var sqlTxt = @"delete from article where Id=@id;
                                     delete from article_content where ArticleId=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                ExecSql(cmd);
            }
        }

        public void Remove(long? categoryId, long thirdCategoryId)
        {

            var sqlTxt = @"delete from article_content where ArticleId in (
                                select Id from article where ThirdCategoryId=@ThirdCategoryId {CategoryIdFilter}
                                ); 
                                delete from article where ThirdCategoryId=@ThirdCategoryId {CategoryIdFilter};";

            if (categoryId.HasValue)
                sqlTxt = sqlTxt.Replace("{CategoryIdFilter}", " and CategoryId=" + categoryId);
            else
                sqlTxt = sqlTxt.Replace("{CategoryIdFilter}", string.Empty);
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "ThirdCategoryId", DbType.Int64, thirdCategoryId);

                ExecSql(cmd);
            }
        }

        public Dictionary<long,long> GetThirdIdList(long? categoryId, long thirdCategoryId)
        {
            var sqlTxt = "select Id,ThirdId from article where ThirdId is not null";

            if (categoryId.HasValue)
                sqlTxt += " and CategoryId=@CategoryId";

            sqlTxt += " and ThirdCategoryId=@ThirdCategoryId;";
            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                if (categoryId.HasValue)
                    SetCommandParameter(cmd, "CategoryId", DbType.Int64, categoryId);
                SetCommandParameter(cmd, "ThirdCategoryId", DbType.Int64, thirdCategoryId);
                var dt = GetDataTable(cmd);

                if (!IsEmptyDataTable(dt))
                {
                    
                    return dt.AsEnumerable().ToDictionary(row => Convert.ToInt64(row["Id"]), row2 => Convert.ToInt64(row2["ThirdId"]));
                }
                return null;
            }
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
            sbBuff.Append("select a.*,b.Name as CategoryName,c.Content from article a");
            sbBuff.AppendFormat(@" left join category b on  a.CategoryId=b.Id
                                                        left join article_content c on a.Id= c.ArticleId
                                                        where 1=1");

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

            sbBuff.Append(" order by a.CreateDate asc,a.id asc");
            sbBuff.Append(") as temp");
            //分页
            sbBuff.Append(" limit " + filter.StartRowNo + "," + filter.PageSize + ";select found_rows();");

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sbBuff.ToString()))
            {

                var ds = GetDataSet(cmd);
                filter.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                return ds.Tables[0].ToList<ArticleInfo>(new DBMapOption()
                {
                    RowFillAction = delegate (object contextObject, object dataRow, object rowFillActionParam)
                    {
                        if (dataRow != null)
                        {
                            var article = (ArticleInfo)contextObject;

                            var content = (((DataRow)dataRow)["Content"]).ToString();

                            content = content.ClearHtml().ClearLine();

                            if (!string.IsNullOrWhiteSpace(content) && content.Length > 200)
                                content = content.Substring(0, 200);

                            article.Summary = content;
                        }

                    }
                });
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

        public ArticleInfo GetBase(long articleId)
        {
            var sql = "select a.*,b.Name Category from article a left join category b on a.CategoryId=b.Id where a.Id=@Id";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sql))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, articleId);

                return GetDataRow(cmd).Fill<ArticleInfo>();
            }
        }

        public int GetCountByCategoryId(long categoryId)
        {
            var sqlTxt = "select count(1) from article where CategoryId=@CategoryId;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "CategoryId", DbType.Int64, categoryId);

                return GetInt(cmd);

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