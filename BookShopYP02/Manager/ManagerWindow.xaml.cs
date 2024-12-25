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

namespace BookShopYP02.Manager
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(); 
            main.Show();
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProducts addProducts = new AddProducts();    
            addProducts.Show(); 
            this.Close();   
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteProducts deleteProducts = new DeleteProducts();   
            deleteProducts.Show();  
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
    }
}
