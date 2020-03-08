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
	/// Interaction logic for ProjectSelector.xaml
	/// </summary>
	public partial class ProjectSelector : UserControl
	{
		public ProjectSelector()
		{
			InitializeComponent();
			IsEnabled = false;
		}

		public void Enable()
		{
			IsEnabled = true;
			// BBTODO: retrieve from server
			projectSelector.ItemsSource = new List<string>()
			{
				"project1",
				"project2"
			};
			projectSelector.SelectedIndex = 0;
		}
		public void Disable()
		{
			IsEnabled = false;
			projectSelector.ItemsSource = null;
		}


		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsEnabled)
			{
				//BBTODO change the project
			}
		}
	}
}
