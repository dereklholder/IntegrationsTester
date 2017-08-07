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
using System.Windows.Shapes;

namespace IntegrationsTester
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            setComboBoxOnLoad();
        }
        private void SaveAllTheThings()
        {
            VariableHandlers.Globals.Default.Save();
            VariableHandlers.StandardCredentials.Default.Save();
            VariableHandlers.CanadianCredentials.Default.Save();
            VariableHandlers.LoopbackCredentials.Default.Save();
            VariableHandlers.FSACredentials.Default.Save();
            VariableHandlers.CustomCredentials.Default.Save();
        }
        private void setComboBoxOnLoad()
        {
            if (VariableHandlers.Globals.Default.Environment == "LIVE")
            {
                EnvironmentComboBox.SelectedIndex = 0;
            }
            if (VariableHandlers.Globals.Default.Environment == "TEST")
            {
                EnvironmentComboBox.SelectedIndex = 1;
            }
            if (VariableHandlers.Globals.Default.DuplicateMode == "CHECK")
            {
                DuplicateModeComboBox.SelectedIndex = 0;
            }
            if (VariableHandlers.Globals.Default.DuplicateMode == "NO_CHECK")
            {
                DuplicateModeComboBox.SelectedIndex = 1;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)EnvironmentComboBox.SelectedItem).Name == "LIVE")
            {
                VariableHandlers.Globals.Default.Environment = "LIVE";
            }
            if (((ComboBoxItem)EnvironmentComboBox.SelectedItem).Name == "TEST")
            {
                VariableHandlers.Globals.Default.Environment = "TEST";
            }
            if (((ComboBoxItem)DuplicateModeComboBox.SelectedItem).Name == "CHECK")
            {
                VariableHandlers.Globals.Default.DuplicateMode = "CHECK";
            }
            if (((ComboBoxItem)DuplicateModeComboBox.SelectedItem).Name == "NO_CHECK")
            {
                VariableHandlers.Globals.Default.DuplicateMode = "NO_CHECK";
            }
            SaveAllTheThings();
            this.Close();
        }

        private void EnvironmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
