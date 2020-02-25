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
	public partial class ChannelList : UserControl
	{
		public Channel FocusedChannel { get => channels[channelList.SelectedIndex]; }
		public List<Channel> channels = new List<Channel>();
		private PcsAccessor m_accessor;
		private Action m_onChannelChanged = () => { };

		public ChannelList()
		{
			InitializeComponent();
		}

		internal void Initialize(PcsAccessor accessor, Action onChannelChanged)
		{
			if (accessor is null)
				throw new ArgumentNullException(nameof(accessor));
			if (onChannelChanged is null)
				throw new ArgumentNullException(nameof(onChannelChanged));

			channels = (List<Channel>)accessor.GetChannels();
			if (channels is null)
				throw new ArgumentNullException(nameof(channels));
			if (channels.DefaultIfEmpty() == default)
				throw new ArgumentException(nameof(channels));

			m_accessor = accessor;
			foreach (var channel in channels)
				Add(channel);

			channelList.SelectedIndex = 0;

			m_onChannelChanged = onChannelChanged;
		}

		internal void Stop()
		{
			channelList.Items.Clear();
		}

		internal void Add(Channel channel)
		{
			channelList.Items.Add(channel.Name);
		}

		private void channelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
			m_accessor.FocusedChannel = FocusedChannel;
			m_onChannelChanged();
		}
	}
}
