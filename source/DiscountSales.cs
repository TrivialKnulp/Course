namespace Shop.source
{
    /// <summary>
    /// Клас DiscountSales використовується для представлення інформації про продаж товару зі знижкою у системі.
    /// Містить ідентифікатор товару, кількість, початкову ціну, відсоток знижки та дату продажу.
    /// </summary>
    public class DiscountSales
    {
        /// <summary>
        /// Ідентифікатор товару, який було продано зі знижкою.
        /// </summary>
        public int ProductID { get; set; }

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
        public string Date { get; set; }
    }
}
