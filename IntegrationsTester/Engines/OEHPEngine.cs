using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace IntegrationsTester.Engines
{ 
    public class OEHPEngine : IDisposable
    {
        private string _environment;
        private string _parameters;
        public string _requestMethod;
        private string _urlToUse;
        public string ErrorMessage;
        public string SessionToken;
        /// <summary>
        /// Sends request to API and returns resposne
        /// </summary>
        /// <param name="parameters">Use OEHP parambuilder to buidl request, or raw OEHP requst coule be sent in</param>
        /// <param name="requestMethod">Which method to use to get transaction, PayPAge returns URL, HTMLdoc returns HTML, DirectPost returns raw API response.</param>
        public OEHPEngine(string parameters, string requestMethod)
        {
            _environment = VariableHandlers.Globals.Default.Environment;
            _parameters = parameters;
            _requestMethod = requestMethod;
        }

        public string Execute()
        {
            try
            {
                switch (_requestMethod)
                {
                    case "PayPage":
                        return PayPagePost();
                    case "HTMLDoc":
                        return HTMLDocPost();
                    case "DirectPost":
                        return DirectPost();
                    default:
                        throw new InvalidOperationException("Invalid Request Method");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Exception Occured, please check the log");
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return ex.ToString();
            }            
        }
        private void urlToUse()
        {
            if (_environment == "TEST")
            {
                switch (_requestMethod)
                {
                    case "PayPage":
                        _urlToUse = VariableHandlers.OEHPVariables.TestPayPagePostURL;
                        break;
                    case "HTMLDoc":
                        _urlToUse = VariableHandlers.OEHPVariables.TestHtmlDocPostURL;
                        break;
                    case "DirectPost":
                        _urlToUse = VariableHandlers.OEHPVariables.TestDirectPostURL;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Request Method");
                }
            }
            if (_environment == "LIVE")
            {
                switch (_requestMethod)
                {
                    case "PayPage":
                        _urlToUse = VariableHandlers.OEHPVariables.LivePayPagePostURL;
                        break;
                    case "HTMLDoc":
                        _urlToUse = VariableHandlers.OEHPVariables.LiveHtmlDocPostURL;
                        break;
                    case "DirectPost":
                        _urlToUse = VariableHandlers.OEHPVariables.LiveDirectPostURL;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Request Method");
                }
            }
        }
        private string PayPagePost()
        {
            try
            {
                urlToUse();
                WebRequest oehpRequest = WebRequest.Create(_urlToUse);

                string postData = _parameters;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                oehpRequest.ContentType = "application/x-www-form-urlencoded";
                oehpRequest.Method = "POST";
                oehpRequest.ContentLength = byteArray.Length;

                Stream dataStream = oehpRequest.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                WebResponse oehpresponse = oehpRequest.GetResponse();
                dataStream = oehpresponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromOehp = reader.ReadToEnd();

                reader.Close();
                oehpresponse.Close();

                VariableHandlers.PayPageJson jsonResponse = new VariableHandlers.PayPageJson();
                jsonResponse = JsonConvert.DeserializeObject<VariableHandlers.PayPageJson>(responseFromOehp);

                string result = jsonResponse.actionUrl + jsonResponse.sealedSetupParameters;
                SessionToken = jsonResponse.sealedSetupParameters;
                if (jsonResponse.errorMessage != null)
                {
                    result = jsonResponse.errorMessage;
                    ErrorMessage = jsonResponse.errorMessage;
                }

                return result;

            }
            catch(Exception ex)
            {
                
                return ex.ToString();
            }

        }
        private string DirectPost()
        {
            try
            {
                urlToUse();
                WebRequest oehpRequest = WebRequest.Create(_urlToUse);

                string postData = _parameters;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                oehpRequest.ContentType = "application/x-www-form-urlencoded";
                oehpRequest.Method = "POST";
                oehpRequest.ContentLength = byteArray.Length;

                Stream dataStream = oehpRequest.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                WebResponse oehpResponse = oehpRequest.GetResponse();

                dataStream = oehpResponse.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                string responseFromOEHP = reader.ReadToEnd();

                reader.Close();
                oehpResponse.Close();

                return responseFromOEHP;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        private string HTMLDocPost()
        {
            try
            {
                urlToUse();
                WebRequest oehpRequest = WebRequest.Create(_urlToUse);

                string postData = _parameters;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                oehpRequest.ContentType = "application/x-www-form-urlencoded";
                oehpRequest.Method = "POST";
                oehpRequest.ContentLength = byteArray.Length;

                Stream dataStream = oehpRequest.GetRequestStream();

                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                WebResponse oehpResponse = oehpRequest.GetResponse();

                dataStream = oehpResponse.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                string responseFromOEHP = reader.ReadToEnd();

                reader.Close();
                oehpResponse.Close();

                return responseFromOEHP;
            }
            catch (Exception ex)
            {
                
                return ex.ToString();

            }
        }
        public void Dispose()
        {
            _environment = null;
            _parameters = null;
            _requestMethod = null;
            _urlToUse = null;
            SessionToken = null;
            ErrorMessage = null;
        }
    }
}
