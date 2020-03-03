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

        public static PcsAccessor Accessor = new PcsAccessor();

        public MainWindow()
        {
            InitializeComponent();

            messageField.OnDeleteBroadcast = delegate (int messageId)
            {
                Accessor.DeleteMessage(messageId);
                messageField.Clear();
                ShowBefore(); // TODO local delete easier
            };
        }

        private void Disconnect()
        {
            Accessor.Disconnect();
            messageField.Clear();
            channelSelector.Disable();
            ToggleAll();
        }

        public void SendMessage(SendableMessage message)
        {
            try
            {
                Accessor.SendMessage(message);
                messageTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Disconnect();
            }
        }

        private void ShowBefore()
        {
            // Get SQL Messages
            messageField.ShowBefore((start, end) => Accessor.GetTopMessagesInRange(start, end, channelSelector.SelectedChannel));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleAll();
        }

        private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var connWindow = new ConnectionWindow(ref Accessor);
            connWindow.ShowDialog();

            if (connWindow.Connected)
            {
                Accessor.MessageReceive += OnMessageReceive;
                Accessor.ListenException += OnServerListenException;

                channelSelector.Enable();

                ShowBefore();

                ToggleAll();
            }
        }

        void OnMessageReceive(object sender, BroadcastMessage broadcastMsg)
        {
            lock (@lock)
            {
                Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                        messageField.AddMessage(broadcastMsg));
                Notify(broadcastMsg);
            }
        }

        void OnServerListenException(object sender, Exception exception)
        {
            MessageBox.Show(exception.Message, "Transmission exception", MessageBoxButton.OK, MessageBoxImage.Error);
            Dispatcher.Invoke(() => Disconnect());
        }

        void Notify(BroadcastMessage broadcastMsg)
        {
            if (broadcastMsg.Author.ID != Accessor.ActiveMemberId)
                PcsNotifier.Notify(this, broadcastMsg); // Notify when it's not us
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Accessor.Disconnect();
            Application.Current.Shutdown();
        }

        private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
            => Disconnect();

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new SendableMessage(messageTextBox.Text, channelSelector.SelectedChannel);
            SendMessage(message);
        }

        private void ShowBeforeButton_Click(object sender, RoutedEventArgs e)
            => ShowBefore();

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToggleSendMessageButton();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Project Communication Software (P.C.S.), developed by Paul Wacquet, Thomas Wacquet and Ilian Baylon.\nVersion 0.4",
                "About Project Communication Software",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Accessor.Dispose();
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
            && Accessor.IsConnected;

        private void ToggleConnectMenuItem()
            => connectMenuItem.IsEnabled = !Accessor.IsConnected;

        private void ToggleDisconnectMenuItem()
            => disconnectMenuItem.IsEnabled = Accessor.IsConnected;

        private void ToggleDisplayPreviousDayButton()
            => displayPreviousDayButton.IsEnabled = Accessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Accessor.DeleteMessage(67);
            Accessor.ModifyMessage(66, new SendableMessage("Modifié", "channel1"));
            messageField.Clear();// TODO Reset func, handle better MessageField //TODO MAKE MessageField adapted to Modify and Remove message // TODO In messageField use List<Broadcast> to handle them and with an update system
            ShowBefore();
        }
    }
}
