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
        readonly PCSClient client;
        readonly Thread serverListening;
        readonly List<Message> receivedMessages;
        readonly DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            client = new PCSClient();
            serverListening = new Thread(() => ListenServer());
            receivedMessages = new List<Message>();

            const int refreshTicks = 10;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(refreshTicks);

            InitializeComponent();
        }

        public void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var ipAddress = IPAddress.Parse(ipAddressTextBox.Text);
            
            client.Connect(ipAddress);

            connectButton.Content = "Connected";
            connectButton.IsEnabled = false;
            ipAddressTextBox.IsEnabled = false;
            disconnectButton.IsEnabled = true;
            signInButton.IsEnabled = true;
            usernameTextBox.IsEnabled = true;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            int id = new Random().Next(UInt16.MaxValue);
            var member = new Member(usernameTextBox.Text, id);

            client.SignIn(member);

            signInButton.Content = "Signed In";
            signInButton.IsEnabled = false;
            usernameTextBox.IsEnabled = false;
            msgTextBox.IsEnabled = true;
            sendMsgButton.IsEnabled = true;

            serverListening.Start();
            dispatcherTimer.Start();
        }

        private void SendMsgButton_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(msgTextBox.Text);
            msgTextBox.Text = string.Empty;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            // WARNING: This method crash and is not safe

            client.Disconnect();

            connectButton.IsEnabled = true;
            signInButton.IsEnabled = false;
            ipAddressTextBox.IsEnabled = true;
            usernameTextBox.IsEnabled = false;
            ipAddressTextBox.Text = string.Empty;
            usernameTextBox.Text = string.Empty;
            connectButton.Content = "Connect";
            signInButton.Content = "Sign In";
            disconnectButton.IsEnabled = false;
            msgTextBox.IsEnabled = false;
            sendMsgButton.IsEnabled = false;

            serverListening.Abort(); // TODO: Use a safer way
            dispatcherTimer.Stop();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            msgRtbZone.Text = string.Empty;

            foreach (Message message in receivedMessages)
            {
                msgRtbZone.Text += "@" + message.Author.Username + ": " + message.Text + "\n";
            }
        }

        private void ListenServer()
        {
            while (true)
            {
                var receivedMessage = client.ReceiveServerMessage();
                new MessageHandler(WriteMessage).Invoke(receivedMessage);
            }
        }

        delegate void MessageHandler(Message message);
        void WriteMessage(Message message) // TODO: Separate into another class
        {
            receivedMessages.Add(message);
        }
    }
}
