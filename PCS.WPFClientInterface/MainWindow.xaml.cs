using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace PCS.WPFClientInterface
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly object @lock = new object();

		private PcsAccessor accessor = new PcsAccessor();


		public Member ActiveMember { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Disconnect()
		{
			accessor.Disconnect();
			messageField.Clear();
			channelList.Stop();

			ToggleAll();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ToggleAll();
		}

		private void ConnectMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var signInWindow = new ConnectionWindow(ref accessor);
			signInWindow.ShowDialog();

			if (signInWindow.Connected)
			{
				ActiveMember = signInWindow.SignedInMember;

				accessor.StartListenAsync(MessageReceived, OnChannelReceived); // TODO Handle exceptions here.

				// BBTEMP:
				//channelList.Enable(); 
				channelList.Initialize(accessor, ResetMessageField);

				// Get FTP Messages
				foreach (var dailyMessage in accessor.GetDailyMessages(DateTime.Now))
					messageField.AddMessage(dailyMessage, () => { });

				ToggleAll();
			}

			void MessageReceived(BroadcastMessage broadcastMsg)
			{
				lock (@lock)
				{
					if (broadcastMsg.BaseMessage.ChannelName == accessor.FocusedChannel.Name)
					{
						Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
								messageField.AddMessage(broadcastMsg, () => Notify(broadcastMsg)));
					}
					else
					{
						Dispatcher.Invoke(() => // Otherwise, can't access controls from another thread
							Notify(broadcastMsg));
					}
				}

				void Notify(BroadcastMessage message)
				{
					if (message.Author != ActiveMember)
						PcsNotifier.Notify(this, message); // Notify when it's not us
				}
			}

			void OnChannelReceived(Channel channel)
			{
				// TODO edit a channel
			}
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			accessor.Disconnect();
			Application.Current.Shutdown();
		}

		private void DisconnectMenuItem_Click(object sender, RoutedEventArgs e)
			=> Disconnect(); // TODO: Do similar everywhere

		private void SendMessageButton_Click(object sender, RoutedEventArgs e)
		{
			var message = new Message(messageTextBox.Text, channelList.FocusedChannel.Name);

			try
			{
				accessor.SendMessage(message);
				messageTextBox.Text = string.Empty;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Disconnect();
			}
		}

		private void DisplayPreviousDayButton_Click(object sender, RoutedEventArgs e)
		{
			messageField.SetPreviousDay();

			foreach (var message in accessor.GetDailyMessages(messageField.LastDayLoaded).Reverse())
				messageField.AddMessageOnTop(message);
		}

		private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ToggleSendMessageButton();
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(
				"Project Communication Software (P.C.S.), developed by Paul Wacquet, Thomas Wacquet and Ilian Baylon.\nVersion 0.3",
				"About Project Communication Software",
				MessageBoxButton.OK,
				MessageBoxImage.Information);
		}

		private void ScrollToEndButton_Click(object sender, RoutedEventArgs e)
		{
			messageField.ScrollToEnd();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			accessor.Dispose();
		}

		private void ToggleAll()
		{
			ToggleDisconnectMenuItem();
			ToggleConnectMenuItem();
			ToggleSendMessageButton();
			ToggleDisplayPreviousDayButton();
		}

		private void ToggleSendMessageButton()
			=> sendMessageButton.IsEnabled = messageTextBox.Text != string.Empty
			&& accessor.IsConnected;

		private void ToggleConnectMenuItem()
			=> connectMenuItem.IsEnabled = !accessor.IsConnected;

		private void ToggleDisconnectMenuItem()
			=> disconnectMenuItem.IsEnabled = accessor.IsConnected;

		private void ToggleDisplayPreviousDayButton()
			=> displayPreviousDayButton.IsEnabled = accessor.IsConnected;

		private void TestButton_Click(object sender, RoutedEventArgs e)
		{
		}

		public void ResetMessageField()
		{
			messageField.Clear();

			foreach (var message in accessor.GetDailyMessages(DateTime.Now))
				messageField.AddMessage(message, () => { });
		}

		private void channelList_Loaded(object sender, RoutedEventArgs e)
		{

		}
	}
}
