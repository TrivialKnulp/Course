using System.ComponentModel;

namespace Shop.source
{
    /// <summary>
    /// Клас InventoryItem використовується для представлення одного товару в інвентаризації.
    /// Містить назву товару, одиницю виміру, ціну, кількість за інвентаризацією та підсумкову вартість.
    /// Реалізує INotifyPropertyChanged для підтримки двостороннього зв'язку з UI.
    /// </summary>
    public class InventoryItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Назва товару.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Одиниця виміру товару.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price { get; set; }

        private double countedQuantity;

        /// <summary>
        /// Кількість товару, порахована під час інвентаризації.
        /// </summary>
        public double CountedQuantity
        {
            get => countedQuantity;
            set
            {
                countedQuantity = value;
                OnPropertyChanged(nameof(CountedQuantity));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        /// <summary>
        /// Підсумкова вартість (CountedQuantity * Price).
        /// </summary>
        public double Subtotal => CountedQuantity * Price;

        /// <summary>
        /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Викликає подію PropertyChanged для вказаної властивості.
        /// </summary>
        /// <param name="name">Назва властивості.</param>
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
