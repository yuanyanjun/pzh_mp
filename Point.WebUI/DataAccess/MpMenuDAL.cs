using Point.Common.DBMaper;
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
    }
}