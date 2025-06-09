namespace Shop.source
{
    /// <summary>
    /// Клас Category використовується для представлення категорії товарів у системі.
    /// Містить унікальний ідентифікатор та назву категорії.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Унікальний ідентифікатор категорії.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Назва категорії.
        /// </summary>
        public string CategoryName { get; set; }
    }
}
