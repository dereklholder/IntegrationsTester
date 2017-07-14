using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.Engines
{
    public class OEHPParamBuilder
    {
        private string _accountToken;
        private string _transactionType;
        private string _chargeType;
        private string _entryMode;
        private string _chargeTotal;
        private string _orderID;
        private string _accountType;
        private string _transactionConditionCode;
        private string _customParameters;
        private string _duplicateMode;
        
        public OEHPParamBuilder(string account_token, string transaction_type, string entry_mode, string charge_type, string charge_total, string order_id, string account_type, string transaction_condition_code, string custom_parameters)
        {
            _accountToken = account_token;
            _transactionType = transaction_type;
            _chargeType = charge_type;
            _chargeTotal = charge_total;
            _entryMode = entry_mode;
            _orderID = order_id;
            _accountType = account_type;
            _transactionConditionCode = transaction_condition_code;
            _customParameters = custom_parameters;

            if (VariableHandlers.Globals.Default.DuplicateMode == "NO_CHECK")
            {
                _duplicateMode = "NO_CHECK";
            }
            else
            {
                _duplicateMode = "CHECK";
            }

        }
        public string BuildAPost()
        {
            try
            {
                switch(_transactionType)
                {
                    case "CREDIT_CARD":
                        return CreditParamBuilder();
                    case "DEBIT_CARD":
                        return DebitParamBuilder();
                    case "ACH":
                        return ACHParamBuilder();
                    case "CREDIT_DEBIT_CARD":
                        return CreditParamBuilder();
                    default:
                        throw new InvalidOperationException("Invalid Transaction Type");
                }
            }
            catch (Exception ex)
            {
                //Implement Logging
                return ex.ToString();
            }
        }
        private string DebitParamBuilder()
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append("account_token=" + _accountToken
                               + "&transaction_type=" + _transactionType
                               + "&entry_mode=" + _entryMode
                               + "&charge_type=" + _chargeType
                               + "&charge_total=" + _chargeTotal
                               + "&order_id=" + _orderID
                               + "&account_type=" + _accountType
                               + "&duplicate_check=" + _duplicateMode
                               + _customParameters);
            //Logging
            return parameters.ToString();
        }
        private string CreditParamBuilder()
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append("account_token=" + _accountToken
                                + "&transaction_type=" + _transactionType
                                + "&entry_mode=" + _entryMode
                                + "&charge_type=" + _chargeType
                                + "&charge_total=" + _chargeTotal
                                + "&order_id=" + _orderID
                                + "&duplicate_check=" + _duplicateMode
                                + _customParameters);
            //Logging
            return parameters.ToString();
        }
        private string ACHParamBuilder()
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append("account_tken=" + _accountToken
                                + "&transaction_type=" + _transactionType
                                + "&entry_mode=" + _entryMode
                                + "&charge_type=" + _chargeType
                                + "&charge_total=" + _chargeTotal
                                + "&order_id=" + _orderID
                                + "&transaction_condition_code=" + _transactionConditionCode
                                + "&duplicate_check=" + _duplicateMode
                                + _customParameters);

            return parameters.ToString();
        }
        public static string OrderIDRandom() //Code for creating Randomized OrderIDs
        {
            Random random = new Random((int)DateTime.Now.Ticks); // Use Timestamp to Seed Random Number
            StringBuilder builder = new StringBuilder();
            Int32 ch;
            for (int i = 0; i < 8; i++)
            {
                ch = Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65));
                builder.Append(ch.ToString());
            }
            return builder.ToString();
        }
    }
    
}
