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
using System.Windows.Shapes;

namespace PCS.WPFClientInterface
{
    /// <summary>
    /// Logique d'interaction pour NewToDoListWindow.xaml
    /// </summary>
    public partial class NewToDoListWindow : Window
    {
        private readonly PcsAccessor m_clientAccessor;

        public NewToDoListWindow(ref PcsAccessor clientAccessor)
        {
            m_clientAccessor = clientAccessor;

            InitializeComponent();
            //ToggleCreateButton();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ToDoList toDoList = new ToDoList
            {
                Name = nameTextBox.Text
            };
            m_clientAccessor.SendToDoList(toDoList);
            Close();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine(nameTextBox.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
