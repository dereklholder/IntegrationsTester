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
        
        public MainWindow()
        {
            InitializeComponent();
            SetCommonCollections();
         
        }
        #region OEHP Processing Methods
        private string _html; //Variable Used to keep Scraped HTML and keep it in scope of all the methods that need to access it
        private string _sessionToken; //Keep sesssion token in scope of all methods that need to access it
        private void RenderBrowser(Engines.OEHPEngine requestToOEHP)
        {
            string getResponseFromOEHP = requestToOEHP.Execute();
            _sessionToken = requestToOEHP.SessionToken;
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
        private void logParametersAndResponse() //splitting this into its own method to make code prettier
        {
            using (var n = new GeneralFunctions.Logging(PostParametersBox.Text.ToString()))
            {
                n.WriteLog();
            }
            using (var n = new GeneralFunctions.Logging(QueryResponseBox.Text.ToString()))
            {
                n.WriteLog();
            }
        }
        private string sendQuery()
        {
            Engines.OEHPParamBuilder buildPost = new Engines.OEHPParamBuilder(StandardCredentials.Default.ActiveAccountToken, (string)TransactionTypeComboBox.SelectedItem, "KEYED", "QUERY", "", OrderIDBox.Text, "", "", "&full_detail_flag=true");
            string queryParameters = buildPost.BuildAPost();
            Engines.OEHPEngine sendPost = new Engines.OEHPEngine(queryParameters, "DirectPost");

            string response = sendPost.Execute();
            QueryResponseBox.Text = response;
            return response;

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
                        value = "PayPage";
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
        private async Task<string> getSignatureString(string html)
        {
            var tcs = new TaskCompletionSource<string>();
            if (html != null)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                try
                {
                    string result = doc.DocumentNode.SelectSingleNode("//input[@type='hidden' and @id='signatureImage' and @name='signatureImage']").Attributes["value"].Value;
                    tcs.TrySetResult(result);
                    return tcs.Task.Result.ToString();
                }
                catch (Exception ex)
                {
                    using (var n = new GeneralFunctions.Logging(ex.ToString()))
                    {
                        n.WriteLog();
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private async Task<string> getPaymentFinishedSignal(string html)
        {
            var tcs = new TaskCompletionSource<string>();
            if (html != null)
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); //Uses HTMl Agility Pack for Easy HTMl Parseing (Regex bad)
                doc.LoadHtml(html);
                try
                {
                    var values = from value in doc.DocumentNode.Descendants("input")
                                 where value.Attributes["id"].Value == "paymentFinishedSignal"
                                 select value;

                    foreach (var value in values)
                    {
                        Console.WriteLine(value.Attributes["value"].Value);
                        tcs.TrySetResult(value.Attributes["value"].Value);

                    }
                    return tcs.Task.Result.ToString();
                }
                catch (Exception ex)
                {
                    using (var n = new GeneralFunctions.Logging(ex.ToString()))
                    {
                        n.WriteLog();
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private async void getRcmStatus(string sessionToken)
        {
            Engines.OEHPRCMStatus rcmStatus = new Engines.OEHPRCMStatus(sessionToken);
            this.Dispatcher.Invoke(() =>
            {
                RCMStatusBox.Text = rcmStatus.Execute();
            });
        }
        private bool getTransactionIsDirectPost(string chargeType)
        {
            if (chargeType == "VOID" || chargeType == "CAPTURE" || chargeType == "AUTH" || chargeType == "ADJUSTMENT")
            {
                return true;
            }
            if (chargeType == "CREDIT" && (string)CreditTypeComboBox.SelectedItem == "DEPENDENT")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Frame loadEndEvent is hooked for performing Query logic 
        private async void oehpChromiumBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {

            //Implement RCM Status
            if (e.Frame.IsMain)
            {
                await oehpChromiumBrowser.GetBrowser().MainFrame.GetSourceAsync().ContinueWith(taskHtml =>
                {
                    string html = taskHtml.Result;
                    _html = html;
                });
            }
            //Initialize these variables outside of Invoke
            string paymentFinishedSignal = await getPaymentFinishedSignal(_html).ConfigureAwait(false); //Hacking My way through Async Functions Because I can
            getRcmStatus(_sessionToken);
            string sigString = await getSignatureString(_html).ConfigureAwait(false);

            this.Dispatcher.Invoke(() =>
            {
                //RCMStatusBox.Text = rcmStatus;
                bool isDirectPost = getTransactionIsDirectPost((string)ChargeTypeComboBox.SelectedItem);
                if (isDirectPost == true)
                {
                    sendQuery();
                }
                else if (paymentFinishedSignal == "done")
                {
                    switch ((string)TransactionTypeComboBox.SelectedItem)
                    {
                        case "CREDIT_CARD":
                            sendQuery();
                            logParametersAndResponse();
                            //Parse Result and Save to DB
                            if (sigString != null)
                            {
                                SignatureImage.Source = GeneralFunctions.DataManipulation.DecodeBase64Image(sigString);
                            }
                            break;
                        case "CREDIT_DEBIT_CARD":
                            sendQuery();
                            logParametersAndResponse();
                            //pArse Result and Save to DB
                            if (sigString != null)
                            {
                                SignatureImage.Source = GeneralFunctions.DataManipulation.DecodeBase64Image(sigString);
                            }
                            break;
                        case "DEBIT_CARD":
                            sendQuery();
                            logParametersAndResponse();
                            //parse result and save to DB
                            break;
                        case "INTERAC":
                            sendQuery();
                            logParametersAndResponse();
                            //parse Result and save to db
                            break;
                        case "ACH":
                            sendQuery();
                            logParametersAndResponse();
                            //Parse Result and Save to DB
                            break;
                        default:
                            using (var n = new GeneralFunctions.Logging("Something Broke in an Unhandled way.. Check Parameters and try again"))
                            {
                                n.WriteLog();
                            }
                            break;
                    }
                }
                else
                {
                    //Do nothing
                }
            });

        }
        #endregion
        #region UI - Collection Handlers for OEHP, NOW WITH VISIBILITY!
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
        #region UI - OEHP Button Interaction
        private void BuildPostButton_Click(object sender, RoutedEventArgs e)
        {

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


            Engines.OEHPEngine sendPost = new Engines.OEHPEngine(PostParametersBox.Text, getSubmitMethodToUse()); //Creates Object for sending the post.
            RenderBrowser(sendPost); //Calls Render Browser that Executes Object function and sends post to OEHP, then Renders the response (Either a Paypage, rawResponse, or an error)

        }
        #endregion
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
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
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
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
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
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
            }
        }
        #endregion
        #region UI - MenuBar Interaction
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }
        #endregion
               

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
