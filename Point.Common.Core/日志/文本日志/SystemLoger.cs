using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOGGING = Microsoft.Practices.EnterpriseLibrary.Logging;
namespace Point.Common.Core
{
    public class SystemLoger
    {
        public readonly static SystemLoger Current = new SystemLoger();

        private bool Enabled
        {
            get
            {
                return true;
            }
        }

        public void Write(object message)
        {
            if (Enabled)
            {
                Task.Factory.StartNew(()=>
                {
                    LOGGING.Logger.Write(message);
                });
              
            }
        }
    }
}
