using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.VariableHandlers
{
    public class EdgeExpressVariables
    {
        public static string RCMUrl = "https://localsystem.paygateway.com:21113/RcmService.svc";
        public static string RCMMethod = "/Initialize";
        public static string RCMQuerystring = "?xl2Parameters=";
        public static string RCMStatusMethod = "/Status";
        public string ActiveParametrs { get; set; }
    }

}
