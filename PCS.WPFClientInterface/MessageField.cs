using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PCS.WPFClientInterface
{
    static class MessageField
    {
        public static void AddMessage(this TextBox field, Message message)
        {
            field.Text += $"@{message.Author.Username} <{message.ChannelTitle}> [{message.DateTime.ToLongTimeString()}]: {message.Text} \n";
        }
    }
}
