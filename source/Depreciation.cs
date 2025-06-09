namespace Shop.source
{
    /// <summary>
    /// Клас Depreciation використовується для представлення інформації про амортизацію (списання) товару у системі.
    /// Містить ідентифікатор амортизації, ідентифікатор та назву товару, кількість, причину та дату амортизації.
    /// </summary>
    public class Depreciation
    {
        /// <summary>
        /// Унікальний ідентифікатор амортизації.
        /// </summary>
        public int DepreciationID { get; set; }

        /// <summary>
        /// Ідентифікатор товару, який списується.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Назва товару, який списується.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Кількість списаного товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Причина амортизації (списання).
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Дата амортизації.
        /// </summary>
        public string Date { get; set; }
    }

}
