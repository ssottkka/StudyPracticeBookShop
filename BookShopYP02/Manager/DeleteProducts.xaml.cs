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
    /// <summary>
    /// Логика взаимодействия для DeleteProducts.xaml
    /// </summary>
    public partial class DeleteProducts : Window
    {
        public ObservableCollection<Товары> Products { get; set; }
        public ObservableCollection<string> Authors { get; set; }
        public DeleteProducts()
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

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Включаем кнопку "Удалить товар", если выбран товар
            DeleteProductButton.IsEnabled = BooksDataGrid.SelectedItem != null;
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Товары selectedProduct)
            {
                using (var context = new BookShopEntities1())
                {
                    // Находим выбранный товар в базе данных
                    var productToDelete = context.Товары.Find(selectedProduct.Id);

                    if (productToDelete != null)
                    {
                        // Удаляем все связанные записи в таблице "ДеталиЗаказа"
                        var relatedOrderDetails = context.ДеталиЗаказа
                            .Where(d => d.КодТовара == productToDelete.Id)
                            .ToList();

                        foreach (var detail in relatedOrderDetails)
                        {
                            context.ДеталиЗаказа.Remove(detail);
                        }

                        // Удаляем товар из базы данных
                        context.Товары.Remove(productToDelete);
                        context.SaveChanges();

                        // Удаляем товар из списка отображаемых товаров
                        Products.Remove(selectedProduct);

                        // Выводим сообщение об успешном удалении
                        MessageBox.Show("Товар успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow managerWindow = new ManagerWindow();  
            managerWindow.Show();   
            this.Close();   
        }
    }
}
