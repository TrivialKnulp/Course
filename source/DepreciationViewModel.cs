using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shop.source
{
    /// <summary>
    /// Клас DepreciationViewModel є посередником між інтерфейсом користувача та даними амортизації (списання) товарів.
    /// Забезпечує завантаження списку амортизацій з бази даних та сповіщення UI про зміни.
    /// Реалізує механізм INotifyPropertyChanged для підтримки двостороннього зв'язку з інтерфейсом.
    /// </summary>
    public class DepreciationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Репозиторій для доступу до даних амортизації.
        /// </summary>
        private readonly DepreciationRepository _repository;

        /// <summary>
        /// Посилання на головне вікно програми.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Колекція амортизацій для відображення у UI.
        /// </summary>
        public ObservableCollection<Depreciation> Depreciations { get; private set; }

        /// <summary>
        /// Конструктор. Ініціалізує ViewModel та завантажує список амортизацій.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public DepreciationViewModel(MainWindow main)
        {
            _mainWindow = main;
            _repository = new DepreciationRepository(_mainWindow);
            LoadDepreciations();
        }

        /// <summary>
        /// Завантажує всі записи амортизації з бази даних.
        /// </summary>
        public void LoadDepreciations()
        {
            Depreciations = _repository.GetAllDepreciations();
            OnPropertyChanged(nameof(Depreciations));
        }

        /// <summary>
        /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Викликає подію PropertyChanged для вказаної властивості.
        /// </summary>
        /// <param name="name">Назва властивості.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
