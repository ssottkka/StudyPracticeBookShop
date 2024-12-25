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
    public partial class ConfirmOrder : Window
    {
        private ObservableCollection<Заказы> Orders;

        public ConfirmOrder()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var context = new BookShopEntities1())
            {
                // Загрузка заказов с КодСтатусаЗаказа = 1
                Orders = new ObservableCollection<Заказы>(context.Заказы
                    .Where(z => z.КодСтатусаЗаказа == 1)
                    .ToList());

                // Привязка данных к DataGrid
                OrdersDataGrid.ItemsSource = Orders;
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Включаем кнопку "Подтвердить заказ", если выбран заказ
            ConfirmOrderButton.IsEnabled = OrdersDataGrid.SelectedItem != null;
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Заказы selectedOrder)
            {
                using (var context = new BookShopEntities1())
                {
                    // Находим выбранный заказ в базе данных
                    var orderToUpdate = context.Заказы.Find(selectedOrder.Код);

                    if (orderToUpdate != null)
                    {
                        // Изменяем статус заказа на 2
                        orderToUpdate.КодСтатусаЗаказа = 2;
                        context.SaveChanges();

                        // Удаляем заказ из списка отображаемых заказов
                        Orders.Remove(selectedOrder);

                        // Выводим сообщение об успешном подтверждении
                        MessageBox.Show("Заказ успешно подтвержден!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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