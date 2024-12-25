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
using QRCoder;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;


namespace BookShopYP02.User
{
    public partial class Catalog : Window
    {
        private Dictionary<Товары, int> _selectedProducts = new Dictionary<Товары, int>();
        private Discount _discountService; // Экземпляр класса Discount
        private int _clientId; // ID клиента

        public Catalog(int clientId)
        {
            InitializeComponent();
            _discountService = new Discount(); // Создаем экземпляр Discount
            LoadCategories();
            LoadProducts();
            _clientId = clientId; // Сохраняем ID клиента
        }

        private void LoadCategories()
        {
            using (var context = new BookShopEntities1())
            {
                var categories = context.КатегорииТоваров.ToList();
                categories.Insert(0, new КатегорииТоваров { Код = 0, Название = "Все категории" });
                CategoryFilterComboBox.ItemsSource = categories;
                CategoryFilterComboBox.DisplayMemberPath = "Название";
                CategoryFilterComboBox.SelectedIndex = 0;
            }
        }

        private void LoadProducts()
        {
            using (var context = new BookShopEntities1())
            {
                var products = context.Товары.ToList();
                if (CategoryFilterComboBox.SelectedItem != null && CategoryFilterComboBox.SelectedIndex != 0)
                {
                    var selectedCategory = CategoryFilterComboBox.SelectedItem as КатегорииТоваров;
                    products = products.Where(p => p.КодКатегории == selectedCategory.Код).ToList();
                }
                ProductsListView.ItemsSource = products;
            }
        }

        private void OnAddToOrderButtonClick(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).Tag as Товары;
            if (product != null)
            {
                AddProductToCart(product);
            }
        }

        private void OnRemoveFromOrderButtonClick(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).Tag as Товары;
            if (product != null && _selectedProducts.ContainsKey(product))
            {
                _selectedProducts[product]--;
                if (_selectedProducts[product] == 0)
                {
                    _selectedProducts.Remove(product);
                }
                UpdateSelectedProductsList();
                MessageBox.Show($"Товар '{product.Наименование}' удален из заказа.");
            }
        }

        private void UpdateSelectedProductsList()
        {
            SelectedProductsListBox.ItemsSource = _selectedProducts.Select(p => new { p.Key.Наименование, Количество = p.Value }).ToList();
        }

        private void OnCreateOrderButtonClick(object sender, RoutedEventArgs e)
        {
            if (_selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите товары для заказа.");
                return;
            }

            // Рассчитываем общую сумму заказа с учетом скидок
            decimal totalAmount = _discountService.CalculateTotalOrderAmount(_selectedProducts);

            using (var context = new BookShopEntities1())
            {
                // Получаем адрес пользователя по его ID
                var client = context.Покупатели.FirstOrDefault(c => c.Код == _clientId);
                if (client == null)
                {
                    MessageBox.Show("Ошибка: пользователь не найден.");
                    return;
                }

                // Генерируем код получения
                string deliveryCode = GenerateDeliveryCode();

                // Создаем новый заказ
                var order = new Заказы
                {
                    КодСотрудника = 1,
                    КодПокупателя = _clientId,
                    ДатаОформления = DateTime.Now,
                    СуммаЗаказа = (double)totalAmount,
                    КодСтатусаЗаказа = 1,
                    КодАдреса = client.КодАдреса
                };
                context.Заказы.Add(order);
                context.SaveChanges();

                // Добавляем детали заказа
                foreach (var product in _selectedProducts)
                {
                    var orderDetail = new ДеталиЗаказа
                    {
                        КодЗаказа = order.Код,
                        КодТовара = product.Key.Id,
                        Количество = product.Value,
                        Цена = product.Key.Цена
                    };
                    context.ДеталиЗаказа.Add(orderDetail);
                }
                context.SaveChanges();

                // Генерируем QR-код и открываем его
                GenerateAndOpenQRCode(order, deliveryCode);
            }

            MessageBox.Show("Заказ успешно создан!");
            _selectedProducts.Clear();
            UpdateSelectedProductsList();
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();   
            mainWindow.Show();  
            this.Close();   
        }

        private void OnCategoryFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadProducts();
        }

        private void OnClearFiltersButtonClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedIndex = 0;
            LoadProducts();
        }


        private Товары _selectedProduct;

        private void OnProductSelected(object sender, MouseButtonEventArgs e)
        {
            // Получаем выбранный товар
            _selectedProduct = ProductsListView.SelectedItem as Товары;

            if (_selectedProduct != null)
            {
                // Отображаем панель с полным описанием товара
                
            }
        }

        private void OnCloseDetailsButtonClick(object sender, RoutedEventArgs e)
        {
            // Скрываем панель с полным описанием товара
        }

      
        public void AddProductToCart(Товары product)
        {
            if (_selectedProducts.ContainsKey(product))
            {
                _selectedProducts[product]++;
            }
            else
            {
                _selectedProducts[product] = 1;
            }
            UpdateSelectedProductsList();
        }

        private string GenerateDeliveryCode()
        {
            Random random = new Random();
            return random.Next(100, 999).ToString();
        }

        private void GenerateAndOpenQRCode(Заказы order, string deliveryCode)
        {
            // Рассчитываем дату выдачи
            DateTime deliveryDate = order.ДатаОформления.AddDays(CalculateDeliveryDays());

            // Генерируем QR-код
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"Дата заказа: {order.ДатаОформления}\n" +
                                                            $"Дата выдачи: {deliveryDate}\n" +
                                                            $"Номер заказа: {order.Код}\n" +
                                                            $"Сумма заказа: {order.СуммаЗаказа}\n" +
                                                            $"Код получения: {deliveryCode}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            // Сохраняем QR-код в файл
            string qrCodeFilePath = $"QRCode_{order.Код}.png";
            qrCodeImage.Save(qrCodeFilePath, ImageFormat.Png);

            // Открываем QR-код с помощью стандартного средства просмотра изображений
            Process.Start(qrCodeFilePath);
        }

        private int CalculateDeliveryDays()
        {
            int availableProducts = _selectedProducts.Count(p => p.Key.Количество > 3);
            return availableProducts >= 3 ? 3 : 6;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchQuery))
            {
                LoadProducts();
                return;
            }

            using (var context = new BookShopEntities1())
            {
                var products = context.Товары
                    .Where(p => p.Наименование.ToLower().Contains(searchQuery) ||
                                p.Автор.ToLower().Contains(searchQuery) ||
                                p.Описание.ToLower().Contains(searchQuery))
                    .ToList();

                ProductsListView.ItemsSource = products;
            }
        }
    }
}