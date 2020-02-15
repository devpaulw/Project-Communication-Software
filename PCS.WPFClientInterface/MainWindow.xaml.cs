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

        private void Disconnect()
        {
            clientAccessor.Disconnect();
            messageField.Clear();

            channelSelector.Disable();

            ToggleAll();
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

                clientAccessor.StartListenAsync((BroadcastMessage broadcastMsg) => MessageReceived(broadcastMsg)); // TODO Handle exceptions here.
                
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
            => Disconnect(); // TODO: Do similar everywhere

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message(messageTextBox.Text, channelSelector.SelectedChannel);

            try
            {
                clientAccessor.SendMessage(message);
                messageTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Disconnect();
            }
        }

        private void DisplayPreviousDayButton_Click(object sender, RoutedEventArgs e)
        {
            messageField.SetPreviousDay();

            foreach (var message in clientAccessor.GetDailyMessages(channelSelector.SelectedChannel, messageField.LastDayLoaded).Reverse())
                messageField.AddMessageOnTop(message);
        }

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
            clientAccessor.Dispose();
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
            && clientAccessor.IsConnected;

        private void ToggleConnectMenuItem()
            => connectMenuItem.IsEnabled = !clientAccessor.IsConnected;

        private void ToggleDisconnectMenuItem()
            => disconnectMenuItem.IsEnabled = clientAccessor.IsConnected;

        private void ToggleDisplayPreviousDayButton()
            => displayPreviousDayButton.IsEnabled = clientAccessor.IsConnected;

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Hello World");

            //string provider = "System.Data.SqlClient";
            //string connStr = @"Data Source=DESKTOP-IQ5GUFM\;Initial Catalog=TESTDV;Integrated Security=True;Pooling=False";
            //DbProviderFactory factory = DbProviderFactories.GetFactory(provider);

            //using (DbConnection connection = factory.CreateConnection())
            //{
            //    if (connection == null)
            //    {
            //        Console.WriteLine("connect error");
            //        Console.ReadLine();
            //        return;
            //    }
            //    connection.ConnectionString = connStr;
            //    connection.Open();
            //    DbCommand command = factory.CreateCommand();

            //    if (command == null)
            //    {
            //        Console.WriteLine("Command error");
            //        Console.ReadLine();
            //        return;
            //    }

            //    command.Connection = connection;
            //    command.CommandText = "Select * From Products";

            //    using (DbDataReader dataReader = command.ExecuteReader())
            //    {
            //        while (dataReader.Read())
            //        {
            //            Console.WriteLine($"Then {dataReader["ProdId"]} {dataReader["Product"]}");
            //            Console.ReadLine();
            //        }
            //    }

            //    DbCommand ct = factory.CreateCommand();
            //    ct.Connection = connection;
            //    ct.CommandText = "INSERT INTO Products (ProdId, Product)" +
            //        "Values (365, 'FromC#')";
            //    ct.ExecuteNonQuery();

            //    //using (DbData)
            //}

            string connStr = @"Data Source=DESKTOP-IQ5GUFM\;Initial Catalog=TESTDV;Integrated Security=True;Pooling=False";
            //string test = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Fichiers personnels\\Documents\\[] Projects\\Project Communication Software\\PCS.WPFClientInterface\\Database1.mdf\";Integrated Security=True";
            
            using (var connection = new SqlConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("connect error");
                    Console.ReadLine();
                    return;
                }

                connection.ConnectionString = connStr;

                connection.Open();

                SqlCommand command = new SqlCommand();

                if (command == null)
                {
                    Console.WriteLine("Command error");
                    Console.ReadLine();
                    return;
                }

                command.Connection = connection;
                command.CommandText = "Select * From Products";

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Console.WriteLine($"Then {dataReader["ProdId"]} {dataReader["Product"]}");
                        Console.ReadLine();
                    }
                }

                command.CommandText = "INSERT INTO Products (ProdId, Product)" +
                    "Values (366, 'FromC#')";

                command.ExecuteNonQuery();
            }

            //using (SqlConnection connection = new SqlConn)

            //String str;
            //SqlConnection myConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

            //str = "CREATE DATABASE MyDatabase ON PRIMARY " +
            //    "(NAME = MyDatabase_Data, " +
            //    "FILENAME = 'C:\\MyDatabaseData.mdf', " +
            //    "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
            //    "LOG ON (NAME = MyDatabase_Log, " +
            //    "FILENAME = 'C:\\MyDatabaseLog.ldf', " +
            //    "SIZE = 1MB, " +
            //    "MAXSIZE = 5MB, " +
            //    "FILEGROWTH = 10%)";

            //SqlCommand myCommand = new SqlCommand(str, myConn);
            //try
            //{
            //    myConn.Open();
            //    myCommand.ExecuteNonQuery();
            //    MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //finally
            //{
            //    if (myConn.State == ConnectionState.Open)
            //        myConn.Close();
            //}

            // your connection string
            //string connectionString = ;

            // your query:
            //var query = GetDbCreationQuery();

            //var conn = new SqlConnection(connectionString);
            //var command = new SqlCommand(query, conn);

            //try
            //{
            //    conn.Open();
            //    command.ExecuteNonQuery();
            //    MessageBox.Show("Database is created successfully", "MyProgram",
            //                    MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            //finally
            //{
            //    if ((conn.State == ConnectionState.Open))
            //    {
            //        conn.Close();
            //    }
            //}

            //string GetDbCreationQuery()
            //{
            //    // your db name
            //    string dbName = "MyDatabase";

            //    // db creation query
            //    string query = "CREATE DATABASE " + dbName + ";";

            //    return query;
            //}
        }
    }
}
