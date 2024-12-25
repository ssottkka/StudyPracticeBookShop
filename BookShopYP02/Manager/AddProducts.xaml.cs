using Microsoft.Win32;
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
    /// Логика взаимодействия для AddProducts.xaml
    /// </summary>
    public partial class AddProducts : Window
    {
        private string photoPath = string.Empty;

        public AddProducts()
        {
            InitializeComponent();
            LoadGenres();
            LoadManufacturers();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow manager = new ManagerWindow();
            manager.Show();
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, выбран ли жанр
            if (GenreComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите жанр.");
                return;
            }

            // Проверка, выбран ли производитель
            if (ManufacturerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите производителя.");
                return;
            }

            // Логика сохранения данных в базу
            string author = AuthorTextBox.Text;
            string name = NameTextBox.Text;
            string priceText = PriceTextBox.Text;
            int genreId = (int)GenreComboBox.SelectedValue;
            int manufacturerId = (int)ManufacturerComboBox.SelectedValue;
            string description = DescriptionTextBox.Text;
            string discountText = DiscountTextBox.Text;

            if (string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceText))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            if (!double.TryParse(priceText, out double price))
            {
                MessageBox.Show("Пожалуйста, введите корректную стоимость.");
                return;
            }

            if (!double.TryParse(discountText, out double discount))
            {
                discount = 0; // Если скидка не указана, устанавливаем её в 0
            }

            // Сохранение данных в базу
            SaveProductToDatabase(author, name, price, genreId, manufacturerId, description, discount, photoPath);
        }

        private void SaveProductToDatabase(string author, string name, double price, int genreId, int manufacturerId, string description, double discount, string photoPath)
        {
            using (var context = new BookShopEntities1())
            {
                var product = new Товары
                {
                    Автор = author,
                    Наименование = name,
                    Цена = (float)price,
                    КодКатегории = genreId,
                    КодПроизводителя = manufacturerId,
                    Описание = description,
                    РазмерСкидки = (float)discount,
                    Картинка = photoPath,
                    Количество = 1 // По умолчанию количество 1
                };

                context.Товары.Add(product);
                context.SaveChanges();
            }

            MessageBox.Show("Товар успешно добавлен!");
            ClearFields();
        }

        private void ClearFields()
        {
            AuthorTextBox.Clear();
            NameTextBox.Clear();
            PriceTextBox.Clear();
            GenreComboBox.SelectedIndex = -1;
            ManufacturerComboBox.SelectedIndex = -1;
            DescriptionTextBox.Clear();
            DiscountTextBox.Clear();
            photoPath = string.Empty;
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика выбора фото
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                photoPath = openFileDialog.FileName;
                MessageBox.Show($"Фото выбрано: {photoPath}");
            }
        }

        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Валидация ввода для поля стоимости
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        private void DiscountTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Валидация ввода для поля скидки
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void LoadGenres()
        {
            using (var context = new BookShopEntities1())
            {
                var genres = context.КатегорииТоваров.ToList();
                if (genres.Count == 0)
                {
                    MessageBox.Show("Жанры не загружены из базы данных.");
                }
                else
                {
                    MessageBox.Show($"Загружено жанров: {genres.Count}");
                }
                GenreComboBox.ItemsSource = genres;
            }
        }

        private void LoadManufacturers()
        {
            using (var context = new BookShopEntities1())
            {
                var manufacturers = context.Производители.ToList();
                if (manufacturers.Count == 0)
                {
                    MessageBox.Show("Производители не загружены из базы данных.");
                }
                else
                {
                    MessageBox.Show($"Загружено производителей: {manufacturers.Count}");
                }
                ManufacturerComboBox.ItemsSource = manufacturers;
            }
        }
    }
}
