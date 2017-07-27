using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationsTester.VariableHandlers
{
    public class UICollections
    {
        public static ObservableCollection<string> SubmitMethodBoxValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("PayPage");
            values.Add("HTMLDoc");
            values.Add("DirectPost");
            return values;
        }
        public static ObservableCollection<string> ModeBoxValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("Test");
            values.Add("Live");
            return values;
        }
        public static ObservableCollection<string> TransactionTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("CREDIT_CARD");
            values.Add("DEBIT_CARD");
            values.Add("CREDIT_DEBIT_CARD");
            values.Add("ACH");
            values.Add("INTERAC");
            values.Add("BATCH");
            return values;
        }
        public static ObservableCollection<string> EdgeExpressModeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("PC");
            values.Add("Cloud");
            return values;
        }
        public static ObservableCollection<string> EdgeExpressCountryValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("US");
            values.Add("CA");
            return values;
        }
        public static ObservableCollection<string> TCCValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("50");
            values.Add("51");
            values.Add("52");
            values.Add("53");
            return values;
        }
        public static ObservableCollection<string> CreditChargeTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("SALE");
            values.Add("CREDIT");
            values.Add("VOID");
            values.Add("FORCE_SALE");
            values.Add("AUTH");
            values.Add("CAPTURE");
            values.Add("ADJUSTMENT");
            values.Add("SIGNATURE");
            values.Add("QUERY_PAYMENT");
            return values;
        }
        public static ObservableCollection<string> DebitChargeTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("PURCHASE");
            values.Add("REFUND");
            values.Add("QUERY_PURCHASE");
            return values;
        }
        public static ObservableCollection<string> CreditDebitChargeTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("SALE");
            return values;
        }
        public static ObservableCollection<string> ACHChargeTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("DEBIT");
            values.Add("CREDIT");
            values.Add("VOID");
            values.Add("QUERY");
            return values;
        }
        public static ObservableCollection<string> BatchChargeTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("SETTLE");
            return values;
        }
        public static ObservableCollection<string> AccountTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("DEFAULT");
            values.Add("CASH_BENEFIT");
            values.Add("FOOD_STAMP");
            return values;
        }
        public static ObservableCollection<string> CreditTypeValues() // Not actual parameters, merely determine workflow and logic.
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("INDEPENDENT");
            values.Add("DEPENDENT");
            return values;
        }
        public static ObservableCollection<string> CreditEntryModeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("KEYED");
            values.Add("EMV");
            values.Add("AUTO");
            values.Add("HID");
            return values;
        }
        public static ObservableCollection<string> DebitEntryModeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("EMV");
            values.Add("HID");
            return values;
        }
        public static ObservableCollection<string> ACHEntryModeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("KEYED");
            return values;
        }
        public static ObservableCollection<string> MPDChargeTypeValues(string transactionType)
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            if (transactionType == "CREDIT_CARD")
            {
                values.Add("SALE");
                values.Add("CREDIT");
                values.Add("FORCE_SALE");
                values.Add("DELETE_CUSTOMER");
                values.Add("AUTH");
                return values;
            }
            if (transactionType == "ACH")
            {
                values.Add("DEBIT");
                values.Add("CREDIT");
                values.Add("DELETE_CUSTOMER");
            }
            return values;
        }
        public static ObservableCollection<string> MPDTransactionTypeValues()
        {
            ObservableCollection<string> values = new ObservableCollection<string>();
            values.Add("CREDIT_CARD");
            values.Add("ACH");
            return values;
        }
        public static List<TCCList> ACHorCCTCC(string transactionType) //Formats list for MPD TransactioNCondition Code
        {
            List<TCCList> _TCCBoxItems = new List<TCCList>();
            if (transactionType == "ACH")
            {
                _TCCBoxItems.Add(new TCCList { Header = "PPD", Value = "50" });
                _TCCBoxItems.Add(new TCCList { Header = "TEL", Value = "51" });
                _TCCBoxItems.Add(new TCCList { Header = "WEB", Value = "52" });
                _TCCBoxItems.Add(new TCCList { Header = "CCD", Value = "53" });
            }
            if (transactionType == "CREDIT_CARD")
            {
                _TCCBoxItems.Add(new TCCList { Header = "Recurring", Value = "6" });
                _TCCBoxItems.Add(new TCCList { Header = "ECommerce", Value = "5" });
            }
            return _TCCBoxItems;
        }
        public class TCCList
        {
            public string Header { get; set; }
            public string Value { get; set; }
        }
    }
}
