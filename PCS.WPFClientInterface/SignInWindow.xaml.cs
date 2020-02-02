using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }

        public bool TryMakeConnection(ref PcsClientAccessor clientAccessor, out Member member)
        {
            member = new Member(usernameTextBox.Text,
                Convert.ToInt32(idTextBox.Text, CultureInfo.CurrentCulture));

            if (CanSignIn())
            {
                clientAccessor.Connect(IPAddress.Parse(serverAddressTextBox.Text), member);

                return true;
            }
            else
                return false;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ServerAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSignInButton();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSignInButton();
        }

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSignInButton();
        }

        private void ToggleSignInButton()
            => signInButton.IsEnabled = CanSignIn();

        private bool CanSignIn()
        {
            return serverAddressTextBox.Text != string.Empty &&
                usernameTextBox.Text != string.Empty &&
                idTextBox.Text != string.Empty && int.TryParse(idTextBox.Text, out _) == true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            serverAddressTextBox.Text = "127.0.0.1";
            usernameTextBox.Text = "PcsTester1";
            idTextBox.Text = "54312";

            ToggleSignInButton();
        }
    }
}
