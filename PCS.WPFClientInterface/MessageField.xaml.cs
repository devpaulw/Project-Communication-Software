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
        const int ImageHeight = 260;

        public MessageField()
        {
            InitializeComponent();

            Clear();
        }

        public void AddMessage(Message message, bool notify, MainWindow mainWindow) // TODO: mainWindow argument is weird for notify
        {
            if (notify)
                PcsNotifier.Notify(mainWindow, message);

            var appendParagraph = new Paragraph();
            appendParagraph.Inlines.Add($"@{message.Author.Username} <{message.ChannelTitle}> [{message.DateTime.ToLongTimeString()}]: {message.Text}");
            appendParagraph.LineHeight = 3;

            fieldRtb.Document.Blocks.Add(appendParagraph);

            if (message.AttachedResources != null)
            {
                foreach (Resource attachedResource in message.AttachedResources)
                    if (attachedResource.Type == ResourceType.Image)
                        AddImage(new BitmapImage(attachedResource.FtpUri));
            }

            ScrollToEnd();
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

        public void Clear() => fieldRtb.Document.Blocks.Clear();

        public void ScrollToEnd() => fieldRtb.ScrollToEnd();
    }
}
