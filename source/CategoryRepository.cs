using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Shop.source
{
    /// <summary>
    /// Клас CategoryRepository відповідає за доступ до даних категорій у базі даних.
    /// Забезпечує отримання, додавання, оновлення, видалення та пошук категорій.
    /// Всі операції виконуються через SQLite та супроводжуються логуванням у головному вікні.
    /// </summary>
    public class CategoryRepository
    {
        /// <summary>
        /// Рядок підключення до бази даних SQLite.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Посилання на головне вікно для логування подій.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Конструктор. Ініціалізує репозиторій категорій з посиланням на головне вікно.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public CategoryRepository(MainWindow main)
        {
            _mainWindow = main;
            _connectionString = _mainWindow.ConnectionString;
        }

        /// <summary>
        /// Отримує всі категорії з бази даних.
        /// </summary>
        /// <returns>Колекція категорій.</returns>
        public ObservableCollection<Category> GetAllCategories()
        {
            var categories = new ObservableCollection<Category>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = reader.GetInt32(0),
                                CategoryName = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Додає нову категорію до бази даних.
        /// </summary>
        /// <param name="category">Категорія для додавання.</param>
        public void AddCategory(Category category)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Categories (CategoryName) VALUES (@name)";
                try
                {
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", category.CategoryName);
                        cmd.ExecuteNonQuery();
                    }
                    _mainWindow.AddLogMessage("Категорію додано успішно.", "Інформація");
                }
                catch (SQLiteException ex)
                {
                    _mainWindow.AddLogMessage("Помилка додавання категорії: " + ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Оновлює дані існуючої категорії у базі даних.
        /// </summary>
        /// <param name="category">Категорія для оновлення.</param>
        public void UpdateCategory(Category category)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string query = "UPDATE Categories SET CategoryName = @name WHERE CategoryID = @id";
                try
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", category.CategoryName);
                        cmd.Parameters.AddWithValue("@id", category.CategoryID);
                        cmd.ExecuteNonQuery();
                    }
                    _mainWindow.AddLogMessage("Категорію успішно оновлено.", "Інформація");
                }
                catch (SQLiteException ex)
                {
                    _mainWindow.AddLogMessage("Категорія помилки оновлення: " + ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Видаляє категорію з бази даних за ідентифікатором.
        /// Перед видаленням перевіряє, чи не використовується категорія у товарах.
        /// </summary>
        /// <param name="categoryId">Ідентифікатор категорії для видалення.</param>
        public void DeleteCategory(int categoryId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Categories WHERE CategoryID = @id";
                try
                {
                    using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    // Перевірка, чи використовується категорія у товарах
                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE CategoryID = @id";
                    using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@id", categoryId);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            _mainWindow.AddLogMessage("Неможливо видалити категорію. Вона використовується в товарах.", "Error");
                            return;
                        }
                    }

                    // Якщо не використовується, видалити категорію
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        cmd.ExecuteNonQuery();
                    }
                    _mainWindow.AddLogMessage("Категорію успішно видалено.", "Інформація");
                }
                catch (SQLiteException ex)
                {
                    _mainWindow.AddLogMessage("Помилка під час видалення категорії: " + ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Пошук категорій за ключовим словом у назві.
        /// </summary>
        /// <param name="keyword">Ключове слово для пошуку.</param>
        /// <returns>Колекція знайдених категорій.</returns>
        public ObservableCollection<Category> SearchCategories(string keyword)
        {
            var categories = new ObservableCollection<Category>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT CategoryID, CategoryName FROM Categories WHERE CategoryName LIKE @keyword";
                try
                {
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category
                                {
                                    CategoryID = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    _mainWindow.AddLogMessage("Помилка під час пошуку категорій: " + ex.Message, "Error");
                }
            }

            return categories;
        }
    }
}
