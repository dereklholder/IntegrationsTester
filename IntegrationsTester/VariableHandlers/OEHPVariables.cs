using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.VariableHandlers
{
    public class OEHPVariables
    {
        //Test OEHP URLs
        public static string TestPayPagePostURL = "https://ws.test.paygateway.com/HostPayService/v1/hostpay/transactions";
        public static string TestDirectPostURL = "https://ws.test.paygateway.com/api/v1/transactions";
        public static string TestHtmlDocPostURL = "https://ws.test.paygateway.com/HostPayService/v1/hostpay/paypage/";
        public static string TestRcmStatusURL = "https://ws.test.paygateway.com/HostPayService/v1/hostpay/transactions/status/";

        //Live OEHP Urls
        public static string LivePayPagePostURL = "https://ws.paygateway.com/HostPayService/v1/hostpay/transactions";
        public static string LiveDirectPostURL = "https://ws.paygateway.com/api/v1/transactions";
        public static string LiveHtmlDocPostURL = "https://ws.paygateway.com/HostPayService/v1/hostpay/paypage/";
        public static string LiveRcmStatusURL = "https://ws.paygateway.com/HostPayService/v1/hostpay/transactions/status/";

    }
    public struct PayPageJson
    {
        public string sealedSetupParameters { get; set; }
        public string actionUrl { get; set; }
        public string errorMessage { get; set; }
    }
    public class RCMStatus
    {
        public string rcmStartingSignal { get; set; }
        public string rcmFinishedSignal { get; set; }
        public string rcmResponseCode { get; set; }
        public string rcmResponseDescription { get; set; }
    }
}
