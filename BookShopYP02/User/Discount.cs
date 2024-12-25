using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopYP02.User
{
    public class Discount
    {
        private readonly BookShopEntities1 _context;
        public decimal CalculateTotalOrderAmount(Dictionary<Товары, int> selectedProducts)
        {
            decimal totalAmount = 0;

            foreach (var item in selectedProducts)
            {
                var product = item.Key;
                var quantity = item.Value;
                var price = product.Цена; // Используем decimal
                var discount = product.РазмерСкидки; // Используем decimal

                // Рассчитываем сумму с учетом скидки
                var discountedPrice = price * (1 - (discount / 100));

                // Приводим результат к decimal перед добавлением к totalAmount
                totalAmount += (decimal)(discountedPrice * quantity);
            }

            return totalAmount;
        }
    }
}
