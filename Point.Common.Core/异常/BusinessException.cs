using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Common.Exceptions
{
    public class BusinessException : CustomException
    {
        public BusinessException()
        {
            base.ExceptionType = "b";
        }
        public BusinessException(string msg)
            : base(msg)
        {
            base.ExceptionType = "b";
        }

      

        public BusinessException(string msg, int errorCode)
            : base(msg, errorCode)
        {
            base.ExceptionType = "b";
        }

        public BusinessException(string msg, string helper, int errorCode)
            : base(msg, helper, errorCode)
        {
            base.ExceptionType = "b";
        }

    }
}
