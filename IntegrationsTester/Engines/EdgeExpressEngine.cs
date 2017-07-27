using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IntegrationsTester.Engines
{
    public class EdgeExpressEngine : VariableHandlers.EdgeExpressVariables
    {
        private string _parameters;
        private string _environment;
        private string _requestMethod;
        private string _response;

        public EdgeExpressEngine(string parameters, string requestMethod)
        {
            _parameters = parameters;
            _requestMethod = requestMethod;
        }
        public string SendToEdgeExpress()
        {
            switch (_requestMethod)
            {
                case "PC":
                    XCharge.XpressLink2.XLEmv EdgeExpress = new XCharge.XpressLink2.XLEmv();
                    EdgeExpress.Execute(_parameters, out _response);
                    return _response;
                case "Cloud":
                    _response = EdgeExpressCloud();
                    return _response;
                default:
                    throw new InvalidOperationException("Invalid Request Method");
            }
        }
        private string EdgeExpressCloud()
        {
            string url = RCMUrl + RCMMethod + RCMQuerystring + _parameters;
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.Method = "GET";
            getRequest.ContentType = "application/xml";

            WebResponse response = getRequest.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(dataStream);
            string rawResponse = responseReader.ReadToEnd();

            responseReader.Close();
            dataStream.Close();

            _response = ParseXmlRcmResponse(rawResponse);

            return _response;
        }
        private string ParseXmlRcmResponse(string cloudTransactionResponseString)
        {
            string parsedData = null;
            using (XmlTextReader xr = new XmlTextReader(new StringReader(cloudTransactionResponseString)))
            {
                while (xr.Read())
                {
                    while (xr.ReadToFollowing("XmlRcmResponse"))
                    {
                        parsedData = Regex.Replace(xr.ReadInnerXml(), @"\t|\n|\r", ""); //Get rid of escape characters, because they are lame.
                    }
                }
                xr.Close();
            }
            return parsedData;
        }
    }
}
