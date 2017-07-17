using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationsTester.GeneralFunctions
{
    public class Logging :IDisposable
    {
        private string _logString;
        private string _timeStamp;
        private string _logPath;

        public Logging (string logString)
        {
            _logString = logString;
            _timeStamp = DateTime.Now.ToString();
            _logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.Combine("Log", "Log.txt")).ToString();
        }
        public void WriteLog()
        {
            try
            {
                File.AppendAllText(_logPath, _timeStamp + Environment.NewLine + _logString + Environment.NewLine + "--------------------------------------------------" + Environment.NewLine);

            }
            catch (Exception ex)
            {
                //Exception Occured whuile writing a log.....

                MessageBox.Show("An Error Occured when Writing to the Log File." + Environment.NewLine + ex.Message);
            }
        }
        public void Dispose()
        {
            _logString = null;
            _timeStamp = null;
            _logPath = null;
        }

    }
}
