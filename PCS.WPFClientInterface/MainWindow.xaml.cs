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
        private PcsClientAccessor clientAccessor;

        public MainWindow()
        {
            clientAccessor = new PcsClientAccessor();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SignInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var signInWindow = new SignInWindow();
            signInWindow.ShowDialog();

            if (signInWindow.TryMakeSignIn(ref clientAccessor))
            {
                clientAccessor.StartListenAsync( // Start get messages dynamically
                    delegate (Message message)
                    {
                        Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                        {
                            messageField.AddMessage(message);
                            messageField.ScrollToEnd();
                        });
                    });

                foreach (var dailyMessage in clientAccessor.Ftp.GetDailyMessages(DateTime.Now)) // Get FTP Messages
                {
                    messageField.AddMessage(dailyMessage);
                }
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
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(messageTextBox.Text, "Default"/*tmp*/);
            clientAccessor.SendMessage(message);

            messageTextBox.Text = string.Empty;
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TryEnableSendMessageButton();

            void TryEnableSendMessageButton()
            {
                if (messageTextBox.Text == string.Empty)
                    sendMessageButton.IsEnabled = false;
                else
                    sendMessageButton.IsEnabled = true;
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Project Communication Software (P.C.S.), developed by SpydotNet and Binary Bluff...",
                "About Project Communication Software",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }

        private void ScrollToEndButton_Click(object sender, RoutedEventArgs e)
        {
            messageField.ScrollToEnd();
        }
    }
}
