using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shop.source
{
    /// <summary>
    /// Клас CategoryViewModel є посередником між інтерфейсом користувача та даними категорій.
    /// Забезпечує завантаження, додавання, редагування, видалення та пошук категорій.
    /// Реалізує механізм сповіщення про зміни для підтримки двостороннього зв'язку з UI (INotifyPropertyChanged).
    /// </summary>
    public class CategoryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Репозиторій для доступу до даних категорій.
        /// </summary>
        private readonly CategoryRepository _repository;

        /// <summary>
        /// Посилання на головне вікно програми.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Колекція категорій для відображення у UI.
        /// </summary>
        public ObservableCollection<Category> Categories { get; private set; }

        /// <summary>
        /// Поточно вибрана категорія.
        /// </summary>
        private Category _selectedCategory;

        /// <summary>
        /// Властивість для отримання або встановлення вибраної категорії.
        /// </summary>
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Конструктор. Ініціалізує ViewModel та завантажує список категорій.
        /// </summary>
        /// <param name="mainWindow">Головне вікно програми.</param>
        public CategoryViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _repository = new CategoryRepository(_mainWindow);
            LoadCategories();
        }

        /// <summary>
        /// Завантажує всі категорії з бази даних.
        /// </summary>
        public void LoadCategories()
        {
            Categories = _repository.GetAllCategories();
            OnPropertyChanged(nameof(Categories));
        }

        /// <summary>
        /// Додає нову категорію до бази даних.
        /// </summary>
        /// <param name="category">Категорія для додавання.</param>
        public void AddCategory(Category category)
        {
            _repository.AddCategory(category);
            LoadCategories();
        }

        /// <summary>
        /// Оновлює дані існуючої категорії у базі даних.
        /// </summary>
        /// <param name="category">Категорія для оновлення.</param>
        public void EditCategory(Category category)
        {
            _repository.UpdateCategory(category);
            LoadCategories();
        }

        /// <summary>
        /// Видаляє категорію з бази даних.
        /// </summary>
        /// <param name="category">Категорія для видалення.</param>
        public void DeleteCategory(Category category)
        {
            _repository.DeleteCategory(category.CategoryID);
            LoadCategories();
        }

        /// <summary>
        /// Пошук категорій за ключовим словом.
        /// </summary>
        /// <param name="keyword">Ключове слово для пошуку.</param>
        public void SearchCategories(string keyword)
        {
            Categories = _repository.SearchCategories(keyword);
            OnPropertyChanged(nameof(Categories));
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
