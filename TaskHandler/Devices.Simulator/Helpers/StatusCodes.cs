using System;
using System.Collections.Generic;
using System.Text;

namespace TaskHandler.Helpers
{
    static public class StatusCodes
    {
        public enum VipaSW1SW2Codes
        {
            Success = 0x9000,
            Timeout = 0xFFFE,
            Failure = 0xFFFF
        }
    }
}
