using System;

namespace Shop.source
{
    /// <summary>
    /// Клас DepreciationReportItem використовується для представлення одного рядка у звіті по амортизації (списанню) товарів.
    /// Містить інформацію про категорію, товар, кількість, ціну, дату та розраховану суму втрат.
    /// </summary>
    public class DepreciationReportItem
    {
        /// <summary>
        /// Назва категорії товару.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Назва товару.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Кількість списаного товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Дата амортизації (списання).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Загальна сума втрат (кількість * ціна).
        /// </summary>
        public double TotalLoss => Quantity * Price;
    }

}
