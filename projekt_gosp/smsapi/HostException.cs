﻿
namespace SMSApi.Api
{
    public class HostException : SMSApi.Api.SmsapiException
    {
        public static readonly int E_JSON_DECODE = -1;

        public HostException(string message, int code)
            : base(message, code)
        {
        }
    }
}
