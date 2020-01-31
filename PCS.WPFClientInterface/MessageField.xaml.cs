using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MessageField.xaml
    /// </summary>
    public partial class MessageField : UserControl
    {
        public MessageField()
        {
            InitializeComponent();
        }

        public void AddMessage(Message message)
        {
            fieldTextBox.Text += $"@{message.Author.Username} <{message.ChannelTitle}> [{message.DateTime.ToLongTimeString()}]: {message.Text} \n";
        }

        public void ScrollToEnd() => fieldTextBox.ScrollToEnd();
    }
}
