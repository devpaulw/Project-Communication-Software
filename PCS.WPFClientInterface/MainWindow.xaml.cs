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
        private PcsClientAccessor clientAccessor = new PcsClientAccessor(@"files");
        private List<Resource> attachedResources = new List<Resource>(); // Add it to a kind of messageField class but for sendmessage TB and send button...
        private string focusedChannelName; // SDNMSG: I advise you do a UserControl like the MessageField example

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

                clientAccessor.StartListenAsync((Message message) => // Start get messages dynamically
                {
                    Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
                        messageField.AddMessage(message, () => Notify(message)));
                });

                foreach (var dailyMessage in clientAccessor.GetDailyMessages(DateTime.Now)) // Get FTP Messages
                    messageField.AddMessage(dailyMessage, () => { });

                ToggleAll();

                // TODO receive the channels from the server
                ChannelList.Items.Add("channel1");
                ChannelList.Items.Add("channel2");
                focusedChannelName = ChannelList.Items[0].ToString();
            }

            void Notify(Message message)
            {
                if (message.Author != ActiveMember)
                    PcsNotifier.Notify(this, message); // Notify when it's not us
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
            
            focusedChannelName = null;
            ChannelList.Items.Clear();

            ToggleAll();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(messageTextBox.Text, focusedChannelName, DateTime.Now, ActiveMember, attachedResources);
            clientAccessor.SendMessage(message);

            messageTextBox.Text = string.Empty;
            attachedResources = new List<Resource>();
        }

        private void AddResourceButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Only images are accepted at this time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);

            string path = null;
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;

            attachedResources.Add(new Resource(localPath: path, type: ResourceType.Image));
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
            ToggleAddResourceButton();
        }

        private void ToggleSendMessageButton()
            => sendMessageButton.IsEnabled = messageTextBox.Text != string.Empty
            && clientAccessor.IsConnected;

        private void ToggleAddResourceButton()
            => addResourceButton.IsEnabled = clientAccessor.IsConnected;

        private void ToggleConnectMenuItem()
            => connectMenuItem.IsEnabled = !clientAccessor.IsConnected;

        private void ToggleDisconnectMenuItem()
            => disconnectMenuItem.IsEnabled = clientAccessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            testButton.IsEnabled = false;

            //PcsNotifier.Notify(this, null);

            //return;

            //clientAccessor.SendMessage(new Message(
            //    "TEST RESOURCE",
            //    "TESTCHANNEL",
            //    DateTime.Now,
            //    new Member("TESTER", 99999),
            //    new List<Resource>() { new Resource(localPath: @"C:\Users\Paul\Pictures\OpenGL-Wide.png", type: ResourceType.Image) }
            //    ));
        }

        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            focusedChannelName = ChannelList.SelectedItem.ToString();
        }
    }
}
