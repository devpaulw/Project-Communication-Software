using System;
using System.Collections.Generic;
using System.Globalization;
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
        const int ShowBeforeCount = 20;

        private int loadedMessagesCount;
        private BroadcastMessage selectedBroadcast;

        public Action<int> OnDeleteBroadcast { get; set; }

        public MessageField()
        {
            InitializeComponent();

            Clear();
        }

        public void AddMessage(BroadcastMessage message)
        {
            var appendParagraph = new BroadcastMessageParagraph(message);

            fieldRtb.Document.Blocks.Add(appendParagraph);

            ScrollToEnd();
        }

        public void AddMessageOnTop(BroadcastMessage message)
        {
            var appendParagraph = new BroadcastMessageParagraph(message);

            if (fieldRtb.Document.Blocks.FirstBlock != null)
                fieldRtb.Document.Blocks.InsertBefore(fieldRtb.Document.Blocks.FirstBlock, appendParagraph);
            else // When no messages in the messageField
                fieldRtb.Document.Blocks.Add(appendParagraph);
        }
        
        /// <param name="getTopMessages">in 1: start, in 2: end, out messages</param>
        public void ShowBefore(Func<int, int, IEnumerable<BroadcastMessage>> getTopMessages)
        {
            loadedMessagesCount += ShowBeforeCount;

            int start = loadedMessagesCount - ShowBeforeCount,
                end = loadedMessagesCount;

            // Get SQL Messages
            foreach (var message in getTopMessages(start, end))
                AddMessageOnTop(message);
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
            loadedMessagesCount = 0;
        }

        public void ScrollToEnd() => fieldRtb.ScrollToEnd();

        private Paragraph GetBroadcastParagraph(BroadcastMessage broadcast)
        {
            string dayFormat = "MMM dd";
            string timeFormat = "t";
            var paragraph = new Paragraph();

            paragraph.Inlines.Add(new Run('@' + broadcast.Author.Username + ' ') { FontWeight = FontWeights.SemiBold, FontSize = 13.5d, Foreground = Brushes.CadetBlue });

            paragraph.Inlines.Add(new Run(string.Format("{0}, {1} ",
                broadcast.DateTime.ToString(dayFormat, CultureInfo.InvariantCulture),
                broadcast.DateTime.ToString(timeFormat, CultureInfo.InvariantCulture))) 
            { FontSize = 11d, Foreground = Brushes.Gray });

            paragraph.Inlines.Add(broadcast.Text);

            paragraph.LineHeight = 3;
            return paragraph;
        }



        private void DeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the {0} ?", selectedBroadcast.ToString()), "Delete message", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OnDeleteBroadcast(selectedBroadcast.ID);
            }
        }

        private void fieldRtb_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (loadedMessagesCount == 0) // When no messages
                e.Handled = true;
            else
            {
                foreach (Block block in fieldRtb.Document.Blocks)
                {
                    if (block.IsMouseOver)
                    {
                        selectedBroadcast = (block as BroadcastMessageParagraph).BroadcastMessage;
                    }
                }
            }
        }

        private class BroadcastMessageParagraph : Paragraph
        {
            public BroadcastMessage BroadcastMessage { get; set; }

            public BroadcastMessageParagraph(BroadcastMessage broadcast)
            {
                BroadcastMessage = broadcast;

                string dayFormat = "MMM dd";
                string timeFormat = "t";

                Inlines.Add(new Run('@' + broadcast.Author.Username + ' ') { FontWeight = FontWeights.SemiBold, FontSize = 13.5d, Foreground = Brushes.CadetBlue });

                Inlines.Add(new Run(string.Format("{0}, {1} ",
                    broadcast.DateTime.ToString(dayFormat, CultureInfo.InvariantCulture),
                    broadcast.DateTime.ToString(timeFormat, CultureInfo.InvariantCulture)))
                { FontSize = 11d, Foreground = Brushes.Gray });

                Inlines.Add(broadcast.Text);

                LineHeight = 3;
            }
        }
    }
}
