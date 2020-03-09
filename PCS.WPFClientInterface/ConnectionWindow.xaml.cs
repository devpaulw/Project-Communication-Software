using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Xml;
using System.Xml.Linq;
using PCS.Data;

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        const string fieldSavePath = "./connection_infos.xml";
        private bool AutoConnect { get; }

        public bool Connected { get; private set; }

        public ConnectionWindow()
        {
            InitializeComponent();
        }

        public ConnectionWindow(bool autoConnect)
        {
            AutoConnect = autoConnect;
            InitializeComponent();
        }

        private void Connect()
        {
            Connected = TryMakeConnection();

            if (Connected)
            {
                SaveFields();
                Close();
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming it was clicked only if we Can Sign In and the Sign In Button is enabled
            Connect();
        }

        private void ServerAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleConnectButton();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleConnectButton();
        }

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleConnectButton();
        }

        private void PasswordTb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ToggleConnectButton();
        }

        private bool TryMakeConnection()
        {
            try
            {
                PcsGlobalInterface.Accessor.Connect(IPAddress.Parse(serverAddressTextBox.Text),
                    new AuthenticationInfos(
                        Convert.ToInt32(idTextBox.Text, CultureInfo.CurrentCulture),
                        passwordTb.Password));

                return true;
            }
            catch (Exception ex)
            {
                PcsGlobalInterface.Accessor.Disconnect(); // Cancel connecting
                MessageBox.Show(ex.Message, $"Connection to {serverAddressTextBox.Text} failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void ToggleConnectButton()
            => connectButton.IsEnabled = CanConnect();

        private bool CanConnect()
            => serverAddressTextBox.Text != string.Empty &&
            passwordTb.Password != string.Empty &&
            idTextBox.Text != string.Empty && int.TryParse(idTextBox.Text, out _) == true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadFields();
            }
            catch (FileNotFoundException) { }
            // DOLATER Get used of other possible exceptions (+everywhere)

            if (AutoConnect && (bool)autoConnectCb.IsChecked)
                Connect();

            ToggleConnectButton();
        }

        private void SaveFields()
        {
            var document = new XDocument(
                new XElement("ConnectionInfos",
                    new XElement("ServerAddress", serverAddressTextBox.Text),
                    new XElement("UserID", idTextBox.Text),
                    new XElement("Password", passwordTb.Password),
                    new XElement("AutoConnect", autoConnectCb.IsChecked)
                )
            );

            document.Save(fieldSavePath);
        }

        private void LoadFields()
        {
            var document = new XmlDocument() ;
            document.Load(fieldSavePath);
            serverAddressTextBox.Text = document.SelectSingleNode("/ConnectionInfos/ServerAddress").InnerXml;
            idTextBox.Text = document.SelectSingleNode("/ConnectionInfos/UserID").InnerXml;
            passwordTb.Password = document.SelectSingleNode("/ConnectionInfos/Password").InnerXml; // DOLATER Make it secured
            autoConnectCb.IsChecked = Convert.ToBoolean(document.SelectSingleNode("/ConnectionInfos/AutoConnect").InnerXml); // DOLATER Make it secured
        }
    }
}
