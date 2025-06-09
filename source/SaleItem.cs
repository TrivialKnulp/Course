using System;
using System.ComponentModel;

namespace Shop.source
{
    /// <summary>
    /// Клас SaleItem використовується для представлення товару у поточному продажу.
    /// Містить ідентифікатор, назву, одиницю виміру, ціну, кількість, знижку та підраховує підсумкову суму з урахуванням знижки.
    /// Реалізує INotifyPropertyChanged для підтримки двостороннього зв'язку з UI.
    /// </summary>
    public class SaleItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Ідентифікатор товару.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Назва товару.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Одиниця виміру товару.
        /// </summary>
        public string Unit { get; set; }

        private double _price;

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); UpdateTotal(); }
        }

        private double _quantity;

        /// <summary>
        /// Кількість товару для продажу.
        /// </summary>
        public double Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); UpdateTotal(); }
        }

        private double _discount;

        /// <summary>
        /// Відсоток знижки на товар.
        /// </summary>
        public double Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(nameof(Discount)); UpdateTotal(); }
        }

        private double _total;

        /// <summary>
        /// Підсумкова сума з урахуванням знижки (Quantity * Price * (1 - Discount / 100)).
        /// </summary>
        public double Total
        {
            get => _total;
            private set { _total = value; OnPropertyChanged(nameof(Total)); }
        }

        /// <summary>
        /// Оновлює підсумкову суму при зміні ціни, кількості або знижки.
        /// </summary>
        private void UpdateTotal()
        {
            Total = Math.Round(Quantity * Price * (1 - Discount / 100.0), 2);
        }

        /// <summary>
        /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Викликає подію PropertyChanged для вказаної властивості.
        /// </summary>
        /// <param name="prop">Назва властивості.</param>
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
