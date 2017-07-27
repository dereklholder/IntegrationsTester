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
        /// <summary>
        /// Method Builds an OEHP Parameter string to be used for a transaction requyest
        /// </summary>
        /// <param name="account_token">Account Token Credential</param>
        /// <param name="transaction_type">Credit, Debit, ACH, Interac, Credit or Debit</param>
        /// <param name="entry_mode">Keyed, Auto, EMV</param>
        /// <param name="charge_type">Depends on Transaction Type</param>
        /// <param name="charge_total">Amount of Transaction</param>
        /// <param name="order_id">Unique Identifier for Transaction</param>
        /// <param name="account_type">For Debit EBT Transactions, or for ACH Savings vs Checking account</param>
        /// <param name="transaction_condition_code">Determines transaction condition, either Recurring, or for ACH SEC Codes</param>
        /// <param name="custom_parameters">Any Additional Parameters sent, must be in NVP format</param>
        public OEHPParamBuilder(string account_token, string transaction_type, string entry_mode, string charge_type, string charge_total, string order_id, string account_type, string transaction_condition_code, string custom_parameters)
        {
            GetAccountTokenToUse(); //Determines what account token is to be used.
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
        private void GetAccountTokenToUse()
        {
            if (VariableHandlers.Globals.Default.UsePresets == true)
            {
                switch (VariableHandlers.Globals.Default.CredentialsToUse)
                {
                    case "US":
                        _accountToken = VariableHandlers.StandardCredentials.Default.AccountToken;
                        break;
                    case "CA":
                        _accountToken = VariableHandlers.CanadianCredentials.Default.AccountToken;
                        break;
                    case "LOOPBACK":
                        _accountToken = VariableHandlers.LoopbackCredentials.Default.AccountToken;
                        break;
                    case "FSA":
                        throw new InvalidOperationException("FSA not Supported on OEHP");
                    default:
                        throw new InvalidOperationException("Invalid Preset Selected");
                }
            }
            else
            {
                _accountToken = VariableHandlers.StandardCredentials.Default.ActiveAccountToken;
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
                    case "BATCH":
                        return BatchParambuilder();
                    default:
                        throw new InvalidOperationException("Invalid Transaction Type");
                }
            }
            catch (Exception ex)
            {
                using (GeneralFunctions.Logging log = new GeneralFunctions.Logging(ex.ToString()))
                {
                    log.WriteLog();
                }
                return ex.ToString();
            }
        }
        private string BatchParambuilder()
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append("account_token=" + _accountToken
                                + "&transaction_type=" + _transactionType
                                + "&charge_type=" + _chargeType);
            return parameters.ToString();
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
            parameters.Append("account_token=" + _accountToken
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
