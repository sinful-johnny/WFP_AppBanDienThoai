using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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

namespace HW4
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        public SqlConnection _connection;
        void Encrypt(string password, string username)
        {
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["username"].Value = username;
            config.AppSettings.Settings["password"].Value = Convert.ToBase64String(cypherText);
            config.AppSettings.Settings["entropy"].Value = Convert.ToBase64String(entropy);
            config.AppSettings.Settings["isPasswordRemmembered"].Value = "1";
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string password = PasswordBox.Password;
                string username = UsernameTextBox.Text;
                string server = ConfigurationManager.AppSettings["Server"];
                string Database = ConfigurationManager.AppSettings["Database"];

                string connectionString = $"""
                                              Server={server};
                                              Database={Database};
                                              User ID= {username};
                                              Password= {password}; 
                                              TrustServerCertificate=True
                                              """;
                _connection = new SqlConnection(connectionString);
            try
            {
                _connection.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
            }
                
            if(_connection.State == ConnectionState.Open)
            {
                MessageBox.Show("Good!", "Logged in successfully", MessageBoxButton.OK);
                if (RemembermeCheckBox.IsChecked == true)
                {
                    Encrypt(password, username);
                }
                else
                {
                    config.AppSettings.Settings["isPasswordRemmembered"].Value = "0";
                    config.AppSettings.Settings["username"].Value = " ";
                    config.AppSettings.Settings["password"].Value = " ";
                    config.AppSettings.Settings["entropy"].Value = " ";
                    config.Save(ConfigurationSaveMode.Minimal);
                }
                DialogResult = true;
            }else
            {
                MessageBox.Show("Wrong credential!", "Log in failed", MessageBoxButton.OK);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["isPasswordRemmembered"].Value == "1")
            {
                UsernameTextBox.Text = config.AppSettings.Settings["username"].Value;
                var cypherText = Convert.FromBase64String(ConfigurationManager.AppSettings["password"]);
                var entropy = Convert.FromBase64String(ConfigurationManager.AppSettings["entropy"]);
                var decryptedPassword = ProtectedData.Unprotect(cypherText, entropy, DataProtectionScope.CurrentUser);
                var realPassword = Encoding.UTF8.GetString(decryptedPassword);

                PasswordBox.Password = realPassword;
                RemembermeCheckBox.IsChecked = true;
            }
        }
    }
}
