using System;

namespace Shop.source
{
    /// <summary>
    /// Клас SaleReportItem використовується для представлення одного рядка у звіті по продажах товарів.
    /// Містить інформацію про дату продажу, товар, кількість, одиницю виміру, ціну за одиницю та розраховану загальну суму продажу.
    /// </summary>
    public class SaleReportItem
    {
        /// <summary>
        /// Дата продажу.
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Назва товару.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Кількість проданого товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Одиниця виміру товару.
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Загальна сума продажу (Quantity * UnitPrice).
        /// </summary>
        public double TotalPrice => Quantity * UnitPrice;
    }

}
