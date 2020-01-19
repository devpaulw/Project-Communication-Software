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
        // WARNING: L'application WPF est complètement instable, lente, tout est mal géré et nécessite de recommencer à zero.

        PcsClient server;
        Thread serverListening;
        readonly List<Message> receivedMessages;
        readonly DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            serverListening = new Thread(() => ListenServer());
            receivedMessages = new List<Message>();

            const int refreshTicks = 50;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(refreshTicks);

            InitializeComponent();
        }

        public void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var ipAddress = IPAddress.Parse(ipAddressTextBox.Text);

            server = new PcsClient();
            server.Connect(ipAddress);

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

            server.SignIn(member);

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
            if (msgTextBox.Text == "bell") // Troll
            {
                server.SendText(msgTextBox.Text + ((char)7).ToString());
                return;
            }
                

            server.SendText(msgTextBox.Text);
            msgTextBox.Text = string.Empty;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            server.Disconnect();
            Environment.Exit(0);

            //// Below is a way to restart the app; Before we make this WPF App more reliable.
            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
            //Window_Closed(this, null);

            // Below deprecated (too buggy)
            // WARNING: This method crash and is not safe
            //connectButton.IsEnabled = true;
            //signInButton.IsEnabled = false;
            //ipAddressTextBox.IsEnabled = true;
            //usernameTextBox.IsEnabled = false;
            //ipAddressTextBox.Text = string.Empty;
            //usernameTextBox.Text = string.Empty;
            //connectButton.Content = "Connect";
            //signInButton.Content = "Sign In";
            //disconnectButton.IsEnabled = false;
            //msgTextBox.IsEnabled = false;
            //sendMsgButton.IsEnabled = false;

            //serverListening.Abort(); // TODO: Use a safer way
            //serverListening = new Thread(() => ListenServer());
            //dispatcherTimer.Stop();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            msgRtbZone.Text = string.Empty;

            foreach (Message message in receivedMessages)
            {
                msgRtbZone.Text += "@" + message.Author.Username + ": " + message.Text + "\n";
            }

            msgRtbZone.ScrollToEnd();
        }

        private void ListenServer()
        {
            while (true)
            {
                //var receivedMessage = server.ReceiveMessage();
                var receivedMessage = Message.FromTextData(server.Receive());
                new MessageHandler(WriteMessage).Invoke(receivedMessage);
            }
        }

        delegate void MessageHandler(Message message);
        void WriteMessage(Message message) // TODO: Separate into another class
        {
            receivedMessages.Add(message);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            serverListening.Abort();
            dispatcherTimer.Stop();
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}
