using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
using PCS.Data;

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object @lock = new object();

        private PcsAccessor accessor = new PcsAccessor();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Disconnect()
        {
            accessor.Disconnect();
            accessor.MessageReceive -= OnMessageReceive;

            messageField.Clear();

            channelSelector.Disable();

            ToggleAll();
        }

        private void ShowMessagesBefore()
        {
            // TODO Not clean
            messageField.LoadedMessagesCount += messageField.ShowBeforeCount;

            // Get SQL Messages
            foreach (var message in accessor.GetTopMessagesInRange(messageField.LoadedMessagesCount - messageField.ShowBeforeCount, messageField.LoadedMessagesCount, channelSelector.SelectedChannel))
                messageField.AddMessageOnTop(message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleAll();
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var connWindow = new ConnectionWindow(ref accessor);
            connWindow.ShowDialog();

            if (connWindow.Connected)
            {
                accessor.MessageReceive += OnMessageReceive;

                channelSelector.Enable();

                ShowMessagesBefore();

                ToggleAll();
            }
        }

        void OnMessageReceive(object sender, BroadcastMessage broadcastMsg)
        {
            lock (@lock)
            {
                Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                        messageField.AddMessage(broadcastMsg, () => Notify(broadcastMsg)));
            }
        }

        void Notify(BroadcastMessage broadcastMsg)
        {
            if (broadcastMsg.Author.ID != accessor.ActiveUserId)
                PcsNotifier.Notify(this, broadcastMsg); // Notify when it's not us
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            accessor.Disconnect();
            Application.Current.Shutdown();
        }

        private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
            => Disconnect(); // TODO: Do similar everywhere

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new SendableMessage(messageTextBox.Text, channelSelector.SelectedChannel);

            try
            {
                accessor.SendMessage(message);
                messageTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Disconnect();
            }
        }

        private void ShowBeforeButton_Click(object sender, RoutedEventArgs e)
            => ShowMessagesBefore();

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSendMessageButton();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Project Communication Software (P.C.S.), developed by Paul Wacquet, Thomas Wacquet and Ilian Baylon.\nVersion 0.3",
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
            accessor.Dispose();
        }

        private void ToggleAll()
        {
            ToggleDisconnectMenuItem();
            ToggleConnectMenuItem();
            ToggleSendMessageButton();
            ToggleDisplayPreviousDayButton();
        }

        private void ToggleSendMessageButton()
            => sendMessageButton.IsEnabled = messageTextBox.Text != string.Empty
            && accessor.IsConnected;

        private void ToggleConnectMenuItem()
            => connectMenuItem.IsEnabled = !accessor.IsConnected;

        private void ToggleDisconnectMenuItem()
            => disconnectMenuItem.IsEnabled = accessor.IsConnected;

        private void ToggleDisplayPreviousDayButton()
            => displayPreviousDayButton.IsEnabled = accessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            accessor.DeleteMessage(14);
            accessor.ModifyMessage(38, new SendableMessage("Modifié", "channel1"));
            messageField.Clear();// TODO Reset func, handle better MessageField //TODO MAKE MessageField adapted to Modify and Remove message // TODO In messageField use List<Broadcast> to handle them and with an update system
            ShowMessagesBefore();
        }
    }
}
