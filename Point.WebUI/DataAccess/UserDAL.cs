using Point.Common.DBMaper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Point.Common.Util;

namespace Point.WebUI
{
    public class UserDAL : Point.Common.DataAccessBase
    {
        private static UserDAL _dal = null;
        public static UserDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new UserDAL();
                return _dal;
            }
        }

        /// <summary>
        /// 验证账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool AccountExist(string account)
        {
            if (string.IsNullOrWhiteSpace(account))
                return false;

            var sqlTxt = "select count(1) from users where Account=@Account;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Account", DbType.String, account);

                return GetInt(cmd) > 0;
            }
        }

        public UserInfo Get(string account, string password)
        {

            var sqlTxt = "select * from users where Account=@Account and Password=@Password;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Account", DbType.String, account);
                SetCommandParameter(cmd, "Password", DbType.String, password.GetSHA1HashCode());
                return GetDataRow(cmd).Fill<UserInfo>();
            }
        }

        public UserInfo Get(long id)
        {

            var sqlTxt = "select * from users where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                return GetDataRow(cmd).Fill<UserInfo>();
            }
        }

        public long Add(UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var sqlTxt = @"insert into users (Account,Password,Name,MobileNo,CreateDate)
                                     values
                                     (@Account,@Password,@Name,@MobileNo,now());
                                    select last_insert_id();";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "Account", DbType.String, user.Account);
                SetCommandParameter(cmd, "Password", DbType.String, user.Password.GetSHA1HashCode());
                SetCommandParameter(cmd, "Name", DbType.String, user.Name);
                SetCommandParameter(cmd, "MobileNo", DbType.String, user.MobileNo);
                return GetLong(cmd);
            }
        }

        public void UpdatePassword(long id, string password)
        {
            var sqlTxt = "update users set Password=@Password where Id=@Id;";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {

                SetCommandParameter(cmd, "Password", DbType.String, password.GetSHA1HashCode());
                SetCommandParameter(cmd, "Id", DbType.Int64, id);

                ExecSql(cmd);
            }
        }
    }
}