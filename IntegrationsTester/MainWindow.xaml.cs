﻿using System;
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
using System.Threading;
using System.IO;
using System.Diagnostics;

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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }
        #region EdgeExpressProcessingMethods
        private void EdgeExpressTransaction(string transactionType)
        {
            try
            {
                StandardCredentials.Default.EdgeExpressParameters = new Engines.EdgeExpressParamBuilder(transactionType).BuildFinancialTransactionRequest();
                EdgeExpressResponseBox.Text = new Engines.EdgeExpressEngine(StandardCredentials.Default.EdgeExpressParameters, (string)EdgeExpressModeComboBox.SelectedItem).SendToEdgeExpress();
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
        #region OEHP Processing Methods
        private string _html; //Variable Used to keep Scraped HTML and keep it in scope of all the methods that need to access it
        private string _sessionToken; //Keep sesssion token in scope of all methods that need to access it
        private void RenderBrowser(Engines.OEHPEngine requestToOEHP)
        {
            string getResponseFromOEHP = requestToOEHP.Execute();
            var browser = oehpChromiumBrowser.GetBrowser();
            _sessionToken = requestToOEHP.SessionToken;

            var n = new Engines.OEHPRCMStatus(_sessionToken);

            if (requestToOEHP.ErrorMessage == null)
            {
                switch (requestToOEHP._requestMethod)
                {
                    case "PayPage":
                        browser.MainFrame.LoadUrl(getResponseFromOEHP);
                        break;
                    case "HTMLDoc":
                        CefSharp.WebBrowserExtensions.LoadHtml(oehpChromiumBrowser, getResponseFromOEHP, false);
                        break;
                    case "DirectPost":
                        CefSharp.WebBrowserExtensions.LoadHtml(oehpChromiumBrowser, getResponseFromOEHP, false);
                        break;
                    default:
                        CefSharp.WebBrowserExtensions.LoadHtml(oehpChromiumBrowser, "Unkown error Occured, Check all transaction Parameters", false);
                        break;
                }
            }
            else
            {
                CefSharp.WebBrowserExtensions.LoadHtml(oehpChromiumBrowser, getResponseFromOEHP, false);
            }
            requestToOEHP.Dispose();
            if ((string)EntryModeComboBox.SelectedItem == "EMV") // Starts Executing RCM status Get 
            {
                var workerThread = new Thread(n.ExecuteOnTimer);
                workerThread.Start();
            }
        }
        private void logParametersAndResponse() //splitting this into its own method to make code prettier
        {
            using (var n = new GeneralFunctions.Logging(PostParametersBox.Text.ToString()))
            {
                n.WriteLog();
            }
            using (var n = new GeneralFunctions.Logging(QueryParametersBox.Text))
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
            QueryParametersBox.Text = queryParameters;
            Engines.OEHPEngine sendPost = new Engines.OEHPEngine(queryParameters, "DirectPost");

            string response = sendPost.Execute();
            QueryResponseBox.Text = response;
            return response;

        }
        private string getSubmitMethodToUse() //Gets what Submit method to use By checking Transaction Type.
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
                case "SETTLE":
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
            EdgeExpressModeComboBox.ItemsSource = UICollections.EdgeExpressModeValues();
            EdgeExpressCountryComboBox.ItemsSource = UICollections.EdgeExpressCountryValues();
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
            EntryModeComboBox.Visibility = Visibility.Visible;
            EntryModeLabel.Visibility = Visibility.Visible;
            //Approval Code box for Force Sale....
            EntryModeComboBox.SelectedIndex = 0;
            ChargeTypeComboBox.SelectedIndex = 0;

            CreditTypeComboBox.SelectedIndex = -1;
            TCCComboBox.SelectedIndex = -1;
        }
        public void SetBatchUICollections()
        {
            //Set Charge Type and Entry mode Combo Boxen.;;
            ChargeTypeComboBox.ItemsSource = UICollections.BatchChargeTypeValues();
            EntryModeComboBox.ItemsSource = UICollections.CreditEntryModeValues();

            AccountTypeLabel.Visibility = Visibility.Hidden;
            AccountTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeComboBox.Visibility = Visibility.Hidden;
            CreditTypeLabel.Visibility = Visibility.Hidden;
            TCCComboBox.Visibility = Visibility.Hidden;
            TCCLabel.Visibility = Visibility.Hidden;
            EntryModeComboBox.Visibility = Visibility.Hidden;
            EntryModeLabel.Visibility = Visibility.Hidden;

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
            EntryModeComboBox.Visibility = Visibility.Visible;
            EntryModeLabel.Visibility = Visibility.Visible;

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

            EntryModeComboBox.Visibility = Visibility.Visible;
            EntryModeLabel.Visibility = Visibility.Visible;

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
            EntryModeComboBox.Visibility = Visibility.Visible;
            EntryModeLabel.Visibility = Visibility.Visible;

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
        private void UsePresetsOEHP_Checked(object sender, RoutedEventArgs e)
        {
            if (UsePresetsOEHP.IsChecked == true)
            {
                accountTokenBox.Visibility = Visibility.Hidden;
            }
            else
            {
                accountTokenBox.Visibility = Visibility.Visible;
            }

        }
        private void BuildPostButton_Click(object sender, RoutedEventArgs e)
        {

            if (!TransactionTypeComboBox.Text.Equals("QUERY_PAYMENT"))
            {
                OrderIDBox.Text = Engines.OEHPParamBuilder.OrderIDRandom();
            }           
            Engines.OEHPParamBuilder buildPost = new Engines.OEHPParamBuilder(accountTokenBox.Text, (string)TransactionTypeComboBox.SelectedItem, (string)EntryModeComboBox.SelectedItem, (string)ChargeTypeComboBox.SelectedItem, AmountBox.Text, OrderIDBox.Text, (string)AccountTypeComboBox.SelectedItem, (string)TCCComboBox.SelectedItem, CustomParametersBox.Text);
            PostParametersBox.Text = buildPost.BuildAPost();

        }
        private void SubmitPostButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Engines.OEHPEngine sendPost = new Engines.OEHPEngine(PostParametersBox.Text, (string)SubmitMethodBox.Text);
                RenderBrowser(sendPost);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void BuildAndSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrderIDBox.Text = Engines.OEHPParamBuilder.OrderIDRandom(); //Create Random Order ID

                Engines.OEHPParamBuilder buildPost = new Engines.OEHPParamBuilder(accountTokenBox.Text, (string)TransactionTypeComboBox.SelectedItem, (string)EntryModeComboBox.SelectedItem, (string)ChargeTypeComboBox.SelectedItem, AmountBox.Text, OrderIDBox.Text, (string)AccountTypeComboBox.SelectedItem, (string)TCCComboBox.SelectedItem, CustomParametersBox.Text);
                PostParametersBox.Text = buildPost.BuildAPost(); //Builds Post Parameters


                Engines.OEHPEngine sendPost = new Engines.OEHPEngine(PostParametersBox.Text, getSubmitMethodToUse()); //Creates Object for sending the post.
                RenderBrowser(sendPost); //Calls Render Browser that Executes Object function and sends post to OEHP, then Renders the response (Either a Paypage, rawResponse, or an error)

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void QueryStringToJsonButton_Click(object sender, RoutedEventArgs e)
        {
            QueryResponseBox.Text = GeneralFunctions.DataManipulation.QueryStringToJson(QueryResponseBox.Text);
        }
        #endregion
        #region UI - OEHP Combo Box Manipulation Logic.
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
                    case "BATCH":
                        SetBatchUICollections();
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
        private void GuideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Integrations Tester User Guide.docx";
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = path;
                Process.Start(info);
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
            }
        }
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SetPresetToUse(object sender, RoutedEventArgs e)
        {
            
            switch (((MenuItem)sender).Name)
            {
                case "EMVTestingMenuItem":
                    Globals.Default.CredentialsToUse = "US";
                    CanadianTestingMenuItem.IsChecked = false;
                    LoopBackTestingMenuItem.IsChecked = false;
                    FSATestingMenuItem.IsChecked = false;
                    CustomPresetMenuItem.IsChecked = false;
                    break;
                case "CanadianTestingMenuItem":
                    Globals.Default.CredentialsToUse = "CA";
                    EMVTestingMenuItem.IsChecked = false;
                    LoopBackTestingMenuItem.IsChecked = false;
                    FSATestingMenuItem.IsChecked = false;
                    CustomPresetMenuItem.IsChecked = false;
                    break;
                case "LoopBackTestingMenuItem":
                    Globals.Default.CredentialsToUse = "LOOPBACK";
                    EMVTestingMenuItem.IsChecked = false;
                    CanadianTestingMenuItem.IsChecked = false;
                    FSATestingMenuItem.IsChecked = false;
                    CustomPresetMenuItem.IsChecked = false;
                    break;
                case "FSATestingMenuItem":
                    Globals.Default.CredentialsToUse = "FSA";
                    EMVTestingMenuItem.IsChecked = false;
                    CanadianTestingMenuItem.IsChecked = false;
                    LoopBackTestingMenuItem.IsChecked = false;
                    CustomPresetMenuItem.IsChecked = false;
                    break;
                case "CustomPresetMenuItem":
                    Globals.Default.CredentialsToUse = "CUSTOM";
                    EMVTestingMenuItem.IsChecked = false;
                    CanadianTestingMenuItem.IsChecked = false;
                    LoopBackTestingMenuItem.IsChecked = false;
                    FSATestingMenuItem.IsChecked = false;
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region UI- EdgeExpress Combo Box Manipulation logic
        private void EdgeExpressCountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StandardCredentials.Default.Country = (string)EdgeExpressCountryComboBox.SelectedItem;
        }
        private void EdgeExpressSubmitMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)EdgeExpressSubmitMethodComboBox.SelectedItem).Name)
            {
                case "SimpleButtons":
                    EdgeExpressSimpleButtonGrid.Visibility = Visibility.Visible;
                    break;
                case "RawParameters":
                    EdgeExpressSimpleButtonGrid.Visibility = Visibility.Hidden;
                    break;
                default:
                    throw new InvalidOperationException("Invalid Submit Method Selected");
            }
        }
        #endregion
        #region UI- EdgeExpress Button Interaction
        private void EdgeExpressSaleButton_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressTransaction("SALE");
        }

        private void EdgeExpressCreditSaleButton_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressTransaction("CREDITSALE");
        }

        private void EdgeExpressCreditReturn_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressTransaction("CREDITRETURN");
        }

        private void EdgeExpressDebitPurchase_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressTransaction("DEBITSALE");
        }

        private void EdgeExpressDebitReturnButton_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressTransaction("DEBITRETURN");
        }
        private void EdgeExpressSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            EdgeExpressResponseBox.Text = new Engines.EdgeExpressEngine(EdgeExpressParameters.Text, (string)EdgeExpressModeComboBox.SelectedItem).SendToEdgeExpress();
        }
        private void EdgeExpressSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            StandardCredentials.Default.EdgeExpressParameters = new Engines.EdgeExpressParamBuilder("SIGNATURE").BuildNonFinancialTransactionRequest();
            string base64String = GeneralFunctions.DataManipulation.GetSignatureBase64FromXml(new Engines.EdgeExpressEngine(StandardCredentials.Default.EdgeExpressParameters, (string)EdgeExpressModeComboBox.SelectedItem).SendToEdgeExpress());

            EdgeExpressSignature.Source = GeneralFunctions.DataManipulation.DecodeBase64Image(base64String);
        }
        #endregion
        #region UI - DtG Button Interaction
        private void DtGSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            DtGResponseBox.Text = new Engines.DtGEngine(DtGRequestBox.Text).SendPost();
        }
        private void DtGLookupTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            DtGRequestBox.Text = new Engines.DTGParamBuilder("lookupTransactionID").BuildLookupTransaction();
        }
        private void DtGLookupTransactionOrderIDButton_Click(object sender, RoutedEventArgs e)
        {
            DtGRequestBox.Text = new Engines.DTGParamBuilder("lookupOrderID").BuildLookupTransaction();
        }
        private void DtGAliasLookupTransaction_Click(object sender, RoutedEventArgs e)
        {
            DtGRequestBox.Text = new Engines.DTGParamBuilder("aliasLookup").BuildAliasTransaction();
        }

        private void DtGAliasUpdateTransaction_Click(object sender, RoutedEventArgs e)
        {
            DtGRequestBox.Text = new Engines.DTGParamBuilder("aliasUpdate").BuildAliasTransaction();
        }

        private void DtGAliasDeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            DtGRequestBox.Text = new Engines.DTGParamBuilder("aliasDelete").BuildAliasTransaction();
        }
        #endregion
        #region UI EdgeLink Button Interaction
        private void EdgeLinkSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EdgeLinkResultBox.Text = new Engines.EdgeLinkEngine(EdgeLinkRequestBox.Text, ((ComboBoxItem)EdgeLinkIntegrationMethodComboBox.SelectedItem).Name).Execute();
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



        private void CustomParametersBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        

        private void Window_Closed(object sender, EventArgs e)
        {
            CefSharp.Cef.Shutdown();
        }

        
    }
}
