namespace Shop.source
{
    /// <summary>
    /// Клас Product використовується для представлення товару у системі.
    /// Містить ідентифікатор товару, ідентифікатор та назву категорії, назву товару, одиницю виміру, ціну, кількість та дату останньої поставки.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Унікальний ідентифікатор товару.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Ідентифікатор категорії, до якої належить товар.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Назва категорії, до якої належить товар.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Назва товару.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Одиниця виміру товару.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Кількість товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Дата останньої поставки товару.
        /// </summary>
        public string LastDeliveryDate { get; set; }
    }
}
