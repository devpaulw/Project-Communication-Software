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
		public Channel FocusedChannel { 
			get {
				if (ChannelList.SelectedItem is null)
					return null;

				return new Channel(ChannelList.SelectedItem.ToString());
			}
		}

		public ChannelSelector()
		{
			InitializeComponent();
		}

		public void Enable()
		{
			// BBTODO retrieve the channels from the server
			ChannelList.Items.Add("channel1");
			ChannelList.Items.Add("channel2");
			ChannelList.SelectedItem = ChannelList.Items[0];
		}

		public void Disable()
		{
			ChannelList.Items.Clear();
		}

		private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PcsGlobalInterface.FocusedChannel = FocusedChannel;
		}
	}
}
