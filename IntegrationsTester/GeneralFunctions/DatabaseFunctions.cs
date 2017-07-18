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
        public void CreateDBFile()
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
        public int CreateOEHPTransactionTable()
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
        public int CreateOEHPSignatureTable()
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
        

        #endregion
        public DatabaseFunctions(string integrationMethod, string action, object data)
        {
            _integrationMethod = integrationMethod;
            _action = action;
            _data = data;
        }
        public void Execute()
        {
            switch (_integrationMethod)
            {
                case "OEHP":
                    //implement

                    break;

                case "EdgeExpress":
                    //Implement

                    break;

                case "DirectToGateway":
                    //Implement

                    break;

                case "EdgeLink":
                    //Implement

                    break;

                case "HPF":
                    //Implement

                    break;

                default:
                    throw new InvalidOperationException("Invalid Integration Method Sent");
                  
            }
        }
    }
}
