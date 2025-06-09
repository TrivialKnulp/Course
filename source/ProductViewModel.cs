using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shop.source
{
    /// <summary>
    /// Клас ProductViewModel є посередником між інтерфейсом користувача та даними товарів.
    /// Забезпечує завантаження, додавання, редагування, видалення та пошук товарів.
    /// Реалізує механізм сповіщення про зміни для підтримки двостороннього зв'язку з UI (INotifyPropertyChanged).
    /// </summary>
    public class ProductViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Репозиторій для доступу до даних товарів.
        /// </summary>
        private readonly ProductRepository _repository;

        /// <summary>
        /// Посилання на головне вікно програми.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Колекція товарів для відображення у UI.
        /// </summary>
        public ObservableCollection<Product> Products { get; private set; }

        private Product _selectedProduct;

        /// <summary>
        /// Отримує список усіх товарів (у вигляді List).
        /// </summary>
        /// <returns>Список об'єктів Product.</returns>
        public List<Product> GetProducts()
        {
            return _repository.GetProducts();
        }

        /// <summary>
        /// Поточно вибраний товар.
        /// </summary>
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Конструктор. Ініціалізує ViewModel та завантажує список товарів.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public ProductViewModel(MainWindow main)
        {
            _mainWindow = main;
            _repository = new ProductRepository(_mainWindow);
            LoadProducts();
        }

        /// <summary>
        /// Завантажує всі товари з бази даних.
        /// </summary>
        public void LoadProducts()
        {
            Products = _repository.GetAllProducts();
            OnPropertyChanged(nameof(Products));
        }

        /// <summary>
        /// Додає новий товар до бази даних.
        /// </summary>
        /// <param name="product">Товар для додавання.</param>
        public void AddProduct(Product product)
        {
            _repository.AddProduct(product);
            LoadProducts();
        }

        /// <summary>
        /// Оновлює дані існуючого товару у базі даних.
        /// </summary>
        /// <param name="product">Товар для оновлення.</param>
        public void EditProduct(Product product)
        {
            _repository.UpdateProduct(product);
            LoadProducts();
        }

        /// <summary>
        /// Видаляє товар з бази даних.
        /// </summary>
        /// <param name="product">Товар для видалення.</param>
        public void DeleteProduct(Product product)
        {
            _repository.DeleteProduct(product.ProductID);
            LoadProducts();
        }

        /// <summary>
        /// Пошук товарів за ключовим словом.
        /// </summary>
        /// <param name="keyword">Ключове слово для пошуку.</param>
        public void SearchProducts(string keyword)
        {
            Products = _repository.SearchProducts(keyword);
            OnPropertyChanged(nameof(Products));
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
