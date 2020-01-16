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

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IndevClass ic;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //ok button from ip address
        {
            IPAddress ipAddress;
            int port;

            try
            {
                ipAddress = IPAddress.Parse(ipAddressTextBox.Text);
                port = Convert.ToInt32(portTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server connection error");
                return;
            }

            // If ipAddress and port are correct
            ic = new IndevClass(ipAddress, port, AddMessage);

            // Listen thread start
            Thread listenThread = new Thread(() => ic.Listen());
            listenThread.Start();

            connectButton.IsEnabled = false;
        }

        private void AddMessage(DateTime dateTime, Member member, string message, Resource resource = null)
        {
            MessageBox.Show(message);
        }
    }

    class Resource { } // TEMP
}
