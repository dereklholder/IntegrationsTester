using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
