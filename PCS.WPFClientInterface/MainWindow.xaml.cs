using Microsoft.Win32;
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
        private readonly object @lock = new object();

        private PcsAccessor clientAccessor = new PcsAccessor();

        public Member ActiveMember { get; private set; }

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
            var signInWindow = new ConnectionWindow(ref clientAccessor);
            signInWindow.ShowDialog();

            if (signInWindow.Connected)
            {
                ActiveMember = signInWindow.SignedInMember;

                clientAccessor.StartListenAsync((BroadcastMessage broadcastMsg) => MessageReceived(broadcastMsg));

                channelSelector.Enable();

                // Get FTP Messages
                foreach (var dailyMessage in clientAccessor.GetDailyMessages(channelSelector.SelectedChannel, DateTime.Now))
                    messageField.AddMessage(dailyMessage, () => { });

                ToggleAll();
            }

            void MessageReceived(BroadcastMessage broadcastMsg)
            {
                lock (@lock)
                {
                    Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                            messageField.AddMessage(broadcastMsg, () => Notify(broadcastMsg)));
                }

                void Notify(BroadcastMessage message)
                {
                    if (message.Author != ActiveMember)
                        PcsNotifier.Notify(this, message); // Notify when it's not us
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
            messageField.Clear();

            channelSelector.Disable();

            ToggleAll();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(messageTextBox.Text, channelSelector.SelectedChannel);
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
            //ToggleAddResourceButton();
        }

        private void ToggleSendMessageButton()
            => sendMessageButton.IsEnabled = messageTextBox.Text != string.Empty
            && clientAccessor.IsConnected;

        //private void ToggleAddResourceButton()
        //    => new Action(() {}).Invoke(); addResourceButton.IsEnabled = clientAccessor.IsConnected;

        private void ToggleConnectMenuItem()
            => connectMenuItem.IsEnabled = !clientAccessor.IsConnected;

        private void ToggleDisconnectMenuItem()
            => disconnectMenuItem.IsEnabled = clientAccessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            testButton.IsEnabled = false;
        }

        private void sendTaskButton_Click(object sender, RoutedEventArgs e)
        {
            clientAccessor.SendTask();
        }
    }
}
