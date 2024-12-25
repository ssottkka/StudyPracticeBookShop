using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private Discount _discountService;

        [TestInitialize]
        public void Setup()
        {
            _discountService = new Discount();
        }

        [TestMethod]
        public void CalculateTotalOrderAmount_NormalConditions_ReturnsCorrectAmount()
        {
            // Arrange
            var selectedProducts = new Dictionary<Товары, int>
            {
                { new Товары { Цена = 100, РазмерСкидки = 10 }, 2 }
            };

            // Act
            decimal totalAmount = _discountService.CalculateTotalOrderAmount(selectedProducts);

            // Assert
            Assert.AreEqual(180, totalAmount); // 100 * 2 * 0.9 = 180
        }

        [TestMethod]
        public void CalculateTotalOrderAmount_ExtremeConditions_ReturnsCorrectAmount()
        {
            // Arrange
            var selectedProducts = new Dictionary<Товары, int>
            {
                { new Товары { Цена = 100, РазмерСкидки = 10 }, 100 },
                { new Товары { Цена = 200, РазмерСкидки = 20 }, 50 },
                { new Товары { Цена = 50, РазмерСкидки = 5 }, 200 }
            };

            // Act
            decimal totalAmount = _discountService.CalculateTotalOrderAmount(selectedProducts);

            // Assert
            // 100 * 100 * 0.9 + 200 * 50 * 0.8 + 50 * 200 * 0.95 = 9000 + 8000 + 9500 = 26500
            Assert.AreEqual(26500, totalAmount);
        }

        [TestMethod]
        public void CalculateTotalOrderAmount_ExceptionalConditions_ReturnsCorrectAmount()
        {
            // Arrange
            var selectedProducts = new Dictionary<Товары, int>
            {
                { new Товары { Цена = -100, РазмерСкидки = 10 }, 2 }, // Отрицательная цена
                { new Товары { Цена = 100, РазмерСкидки = -20 }, 2 }  // Отрицательная скидка
            };

            // Act
            decimal totalAmount = _discountService.CalculateTotalOrderAmount(selectedProducts);

            // Assert
            // -100 * 2 * 0.9 + 100 * 2 * 1.2 = -180 + 240 = 60
            Assert.AreEqual(60, totalAmount);
        }
    }

    // Временный класс для тестирования
    public class Товары
    {
        public decimal Цена { get; set; }
        public decimal РазмерСкидки { get; set; }
    }

    // Класс Discount для тестирования
    public class Discount
    {
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