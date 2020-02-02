using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PcsClientAccessor clientAccessor = new PcsClientAccessor();
        private Member activeMember;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleAll();
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var signInWindow = new SignInWindow();
            signInWindow.ShowDialog();

            if (signInWindow.TryMakeConnection(ref clientAccessor, out activeMember))
            {
                clientAccessor.StartListenAsync( // Start get messages dynamically
                    delegate (Message message)
                    {
                        Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                            messageField.AddMessage(message));
                    });

                foreach (var dailyMessage in clientAccessor.Ftp.GetDailyMessages(DateTime.Now)) // Get FTP Messages
                    messageField.AddMessage(dailyMessage);

                ToggleAll();
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            clientAccessor.Disconnect();
            Application.Current.Shutdown();
        }

        private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            clientAccessor.Disconnect();
            messageField.Clear();
            ToggleAll();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(messageTextBox.Text, "Default"/*tmp*/, DateTime.Now, activeMember, null);
            clientAccessor.SendMessage(message);

            messageTextBox.Text = string.Empty;
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSendMessageButton();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Project Communication Software (P.C.S.), developed by SpydotNet and Binary Bluff.\n[...]",
                "About Project Communication Software",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ScrollToEndButton_Click(object sender, RoutedEventArgs e)
        {
            messageField.ScrollToEnd();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clientAccessor.Dispose();
        }

        private void ToggleAll()
        {
            ToggleDisconnectMenuItem();
            ToggleConnectMenuItem();
            ToggleSendMessageButton();
        }

        private void ToggleSendMessageButton()
            => sendMessageButton.IsEnabled = messageTextBox.Text != string.Empty && clientAccessor.IsConnected;

        private void ToggleConnectMenuItem() 
            => connectMenuItem.IsEnabled = !clientAccessor.IsConnected;

        private void ToggleDisconnectMenuItem() 
            => disconnectMenuItem.IsEnabled = clientAccessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            clientAccessor.SendMessage(new Message(
                "TEST RESOURCE",
                "TESTCHANNEL",
                DateTime.Now,
                new Member("TESTER", 99999),
                new List<string>() { @"C:\Users\Paul\Pictures\OpenGL-Wide.png" }
                ));
        }
    }
}
