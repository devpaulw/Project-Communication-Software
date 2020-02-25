using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using PCS.Data;

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Interaction logic for MessageField.xaml
    /// </summary>
    public partial class MessageField : UserControl
    {
        const int ImageHeight = 260;

        public int LoadedMessagesCount { get; set; }
        public int ShowBeforeCount => 20;

        public MessageField() // TODO Implement ClientAccessor reference for this class it will be simpler for GetDailyMessages
        {
            InitializeComponent();

            Clear();
        }

        public void AddMessage(BroadcastMessage message, Action notify)
        {
            notify();

            var appendParagraph = new Paragraph();
            appendParagraph.Inlines.Add($"@{message.Author.Username} <{message.BaseMessage.ChannelName}> [{message.DateTime.ToLongTimeString()}]: {message.BaseMessage.Text}");
            appendParagraph.LineHeight = 3;

            fieldRtb.Document.Blocks.Add(appendParagraph);

            ScrollToEnd();
        }

        public void AddMessageOnTop(BroadcastMessage message)
        {
            var appendParagraph = new Paragraph();
            appendParagraph.Inlines.Add($"@{message.Author.Username} <{message.BaseMessage.ChannelName}> [{message.DateTime.ToLongTimeString()}]: {message.BaseMessage.Text}");
            appendParagraph.LineHeight = 3;

            if (fieldRtb.Document.Blocks.FirstBlock != null)
                fieldRtb.Document.Blocks.InsertBefore(fieldRtb.Document.Blocks.FirstBlock, appendParagraph);
            else // When no messages in the messageField
                fieldRtb.Document.Blocks.Add(appendParagraph);
        }

        public void AddImage(BitmapImage bitmap) 
        {
            double sizeRatio = ImageHeight / bitmap.Height;

            var image = new Image
            {
                Source = bitmap,
                Height = ImageHeight,
                Width = bitmap.Width * sizeRatio
            };

            var container = new InlineUIContainer(image);
            var paragraph = new Paragraph(container);

            fieldRtb.Document.Blocks.Add(paragraph);
        }

        public void Clear()
        {
            fieldRtb.Document.Blocks.Clear();
            LoadedMessagesCount = 0;
        }

        public void ScrollToEnd() => fieldRtb.ScrollToEnd();
    }
}
