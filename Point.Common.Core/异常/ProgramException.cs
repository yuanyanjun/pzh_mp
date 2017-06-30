
namespace Point.Common.Exceptions
{
    public class ProgramException : CustomException
    {
        public ProgramException()
        {
            base.ExceptionType = "p";
        }
        public ProgramException(string msg)
            : base(msg)
        {
            base.ExceptionType = "p";
        }

        public ProgramException(string msg, int errorCode)
            : base(msg, errorCode)
        {
            base.ExceptionType = "p";
        }

        public ProgramException(string msg, string helper, int errorCode)
            : base(msg, helper, errorCode)
        {
            base.ExceptionType = "p";
        }
    }
}
