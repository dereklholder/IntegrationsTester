using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace IntegrationsTester.GeneralFunctions
{
    public class DatabaseFunctions : IntegrationsTester.VariableHandlers.OEHPVariables
    {
        private string _integrationMethod;
        private string _action;
        private object _data;
        #region Generic DB Functions
        private void CreateDBFile()
        {
           if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite") != true)
            {
                try
                {
                    SQLiteConnection.CreateFile(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite");

                    //Create Tables

                }
                catch (Exception ex)
                {
                    using (var n = new GeneralFunctions.Logging(ex.ToString()))
                    {
                        n.WriteLog();
                    }
                }
            }
           else
            {
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite");

                    SQLiteConnection.CreateFile(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite");

                    //Create Tables
                }
                catch (Exception ex)
                {
                    using (var n = new GeneralFunctions.Logging(ex.ToString()))
                    {
                        n.WriteLog();
                    }
                }
            }
        }
        #endregion
        #region OEHP Database Functions
        private int CreateOEHPTransactionTable()
        {
            string sql = ""; //Build SQL String to Create OEHP TransactioN database Table

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source = database.sqlite;Version=3;");

            try
            {
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return 0;
            }
            finally
            {
                sql = null;
                m_dbConnection.Close();
            }
        }
        private int CreateOEHPSignatureTable()
        {
            string sql = ""; //Build SQL String to Create OEHP Signature Database Table
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source = database.sqlite;Version=3;");
            try
            {
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return 0;
            }
            finally
            {
                sql = null;
                m_dbConnection.Close();
            }
        }
        private void InsertOEHPTransaction(object data)
        {
            throw new NotImplementedException();
        }
        private void InsertOEHPSignatureTransaction(object data)
        {

        }
        #endregion
        #region EdgeExpress Database Functions
        private int CreateEdgeExpressTransactionTable()
        {
            string sql = "CREATE TABLE transactiondb (DUPLICATECARD varchar(10), DATE_TIME varchar(25), HOSTRESPONSECODE varchar(5), HOSTRESPONSEDESCRIPTION varchar(255), RESULT varchar(10), RESULTMSG varchar(255), APPROVEDAMOUNT varchar(10), BATCHNO varchar(10), BATCHAMOUNT varchar(10), APPROVALCODE varchar(10), ACCOUNT varchar(10), CARDHOLDERNAME varchar(25), CARDTYPE varchar(5), CARDBRAND varchar(10), CARDBRANDSHORT varchar(5), LANGUAGE varchar(10), ALIAS varchar(50), ENTRYTYPE varchar(10), RECEIPTTEXT varchar(500), EXPMONTH varchar(10), EXPYEAR varchar(10), TRANSACTIONID varchar(10), EMVTRANSACTION varchar(10))";
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source = database.sqlite;Version=3;");
            try
            {
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return 0;
            }
            finally
            {
                m_dbConnection.Close();
            }
        }
        private int CreateEdgeExpressSignatureTable()
        {
            string sql = "CREATE TABLE signaturedb (ID varchar(20), SIGNATUREFORMAT varchar(10), SIGNATUREIMAGE varchar(200), RESULT varchar(10), RESULTMSG varchar(25))";
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source = database.eedb;Version=3;");
            try
            {
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                using (var n = new GeneralFunctions.Logging(ex.ToString()))
                {
                    n.WriteLog();
                }
                return 0;
            }
            finally
            {
                m_dbConnection.Close();

            }
        }
        private void InsertEdgeExpressTransaction(object data)
        {
            throw new NotImplementedException();
        }
        private void InsertEdgeExpressSignature(object data)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region DirectToGateway Functions
        private int CreateDTGTransactionTable()
        {
            throw new NotImplementedException();
        }
        private void InsertDirectToGatewayTransaction()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region EdgeLink Functions
        private int CreateEdgeLinkTransactionTable()
        {
            throw new NotImplementedException();
        }
        private int CreateEdgeLinkSignatureTable()
        {
            throw new NotImplementedException();
        }
        private void InsertEdgeLinkTransaction()
        {
            throw new NotImplementedException();
        }
        private void InsertEdgeLinkSignature()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region HPF Functions
        private int CreateHPFTransactionTable()
        {
            throw new NotImplementedException();
        }
        private int CreateHPFSignatureTable()
        {
            throw new NotImplementedException();
        }
        private void InsertHPFTransaction()
        {
            throw new NotImplementedException();
        }
        private void InsertHPFSignature()
        {
            throw new NotImplementedException();
        }
        #endregion

        public DatabaseFunctions(string integrationMethod, string action, object data)
        {
            _integrationMethod = integrationMethod;
            _action = action;
            _data = data;
        }
        public void CreateDB()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite") != true)
            {
                CreateDBFile();
                CreateDTGTransactionTable();
                CreateEdgeExpressSignatureTable();
                CreateEdgeExpressTransactionTable();
                CreateEdgeLinkSignatureTable();
                CreateEdgeLinkTransactionTable();
                CreateHPFSignatureTable();
                CreateHPFTransactionTable();
                CreateOEHPSignatureTable();
                CreateOEHPTransactionTable();
            }
            else
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "database.sqlite");
                CreateDB();
            }
        }
        public void Execute()
        {
            switch (_integrationMethod)
            {
                case "OEHP":
                    throw new NotImplementedException();

                    break;

                case "EdgeExpress":
                    throw new NotImplementedException();

                    break;

                case "DirectToGateway":
                    throw new NotImplementedException();

                    break;

                case "EdgeLink":
                    throw new NotImplementedException();

                    break;

                case "HPF":
                    throw new NotImplementedException();

                    break;

                default:
                    throw new InvalidOperationException("Invalid Integration Method Sent");
                  
            }
        }
    }
}
