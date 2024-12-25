using Microsoft.Win32;
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
    /// Логика взаимодействия для EditProducts.xaml
    /// </summary>
    public partial class EditProducts : Window
    {
        private BookShopEntities1 _context;
        public ObservableCollection<Товары> Products { get; set; }
        public ObservableCollection<string> Authors { get; set; }
        private Товары _selectedProduct;
        public EditProducts()
        {
            InitializeComponent();
            _context = new BookShopEntities1();
            LoadData();

        }

        private void LoadData()
        {
            // Загрузка всех товаров
            Products = new ObservableCollection<Товары>(_context.Товары.ToList());

            // Загрузка уникальных авторов для ComboBox
            Authors = new ObservableCollection<string>(_context.Товары
                .Select(p => p.Автор)
                .Distinct()
                .ToList());

            // Привязка данных к DataGrid и ComboBox
            BooksDataGrid.ItemsSource = Products;
            AuthorFilterComboBox.ItemsSource = Authors;
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
            _selectedProduct = BooksDataGrid.SelectedItem as Товары;
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалоговое окно для выбора новой картинки
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Устанавливаем новую картинку
                if (_selectedProduct != null)
                {
                    _selectedProduct.Картинка = openFileDialog.FileName;

                    // Обновляем только выделенную строку
                    var index = Products.IndexOf(_selectedProduct);
                    if (index != -1)
                    {
                        Products[index] = new Товары
                        {
                            Id = _selectedProduct.Id,
                            Автор = _selectedProduct.Автор,
                            Наименование = _selectedProduct.Наименование,
                            РазмерСкидки = _selectedProduct.РазмерСкидки,
                            Количество = _selectedProduct.Количество,
                            Цена = _selectedProduct.Цена,
                            Картинка = _selectedProduct.Картинка,
                            Производители = _selectedProduct.Производители
                        };
                    }
                }
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var product in Products)
            {
                // Находим товар по его идентификатору
                var productToUpdate = _context.Товары.Find(product.Id);

                if (productToUpdate != null)
                {
                    // Обновляем данные товара
                    productToUpdate.Автор = product.Автор;
                    productToUpdate.Наименование = product.Наименование;
                    productToUpdate.РазмерСкидки = product.РазмерСкидки;
                    productToUpdate.Количество = product.Количество;
                    productToUpdate.Цена = product.Цена;
                    productToUpdate.Картинка = product.Картинка;

                    // Обновляем производителя
                    if (product.Производители != null && !string.IsNullOrEmpty(product.Производители.Наименование))
                    {
                        // Ищем производителя по его наименованию
                        var manufacturer = _context.Производители
                            .FirstOrDefault(m => m.Наименование == product.Производители.Наименование);

                        if (manufacturer != null)
                        {
                            // Если производитель найден, обновляем связь
                            productToUpdate.КодПроизводителя = manufacturer.Код;
                        }
                        else
                        {
                            // Если производитель не найден, создаем нового
                            var newManufacturer = new Производители
                            {
                                Наименование = product.Производители.Наименование
                            };
                            _context.Производители.Add(newManufacturer);
                            _context.SaveChanges(); // Сохраняем нового производителя

                            // Обновляем связь с новым производителем
                            productToUpdate.КодПроизводителя = newManufacturer.Код;
                        }
                    }
                }
            }

            // Сохраняем изменения в базе данных
            _context.SaveChanges();

            // Выводим сообщение об успешном сохранении
            MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow managerWindow = new ManagerWindow();  
            managerWindow.Show();   
            this.Close();
        }
    }
}
