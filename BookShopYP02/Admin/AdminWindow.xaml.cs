using BookShopYP02.Manager;
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

namespace BookShopYP02.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.AddProducts addProducts = new Manager.AddProducts();
            addProducts.Show();
            this.Close();   
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditProducts editProducts = new EditProducts(); 
            editProducts.Show();
            this.Close();
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            EditOrder editOrder = new EditOrder();
            editOrder.Show();   
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteProducts deleteProducts = new DeleteProducts();   
            deleteProducts.Show();  
            this.Close();   
        }

        private void ViewListButton_Click(object sender, RoutedEventArgs e)
        {
            CheckProductsList checkProductsList = new CheckProductsList();  
            checkProductsList.Show();   
            this.Close();   
        }

        private void ConfirmOrderstButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmOrder confirmOrder = new ConfirmOrder();
            confirmOrder.Show();    
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
