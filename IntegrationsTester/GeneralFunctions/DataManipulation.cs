using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;

namespace IntegrationsTester.GeneralFunctions
{
    public class DataManipulation
    {
        public static BitmapImage DecodeBase64Image(string base64string)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64string);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(bytes);
                bi.EndInit();

                return bi;
            }
            catch (Exception ex)
            {
                using (var n = new Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return null;
            }
        }
        public static string GetSignatureBase64FromXml(string xml)
        {
            string parsedData = null;
            using (XmlTextReader xr = new XmlTextReader(new StringReader(xml)))
            {
                while (xr.Read())
                {
                    while (xr.ReadToFollowing("SIGNATUREIMAGE"))
                    {
                        parsedData = xr.ReadInnerXml();
                    }
                }
            }
            return parsedData;
        }
        public static string QueryStringToJson(string queryString)
        {
            try
            {
                NameValueCollection keyPairs = HttpUtility.ParseQueryString(queryString);
                keyPairs.AllKeys.Where(k => !String.IsNullOrEmpty(k)).ToDictionary(k => k, k => keyPairs[k]);
                Dictionary<string, string> dictData = new Dictionary<string, string>(keyPairs.Count);
                foreach (string key in keyPairs.AllKeys)
                {
                    if (key != null)
                    {
                        dictData.Add(key, keyPairs.Get(key));
                    }
                }

                var entries = dictData.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
                return "{" + string.Join(", \n", entries) + "}";

            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return "An error Occured converting Query String, please check the log.";
            }
        }
    }
}
