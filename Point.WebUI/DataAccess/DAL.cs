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

    
       
        #region 菜单

        public IEnumerable<MpMenuItem> SelectMenuList()
        {
            var sqlTxt = "select * from pzh_mp_menu order by SortOrder asc;";

            return GetDataTable(sqlTxt).ToList<MpMenuItem>();
        }
        #endregion
    }
}