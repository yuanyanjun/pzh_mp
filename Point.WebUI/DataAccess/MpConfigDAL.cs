using Point.Common.DBMaper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Point.WebUI
{
    public class MpConfigDAL: Point.Common.DataAccessBase
    {
        private static MpConfigDAL _dal = null;
        public static MpConfigDAL Instance
        {
            get
            {
                if (_dal == null)
                    _dal = new MpConfigDAL();
                return _dal;
            }
        }

        public void SetMpConfig(MpConfigInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            var sqlTxt = @"delete from mp_config;
                                     insert into mp_config (AppId,Sercet,Token,EncodingAESKey) values 
                                     (@AppId,@Sercet,@Token,@EncodingAESKey);";

            using (DbCommand cmd = DbInstance.GetSqlStringCommand(sqlTxt))
            {
                SetCommandParameter(cmd, "AppId", DbType.String, info.AppId);
                SetCommandParameter(cmd, "Sercet", DbType.String, info.Sercet);
                SetCommandParameter(cmd, "Token", DbType.String, info.Token);
                SetCommandParameter(cmd, "EncodingAESKey", DbType.String, info.EncodingAESKey);
                ExecSql(cmd);
            }
        }

        public MpConfigInfo GetMpConfig()
        {
            var sqlTxt = "select  * from mp_config limit 0,1;";

            return GetDataRow(sqlTxt).Fill<MpConfigInfo>();
        }
    }
}