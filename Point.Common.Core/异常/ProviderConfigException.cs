using System;
using Point.Common.Exceptions;
using Newtonsoft.Json;
namespace Point.Common.Exceptions
{
    public class ProviderConfigException : Point.Common.Exceptions.BusinessException
    {
        public ProviderConfigException()
        {
        }
        public ProviderConfigException(string msg)
            : base(msg)
        {
        }

        public ProviderConfigException(string msg, int errorCode)
            : base(msg, errorCode)
        {
        }

        public ProviderConfigException(string msg, string helper, int errorCode)
            : base(msg, helper, errorCode)
        {
        }

      

    }
}
