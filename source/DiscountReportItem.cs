using System;

namespace Shop.source
{
    /// <summary>
    /// Клас DiscountReportItem використовується для представлення одного рядка у звіті по продажах зі знижкою.
    /// Містить інформацію про категорію, товар, кількість, початкову ціну, відсоток знижки, дату продажу та розраховану суму знижки.
    /// </summary>
    public class DiscountReportItem
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
        /// Кількість проданого товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Початкова ціна одиниці товару.
        /// </summary>
        public double OriginalPrice { get; set; }

        /// <summary>
        /// Відсоток знижки, застосований до товару.
        /// </summary>
        public double DiscountPercent { get; set; }

        /// <summary>
        /// Дата продажу.
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Сума знижки (кількість * початкова ціна * відсоток знижки / 100).
        /// </summary>
        public double DiscountAmount => Quantity * OriginalPrice * DiscountPercent / 100.0;
    }

}
