using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IntegrationsTester.Engines
{
    public class DTGParamBuilder
    {
        private string _transactionType;

        public DTGParamBuilder(string transactionType)
        {
            _transactionType = transactionType;
        }

        public string BuildAliasTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.OmitXmlDeclaration = true;
            ws.NewLineOnAttributes = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("GatewayRequest");

                xml.WriteStartElement("SpecVersion");
                xml.WriteString("XWeb3.7");
                xml.WriteEndElement();

                xml.WriteStartElement("XWebID");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("AuthKey");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("TerminalID");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("POSType");
                xml.WriteString("PC");
                xml.WriteEndElement();

                xml.WriteStartElement("PinCapabilities");
                xml.WriteString("FALSE");
                xml.WriteEndElement();

                xml.WriteStartElement("TrackCapabilities");
                xml.WriteString("NONE");
                xml.WriteEndElement();

                xml.WriteStartElement("Alias");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("TransactionType");
                switch (_transactionType)
                {
                    case "aliasLookup":
                        xml.WriteString("AliasLookupTransaction");
                        xml.WriteEndElement();
                        break;
                    case "aliasDelete":
                        xml.WriteString("AliasDeleteTransaction");
                        xml.WriteEndElement();
                        break;
                    case "aliasUpdate":
                        xml.WriteString("AliasUpdateTransaction");
                        xml.WriteEndElement();

                        xml.WriteStartElement("ExpDate");
                        xml.WriteString("");
                        xml.WriteEndElement();

                        xml.WriteStartElement("AcctNum");
                        xml.WriteString("");
                        xml.WriteEndElement();
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Transaction Type Sent");

                }
                xml.WriteEndElement();
            }
            return sb.ToString();
        }
        public string BuildLookupTransaction()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.OmitXmlDeclaration = true;
            ws.NewLineOnAttributes = true;

            using (XmlWriter xml = XmlWriter.Create(sb, ws))
            {
                xml.WriteStartElement("GatewayRequest");

                xml.WriteStartElement("SpecVersion");
                xml.WriteString("XWeb3.7");
                xml.WriteEndElement();

                xml.WriteStartElement("XWebID");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("AuthKey");
                xml.WriteString("");
                xml.WriteEndElement();

                xml.WriteStartElement("TerminalID");
                xml.WriteString("");
                xml.WriteEndElement();

                if (_transactionType == "lookupTransactionID")
                {
                    xml.WriteStartElement("TransactionID");
                    xml.WriteString("");
                    xml.WriteEndElement();
                }
                if (_transactionType == "lookupOrderID")
                {
                    xml.WriteStartElement("OrderID");
                    xml.WriteString("");
                    xml.WriteEndElement();
                }

                xml.WriteStartElement("TransactionType");
                xml.WriteString("LookupTransaction");
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
            return sb.ToString();
        }
    }
}
