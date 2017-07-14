using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IntegrationsTester.VariableHandlers;

namespace IntegrationsTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region UICollection Handlers for OEHP, NOW WITH VISIBILITY!
        public void SetCommonCollections() //Make sure to keep implementing!
        {
            TransactionTypeComboBox.ItemsSource = UICollections.TransactionTypeValues();
            SubmitMethodBox.ItemsSource = UICollections.SubmitMethodBoxValues();
            TCCComboBox.ItemsSource = UICollections.TCCValues();
            AccountTypeComboBox.ItemsSource = UICollections.AccountTypeValues();
            CreditTypeComboBox.ItemsSource = UICollections.CreditTypeValues();
        }
        public void SetCreditCardUICollections()
        {
            //Set Charge Type and Entry mode Combo Boxen
            ChargeTypeComboBox.ItemsSource = UICollections.CreditChargeTypeValues();
            EntryModeComboBox.ItemsSource = UICollections.CreditEntryModeValues();

            AccountTypeLabel.Visibility = Visibility.Hidden;
            AccountTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
            TCCComboBox.Visibility = Visibility.Hidden;
            TCCLabel.Visibility = Visibility.Hidden;
            //Approval Code box for Force Sale....
            EntryModeComboBox.SelectedIndex = 0;
            ChargeTypeComboBox.SelectedIndex = 0;

            CreditTypeComboBox.SelectedIndex = -1;
            TCCComboBox.SelectedIndex = -1;
        }
        public void SetDebitCardUICollections()
        {
            //Set Charge Type and Entry Mode Combo Boxen
            ChargeTypeComboBox.ItemsSource = UICollections.DebitChargeTypeValues();
            EntryModeComboBox.ItemsSource = UICollections.DebitEntryModeValues();

            AccountTypeLabel.Visibility = Visibility.Visible;
            AccountTypeComboBox.Visibility = Visibility.Visible;

            CreditTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
            TCCComboBox.Visibility = Visibility.Hidden;
            TCCLabel.Visibility = Visibility.Hidden;
            //Approval Code box for Force Sale....
            EntryModeComboBox.SelectedIndex = 0;
            ChargeTypeComboBox.SelectedIndex = 0;
            AccountTypeComboBox.SelectedIndex = 0;

            CreditTypeComboBox.SelectedIndex = -1;
            TCCComboBox.SelectedIndex = -1;
        }
        public void SetACHUICollections()
        {
            ChargeTypeComboBox.ItemsSource = UICollections.ACHChargeTypeValues();
            EntryModeComboBox.ItemsSource = UICollections.ACHEntryModeValues();

            AccountTypeComboBox.Visibility = Visibility.Hidden;
            AccountTypeLabel.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
            CreditTypeComboBox.Visibility = Visibility.Hidden;
            TCCComboBox.Visibility = Visibility.Visible;
            TCCLabel.Visibility = Visibility.Visible;

            EntryModeComboBox.SelectedIndex = 0;

            ChargeTypeComboBox.SelectedIndex = 0;

            CreditTypeComboBox.SelectedIndex = -1;
            TCCComboBox.SelectedIndex = 0;

        }
        public void SetCreditDebitUICollections()
        {
            ChargeTypeComboBox.ItemsSource = UICollections.CreditDebitChargeTypeValues();
            EntryModeComboBox.ItemsSource = UICollections.DebitEntryModeValues();


            AccountTypeComboBox.Visibility = Visibility.Visible;
            AccountTypeLabel.Visibility = Visibility.Visible;
            CreditTypeLabel.Visibility = Visibility.Hidden;
            CreditTypeComboBox.Visibility = Visibility.Hidden;
            TCCComboBox.Visibility = Visibility.Hidden;
            TCCLabel.Visibility = Visibility.Hidden;

            EntryModeComboBox.SelectedIndex = 0;

            AccountTypeComboBox.SelectedIndex = 0;
            ChargeTypeComboBox.SelectedIndex = 0;

            CreditTypeComboBox.SelectedIndex = -1;
            TCCComboBox.SelectedIndex = -1;
        }
        public void SetVisibilityCredit()
        {
            OrderIDBox.IsReadOnly = true;
            OrderIDBox.Text = "";

            CreditTypeComboBox.SelectedIndex = -1;
            CreditTypeComboBox.Visibility = Visibility.Visible;
            CreditTypeLabel.Visibility = Visibility.Visible;

            //Approval Code PH
        }
        public void SetVisbilityForceSale()
        {
            OrderIDBox.IsReadOnly = true;
            OrderIDBox.Text = "";

            CreditTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
        }
        public void SetVisibilityAdjustmentVoidCaptureQuery()
        {
            OrderIDBox.IsReadOnly = false;

            CreditTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            SetCommonCollections();
         
        }
        private void RenderBrowser(Engines.OEHPEngine requestToOEHP)
        {
            string getResponseFromOEHP = requestToOEHP.Execute();
            if (requestToOEHP.ErrorMessage == null)
            {
                switch (requestToOEHP._requestMethod)
                {
                    case "PayPage":
                        oehpChromiumBrowser.Load(getResponseFromOEHP);
                        break;
                    case "HTMLDoc":
                            CefSharp.WebBrowserExtensions.LoadHtml(oehpChromiumBrowser, getResponseFromOEHP, false);
                        break;
                    case "DirectPost":
                        oehpChromiumBrowser.Content = getResponseFromOEHP;
                        break;
                    default:
                        oehpChromiumBrowser.Content = "Unkown Error Occured, Check all Transaction Parameters";
                        break;
                }
            }
            else
            {
                oehpChromiumBrowser.Content = requestToOEHP.ErrorMessage;
            }
        }
        private string getSubmitMethodToUse()
        {
            switch ((string)ChargeTypeComboBox.SelectedItem)
            {
                case "CREDIT":
                    string value = "";
                    if ((string)CreditTypeComboBox.SelectedItem == "DEPENDENT")
                    {
                        value = "DirectPost";
                    }
                    if ((string)CreditTypeComboBox.SelectedItem == "INDEPENDENT")
                    {
                        value =  "PayPage";
                    }
                    return value;
                case "SALE":
                    return "PayPage";
                case "VOID":
                    return "DirectPost";
                case "FORCE_SALE":
                    return "DirectPost";
                case "CAPTURE":
                    return "DirectPost";
                case "AUTH":
                    return "PayPage";
                case "ADJUSTMENT":
                    return "DirectPost";
                case "SIGNATURE":
                    return "PayPage";
                case "QUERY_PAYMENT":
                    return "DirectPost";
                case "QUERY_PURCHASE":
                    return "DirectPost";
                case "PURCHASE":
                    return "PayPage";
                case "REFUND":
                    return "PayPage";
                case "QUERY":
                    return "DirectPost";
                default:
                    return null;
            }
        }
        //Placeholder Button! Do nto use
        private void basicTestButtonPH_Click(object sender, RoutedEventArgs e)
        {
            Engines.OEHPEngine engine = new Engines.OEHPEngine(VariableHandlers.StandardCredentials.Default.TestingString, "PayPage");
            string url = engine.Execute();

            oehpChromiumBrowser.Address = url;
        }
        #region UI - Combo Box Manipulation Logic.
        private void TransactionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch ((string)TransactionTypeComboBox.SelectedItem)
                {
                    case "CREDIT_CARD":
                        SetCreditCardUICollections();
                        break;

                    case "DEBIT_CARD":
                        SetDebitCardUICollections();
                        break;

                    case "CREDIT_DEBIT_CARD":
                        SetCreditDebitUICollections();
                        break;
                    case "ACH":
                        SetACHUICollections();
                        break;
                    case "INTERAC":
                        SetDebitCardUICollections();
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Charge Type Selected");
                }
            }
            catch (Exception ex)
            {
                //Implement Logging
            }
        }
        private void CreditTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch ((string)CreditTypeComboBox.SelectedItem)
                {
                    case "INDEPEDENT":
                        OrderIDBox.IsReadOnly = true;
                        break;
                    case "DEPENDENT":
                        OrderIDBox.IsReadOnly = false;
                        break;
                    default:
                        OrderIDBox.IsReadOnly = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                //Implement Logging
            }
        }
        private void ChargeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch ((string)ChargeTypeComboBox.SelectedItem)
                {
                    case "CREDIT":
                        SetVisibilityCredit();
                        break;
                    case "ADJUSTMENT":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    case "FORCE_SALE":
                        SetVisbilityForceSale();
                        break;
                    case "VOID":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    case "CAPTURE":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    case "QUERY_PAYMENT":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    case "QUERY_PURCHASE":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    case "QUERY":
                        SetVisibilityAdjustmentVoidCaptureQuery();
                        break;
                    default:
                        SetVisbilityForceSale();
                        break;

                }
            }
            catch (Exception ex)
            {
                // Implement Logging
            }
        }
        #endregion
        

        private void accountTokenBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }

        private void BuildPostButton_Click(object sender, RoutedEventArgs e)
        {
            //Not Fully Implemented!
            OrderIDBox.Text = Engines.OEHPParamBuilder.OrderIDRandom();
            Engines.OEHPParamBuilder buildPost = new Engines.OEHPParamBuilder(accountTokenBox.Text, (string)TransactionTypeComboBox.SelectedItem, (string)EntryModeComboBox.SelectedItem, (string)ChargeTypeComboBox.SelectedItem, AmountBox.Text, OrderIDBox.Text, (string)AccountTypeComboBox.SelectedItem, (string)TCCComboBox.SelectedItem, CustomParametersBox.Text);
            PostParametersBox.Text = buildPost.BuildAPost();

        }

        private void SubmitPostButton_Click(object sender, RoutedEventArgs e)
        {
            
            Engines.OEHPEngine sendPost = new Engines.OEHPEngine(PostParametersBox.Text, (string)SubmitMethodBox.Text);
            RenderBrowser(sendPost);
        }

        private void BuildAndSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            OrderIDBox.Text = Engines.OEHPParamBuilder.OrderIDRandom(); //Create Random Order ID

            Engines.OEHPParamBuilder buildPost = new Engines.OEHPParamBuilder(accountTokenBox.Text, (string)TransactionTypeComboBox.SelectedItem, (string)EntryModeComboBox.SelectedItem, (string)ChargeTypeComboBox.SelectedItem, AmountBox.Text, OrderIDBox.Text, (string)AccountTypeComboBox.SelectedItem, (string)TCCComboBox.SelectedItem, CustomParametersBox.Text);
            PostParametersBox.Text = buildPost.BuildAPost(); //Builds Post Parameters


            Engines.OEHPEngine sendPost = new Engines.OEHPEngine(PostParametersBox.Text, getSubmitMethodToUse());
            RenderBrowser(sendPost);

        }
    }
}
