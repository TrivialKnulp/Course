namespace Shop.source
{
    /// <summary>
    /// Клас Delivery використовується для представлення інформації про поставку товару у системі.
    /// Містить ідентифікатор поставки, ідентифікатор товару, кількість та дату поставки.
    /// </summary>
    public class Delivery
    {
        /// <summary>
        /// Унікальний ідентифікатор поставки.
        /// </summary>
        public int DeliveryID { get; set; }

        /// <summary>
        /// Ідентифікатор товару, який був поставлений.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Кількість поставленого товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Дата поставки.
        /// </summary>
        public string DeliveryDate { get; set; }
    }
}
