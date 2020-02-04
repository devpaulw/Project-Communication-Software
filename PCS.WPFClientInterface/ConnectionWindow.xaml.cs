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

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        const string fieldSavePath = "./connection_infos.xml";

        private readonly PcsClientAccessor m_clientAccessor;

        public bool Connected { get; private set; }
        public Member SignedInMember { get; private set; }

        public ConnectionWindow(ref PcsClientAccessor clientAccessor)
        {
            m_clientAccessor = clientAccessor;

            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming it was clicked only if we Can Sign In and the Sign In Button is enabled

            var member = new Member(usernameTextBox.Text,
                Convert.ToInt32(idTextBox.Text, CultureInfo.CurrentCulture));

            Connected = TryMakeConnection(member);

            if (Connected)
            {
                SignedInMember = member;
                SaveFields();
                Close();
            }

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

        private bool TryMakeConnection(Member member)
        {
            try
            {
                m_clientAccessor.Connect(IPAddress.Parse(serverAddressTextBox.Text), member);

                return true;
            }
            catch (SocketException ex) // Connection failed
            {
                MessageBox.Show(ex.Message, $"Connection to {serverAddressTextBox.Text} failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, $"Connection to {serverAddressTextBox.Text} failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void ToggleConnectButton()
            => connectButton.IsEnabled = CanConnect();

        private bool CanConnect()
            => serverAddressTextBox.Text != string.Empty &&
            usernameTextBox.Text != string.Empty &&
            idTextBox.Text != string.Empty && int.TryParse(idTextBox.Text, out _) == true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //serverAddressTextBox.Text = "127.0.0.1";
            //usernameTextBox.Text = "PcsTester1";
            //idTextBox.Text = "54312";
            try
            {
                LoadFields();
            }
            catch (FileNotFoundException) { }

            ToggleConnectButton();
        }

        private void SaveFields()
        {
            var document = new XDocument(
                new XElement("ConnectionInfos",
                    new XElement("ServerAddress", serverAddressTextBox.Text),
                    new XElement("Username", usernameTextBox.Text),
                    new XElement("ID", idTextBox.Text)
                )
            );

            document.Save(fieldSavePath);
        }

        private void LoadFields()
        {
            var document = new XmlDocument();
            document.Load(fieldSavePath);
            serverAddressTextBox.Text = document.SelectSingleNode("/ConnectionInfos/ServerAddress").InnerXml;
            usernameTextBox.Text = document.SelectSingleNode("/ConnectionInfos/Username").InnerXml;
            idTextBox.Text = document.SelectSingleNode("/ConnectionInfos/ID").InnerXml;
        }
    }
}
