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

            fieldRtb.Document.Blocks.Clear();
        }

        public void AddMessage(Message message)
        {
            if (message.Text == "imgtest")
            {
                AddImage(new BitmapImage(new Uri(@"D:\Fichiers personnels\Images\strucpp4.png", UriKind.RelativeOrAbsolute)));
            }

            var appendParagraph = new Paragraph();
            appendParagraph.Inlines.Add($"@{message.Author.Username} <{message.ChannelTitle}> [{message.DateTime.ToLongTimeString()}]: {message.Text}");
            appendParagraph.LineHeight = 3;

            fieldRtb.Document.Blocks.Add(appendParagraph);
        }

        public void AddImage(BitmapImage bitmap)
        {
            var image = new Image();
            image.Source = bitmap;
            image.Width = bitmap.Width; // TODO: Fix a constant size
            image.Height = bitmap.Height;

            var container = new InlineUIContainer(image);
            var paragraph = new Paragraph(container);
            fieldRtb.Document.Blocks.Add(paragraph);
        }

        public void Clear() => fieldRtb.Document.Blocks.Clear();

        public void ScrollToEnd() => fieldRtb.ScrollToEnd();
    }
}
