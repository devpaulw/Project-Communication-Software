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
	/// Interaction logic for ChannelList.xaml
	/// </summary>
	public partial class ChannelSelector : UserControl
	{
		public string SelectedChannel { get => ChannelList.SelectedItem.ToString(); }

		public ChannelSelector()
		{
			InitializeComponent();
		}

		public void Add(Channel channel)
		{
			ChannelList.Items.Add(channel.Name);
			if (ChannelList.Items.Count == 1)
			{

			}
		}

		public void Enable()
		{
			// TODO retrieve the channels from the server
			if (ChannelList.Items.Count == 0)
			{
				throw new Exception("Channels not received");
			}
		}

		public void Disable()
		{
			ChannelList.Items.Clear();
		}

		internal void Initialize(IEnumerable<Channel> channels)
		{
			if (channels.DefaultIfEmpty() == default)
				throw new ArgumentNullException(nameof(channels));

			foreach (var channel in channels)
				Add(channel);

			ChannelList.SelectedItem = ChannelList.Items[0];
		}
	}
}
