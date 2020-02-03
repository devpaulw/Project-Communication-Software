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

            fieldRtb.Document.Blocks.Clear();
        }

        public void AddMessage(Message message)
        {
            var appendParagraph = new Paragraph();
            appendParagraph.Inlines.Add($"@{message.Author.Username} <{message.ChannelTitle}> [{message.DateTime.ToLongTimeString()}]: {message.Text}");
            appendParagraph.LineHeight = 3;

            fieldRtb.Document.Blocks.Add(appendParagraph);

            if (message.AttachedResources != null)
            {
                foreach (Resource attachedResource in message.AttachedResources)
                    AddImage(new BitmapImage(attachedResource.FtpUri));
            }

            ScrollToEnd();

            if (message.Text == "imgtestv2")
            {
                //AddImage(new BitmapImage(new Uri(@"D:\Fichiers personnels\Images\Paysages\Paysage_(landscape)_wallpaper_HD_0024.jpg", UriKind.RelativeOrAbsolute)));
                AddImage(new BitmapImage(
                    new Uri(@"ftp://127.0.0.1:6784/resources/steve.png", UriKind.RelativeOrAbsolute)));
            }
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
