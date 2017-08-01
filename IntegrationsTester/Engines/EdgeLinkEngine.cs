using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.Engines
{
    public class EdgeLinkEngine
    {
        #region DLL Imports
        [DllImport("XCClient.dll", EntryPoint = "XC_eXpressLink",
                ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern void XC_eXpressLink(IntPtr hHandle, string Parameters, StringBuilder Result);
        #endregion
        private string _parameters;
        private string _entryPoint;
        private string _result;
        
        public EdgeLinkEngine(string parameters, string entryPoint)
        {
            _parameters = parameters;
            _entryPoint = entryPoint;
        }
        public string Execute()
        {
            if (_entryPoint == "DLL")
            {
                DLLCall();
                return _result;
            }
            if (_entryPoint == "EXE")
            {
                EXECall();
                return _result;
            }
            else
            {
                throw new InvalidOperationException("Invalid Entry Point");
            }
        }
        private void DLLCall()
        {
            StringBuilder sb = new StringBuilder(20000);

            IntPtr hHandle = new IntPtr();
            XC_eXpressLink(hHandle, _parameters, sb);

            _result = sb.ToString();
        }
        private void ReadResultFile()
        {
            _result = File.ReadAllText(VariableHandlers.Globals.Default.ResultFilePath);
        }
        private void EXECall()
        {
            System.Diagnostics.Process xCharge = new System.Diagnostics.Process();
            xCharge.StartInfo.FileName = "c:\\programdata\\cam commerce solutions\\x-charge\\application\\xcharge.exe";
            xCharge.StartInfo.Arguments = _parameters;
            xCharge.Start();
            xCharge.WaitForExit();
            xCharge.Dispose();

            ReadResultFile();

        }
    }
}
