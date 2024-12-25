using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class CheckProductsList : Window
    {
        public ObservableCollection<Товары> Products { get; set; }
        public ObservableCollection<string> Authors { get; set; }

        public CheckProductsList()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new BookShopEntities1())
            {
                // Загрузка всех товаров
                Products = new ObservableCollection<Товары>(context.Товары.ToList());

                // Загрузка уникальных авторов для ComboBox
                Authors = new ObservableCollection<string>(context.Товары
                    .Select(p => p.Автор)
                    .Distinct()
                    .ToList());

                // Привязка данных к DataGrid и ComboBox
                BooksDataGrid.ItemsSource = Products;
                AuthorFilterComboBox.ItemsSource = Authors;
            }
        }

        private void AuthorFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorFilterComboBox.SelectedItem != null)
            {
                var selectedAuthor = AuthorFilterComboBox.SelectedItem.ToString();
                BooksDataGrid.ItemsSource = Products
                    .Where(p => p.Автор == selectedAuthor)
                    .ToList();
            }
            else
            {
                BooksDataGrid.ItemsSource = Products;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            BooksDataGrid.ItemsSource = Products
                .Where(p => p.Наименование.ToLower().Contains(searchText))
                .ToList();
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            ManagerWindow managerWindow = new ManagerWindow();
            managerWindow.Show();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow managerWindow = new ManagerWindow();  
            managerWindow.Show();   
            this.Close();   
        }
    }
}