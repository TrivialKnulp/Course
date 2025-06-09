namespace Shop.source
{
    /// <summary>
    /// Клас StockReportItem використовується для представлення одного рядка у звіті по залишках товарів на складі.
    /// Містить інформацію про категорію, товар, кількість, одиницю виміру, ціну та розраховану загальну вартість.
    /// </summary>
    public class StockReportItem
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
        /// Кількість товару на складі.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Одиниця виміру товару.
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Загальна вартість (Quantity * Price).
        /// </summary>
        public double TotalValue => Quantity * Price;
    }

}
