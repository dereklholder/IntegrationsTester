using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using System.Windows.Threading;

namespace IntegrationsTester.Engines
{
    public class OEHPRCMStatus
    {
        private string _sessionToken;
        private string _urlToUse;

        public OEHPRCMStatus(string sessionToken)
        {
            _sessionToken = sessionToken;
            if (VariableHandlers.Globals.Default.Environment == "LIVE")
            {
                _urlToUse = VariableHandlers.OEHPVariables.LiveRcmStatusURL;
            }
            else
            {
                _urlToUse = VariableHandlers.OEHPVariables.TestRcmStatusURL;
            }
        }
        public void ExecuteOnTimer()
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                
                do
                {
                    VariableHandlers.Globals.Default.RCMStatus = Execute();
                    if (VariableHandlers.Globals.Default.RCMStatus != null)
                    {
                        nvc = HttpUtility.ParseQueryString(VariableHandlers.Globals.Default.RCMStatus);
                    }
                    System.Threading.Thread.Sleep(1000);
                    if (nvc.Get("rcm_finished_signal") == "true")
                    {
                        break;
                    }

                } while (nvc.Get("rcm_finished_signal") != "true" || nvc.Get("rcm_finished_signal") != null);
            }
                
            catch (Exception ex )
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
            }
        }
        public string Execute()
        {
            try
            {
                WebRequest rcmStatusRequest = WebRequest.Create(_urlToUse + _sessionToken);

                rcmStatusRequest.Method = "GET";

                Stream objStream;
                objStream = rcmStatusRequest.GetResponse().GetResponseStream();

                StreamReader sr = new StreamReader(objStream);

                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return null;
            }
        }

    }
}
