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

namespace BookShopYP02.User
{
    /// <summary>
    /// Логика взаимодействия для UnatorizationProductsView.xaml
    /// </summary>
    public partial class UnatorizationProductsView : Window
    {
        public UnatorizationProductsView()
        {
            InitializeComponent();
        }
        private void LoadProducts()
        {
            using (var context = new BookShopEntities1())
            {
                var products = context.Товары.ToList();
                ProductsListView.ItemsSource = products;
            }
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();   
        }
    }
}
