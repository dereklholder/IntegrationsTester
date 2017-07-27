using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.Engines
{
    public class DtGEngine
    {
        private string _environment;
        private string _parameters;
        private string _liveGatewayUrl = "https://gw.t3secure.net/x-chargeweb.dll";
        private string _testGatewayUrl = "https://test.t3secure.net/x-chargeweb.dll";
        private string _urlToUse;
        private string _response;

        public DtGEngine(string parameters)
        {
            _environment = VariableHandlers.Globals.Default.Environment;
            urlToUse();
            _parameters = parameters;
        }
        private void urlToUse()
        {
            if (_environment == "LIVE")
            {
                _urlToUse = _liveGatewayUrl;
            }
            else
            {
                _urlToUse = _testGatewayUrl;
            }
        }
        public string SendPost()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlToUse);

                request.KeepAlive = false;
                request.Timeout = 60000;
                request.Method = "POST";

                byte[] byteArray = Encoding.ASCII.GetBytes(_parameters);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                WebResponse response = request.GetResponse();

                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                _response = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                return _response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
            
    }
}
