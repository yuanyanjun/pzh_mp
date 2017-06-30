
namespace Point.Common.Exceptions
{
    public class CustomException : System.Exception
    {
        public int ErrorCode { get; set; }
        public string ExceptionType { get; set; }
        public string ViewName { get; set; }

        public CustomException()
        {
            this.ErrorCode = 0;
            this.ExceptionType = this.GetType().Name;
        }
        public CustomException(string msg)
            : base(msg)
        {
            this.ErrorCode = 0;
            this.ExceptionType = this.GetType().Name;
        }

        public CustomException(string msg, int errorCode)
            : base(msg)
        {
            this.ErrorCode = errorCode;
            this.ExceptionType = this.GetType().Name;
        }

        public CustomException(string msg, int errorCode, string viewName)
            : base(msg)
        {
            this.ErrorCode = errorCode;
            this.ExceptionType = this.GetType().Name;
            this.ViewName = viewName;
        }

        public CustomException(string msg, string helper, int errorCode)
            : base(msg + helper)
        {
            this.ErrorCode = errorCode;
            this.ExceptionType = this.GetType().Name;
        }
    }
}
