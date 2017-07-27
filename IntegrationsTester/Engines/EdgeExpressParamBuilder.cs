using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;

namespace IntegrationsTester.Engines
{
    public class EdgeExpressParamBuilder
    {
        private string _transactionType;
        private string _country;
        private string _amount;
        private string _response;
        private string _xWebId;
        private string _xWebTerminalId;
        private string _xWebAuthkey;
        private string _duplicateMode = VariableHandlers.Globals.Default.DuplicateMode;


        public EdgeExpressParamBuilder(string transactionType)
        {
            _transactionType = transactionType;
            SetCredentialsToUse();
            _amount = VariableHandlers.StandardCredentials.Default.CurrentAmount;
            _country = VariableHandlers.StandardCredentials.Default.Country;
        }
        private void SetCredentialsToUse()
        {
            switch (VariableHandlers.Globals.Default.CredentialsToUse)
            {
                case "US":
                    _xWebId = VariableHandlers.StandardCredentials.Default.ActiveXWebID;
                    _xWebTerminalId = VariableHandlers.StandardCredentials.Default.ActiveXWebTerminalID;
                    _xWebAuthkey = VariableHandlers.StandardCredentials.Default.ActiveXWebAuthKey;
                    break;
                case "CA":
                    if (_transactionType == "DEBITSALE" || _transactionType == "DEBITRETURN")
                    {
                        _xWebId = VariableHandlers.CanadianCredentials.Default.XWebIDDebit;
                        _xWebTerminalId = VariableHandlers.CanadianCredentials.Default.XWebTerminalIDDebit;
                        _xWebAuthkey = VariableHandlers.CanadianCredentials.Default.XWebAuthKeyDebit;
                    }
                    else
                    {
                        _xWebId = VariableHandlers.CanadianCredentials.Default.XWebIDCredit;
                        _xWebTerminalId = VariableHandlers.CanadianCredentials.Default.XWebTerminalIDCredit;
                        _xWebAuthkey = VariableHandlers.CanadianCredentials.Default.XWebAuthKeyCredit;
                    }
                    break;
                case "LOOPBACK":
                    _xWebId = VariableHandlers.LoopbackCredentials.Default.XWebID;
                    _xWebTerminalId = VariableHandlers.LoopbackCredentials.Default.XWebTerminalID;
                    _xWebAuthkey = VariableHandlers.LoopbackCredentials.Default.XWebAuthKey;
                    break;
                case "FSA":
                    _xWebId = VariableHandlers.FSACredentials.Default.XWebID;
                    _xWebTerminalId = VariableHandlers.FSACredentials.Default.XWebTerminalID;
                    _xWebAuthkey = VariableHandlers.FSACredentials.Default.XWebAuthKey;
                    break;
                default:
                    break;
            }
        }
        public string BuildFinancialTransactionRequest()
        {
           
            switch (_country)
            {
                case "US":
                    _response = BuildBasicUSTransaction();
                    return _response;
                case "CA":
                    if (_transactionType != "DEBITSALE" || _transactionType != "DEBITRETURN")
                    {
                        _response = BuildCACreditTransaction();
                    }
                    if (_transactionType == "SALE")
                    {
                        throw new InvalidOperationException("SALE not supported in Canada");
                    }
                    else
                    {
                        _response = BuildCADebitTransaction();
                    }
                    return _response;
                default:
                    throw new InvalidOperationException("Invalid Country");
            }

        }
        public string BuildNonFinancialTransactionRequest()
        {
            switch (_transactionType)
            {
                case "SIGNATURE":
                    return BuildSignatureTransaction();
                default:
                    throw new InvalidOperationException("Transaction Type Not Implemented");
            }
        }
        private string BuildSignatureTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = false;
            ws.OmitXmlDeclaration = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("REQUEST");

                xml.WriteStartElement("TRANSACTIONTYPE");
                xml.WriteString("PPDPROMPTSIGNATURE");
                xml.WriteEndElement();

                xml.WriteStartElement("DISPLAYCAPTUREDSIGNATURE");
                xml.WriteString("T");
                xml.WriteEndElement();

                xml.WriteStartElement("TITLE");
                xml.WriteString("Please Sign");
                xml.WriteEndElement();

                xml.WriteStartElement("SIGNATUREFILEFORMAT");
                xml.WriteString("BMP");
                xml.WriteEndElement();

                xml.WriteStartElement("RETURNSIGNATUREFORMAT");
                xml.WriteString("BASE64");
                xml.WriteEndElement();

                xml.WriteEndElement();
            }

            return sb.ToString();
        }

        private string BuildCACreditTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = false;
            ws.OmitXmlDeclaration = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("REQUEST");

                xml.WriteStartElement("XWEBID");
                xml.WriteString(_xWebId);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBTERMINALID");
                xml.WriteString(_xWebAuthkey);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBAUTHKEY");
                xml.WriteString(_xWebTerminalId);
                xml.WriteEndElement();

                xml.WriteStartElement("TRANSACTIONTYPE");
                xml.WriteString(_transactionType);
                xml.WriteEndElement();

                xml.WriteStartElement("AMOUNT");
                xml.WriteString(_amount);
                xml.WriteEndElement();

                if (_duplicateMode == "NO_CHECK")
                {
                    xml.WriteStartElement("ALLOWDUPLICATES");
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
            return sb.ToString();
        }
        private string BuildCADebitTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = false;
            ws.OmitXmlDeclaration = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("REQUEST");

                xml.WriteStartElement("XWEBID");
                xml.WriteString(_xWebId);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBTERMINALID");
                xml.WriteString(_xWebTerminalId);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBAUTHKEY");
                xml.WriteString(_xWebAuthkey);
                xml.WriteEndElement();

                xml.WriteStartElement("TRANSACTIONTYPE");
                xml.WriteString(_transactionType);
                xml.WriteEndElement();

                xml.WriteStartElement("AMOUNT");
                xml.WriteString(_amount);
                xml.WriteEndElement();

                if (_duplicateMode == "NO_CHECK")
                {
                    xml.WriteStartElement("ALLOWDUPLICATES");
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
            return sb.ToString();

        }
        private string BuildBasicUSTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = false;
            ws.OmitXmlDeclaration = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("REQUEST");

                xml.WriteStartElement("XWEBID");
                xml.WriteString(_xWebId);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBTERMINALID");
                xml.WriteString(_xWebTerminalId);
                xml.WriteEndElement();

                xml.WriteStartElement("XWEBAUTHKEY");
                xml.WriteString(_xWebAuthkey);
                xml.WriteEndElement();

                xml.WriteStartElement("TRANSACTIONTYPE");
                xml.WriteString(_transactionType);
                xml.WriteEndElement();

                xml.WriteStartElement("AMOUNT");
                xml.WriteString(_amount);
                xml.WriteEndElement();

                if (_duplicateMode == "NO_CHECK")
                {
                    xml.WriteStartElement("ALLOWDUPLICATES");
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
            return sb.ToString();
        }

    }
}
