using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class PosPlatformClient
    {
        public bool timeout = false;
        public string name;
        //public string loginUrl;
        //public string verifyCodeUrl;
        public string verifyCode;
        public string ip;
        public DateTime lastAccess;
    }
}
