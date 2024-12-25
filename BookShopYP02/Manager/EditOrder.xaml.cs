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
    public partial class EditOrder : Window
    {
        public ObservableCollection<Заказы> Orders { get; set; }

        public EditOrder()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var context = new BookShopEntities1())
            {
                // Загрузка всех заказов без подключения связанных данных
                Orders = new ObservableCollection<Заказы>(context.Заказы.ToList());

                // Привязка данных к DataGrid
                OrdersDataGrid.ItemsSource = Orders;
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BookShopEntities1())
            {
                foreach (var order in Orders)
                {
                    var orderToUpdate = context.Заказы.Find(order.Код);

                    if (orderToUpdate != null)
                    {
                        // Обновляем данные заказа
                        orderToUpdate.ДатаОформления = order.ДатаОформления;
                        orderToUpdate.ДатаДоставки = order.ДатаДоставки;
                        orderToUpdate.СуммаЗаказа = order.СуммаЗаказа;

                        // Обновляем статус заказа
                        if (order.КодСтатусаЗаказа != null)
                        {
                            var status = context.СтатусыЗаказов
                                .FirstOrDefault(s => s.КодСтатуса == order.КодСтатусаЗаказа);

                            if (status != null)
                            {
                                orderToUpdate.КодСтатусаЗаказа = status.КодСтатуса;
                            }
                        }

                        // Обновляем покупателя
                        if (order.КодПокупателя != null)
                        {
                            var customer = context.Покупатели
                                .FirstOrDefault(c => c.Код == order.КодПокупателя);

                            if (customer != null)
                            {
                                orderToUpdate.КодПокупателя = customer.Код;
                            }
                        }

                        // Обновляем сотрудника
                        if (order.КодСотрудника != null)
                        {
                            var employee = context.Сотрудники
                                .FirstOrDefault(emp => emp.КодСотрудника == order.КодСотрудника);

                            if (employee != null)
                            {
                                orderToUpdate.КодСотрудника = employee.КодСотрудника;
                            }
                        }
                    }
                }

                // Сохраняем изменения в базе данных
                context.SaveChanges();

                // Выводим сообщение об успешном сохранении
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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