using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Shop.source
{
    /// <summary>
    /// Клас ProductRepository відповідає за доступ до даних товарів у базі даних.
    /// Забезпечує отримання, додавання, оновлення, видалення та пошук товарів.
    /// </summary>
    public class ProductRepository
    {
        private readonly string _connectionString;
        private MainWindow _mainWindow;

        /// <summary>
        /// Конструктор. Ініціалізує репозиторій товарів з посиланням на головне вікно.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public ProductRepository(MainWindow main)
        {
            _mainWindow = main;
            _connectionString = _mainWindow.ConnectionString;
        }

        /// <summary>
        /// Отримує список усіх товарів з бази даних (у вигляді List).
        /// </summary>
        /// <returns>Список об'єктів Product.</returns>
        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT p.ProductID, p.CategoryID, c.CategoryName, p.Name, p.Unit, p.Price, p.Quantity, p.LastDeliveryDate
                                 FROM Products p
                                 JOIN Categories c ON p.CategoryID = c.CategoryID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                CategoryID = reader.GetInt32(1),
                                CategoryName = reader.GetString(2),
                                Name = reader.GetString(3),
                                Unit = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                Quantity = reader.GetDouble(6),
                                LastDeliveryDate = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return products;
        }

        /// <summary>
        /// Отримує список усіх товарів з бази даних (у вигляді ObservableCollection).
        /// </summary>
        /// <returns>Колекція об'єктів Product.</returns>
        public ObservableCollection<Product> GetAllProducts()
        {
            var products = new ObservableCollection<Product>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT p.ProductID, p.CategoryID, c.CategoryName, p.Name, p.Unit, p.Price, p.Quantity, p.LastDeliveryDate
                                 FROM Products p
                                 JOIN Categories c ON p.CategoryID = c.CategoryID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                CategoryID = reader.GetInt32(1),
                                CategoryName = reader.GetString(2),
                                Name = reader.GetString(3),
                                Unit = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                Quantity = reader.GetDouble(6),
                                LastDeliveryDate = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return products;
        }

        /// <summary>
        /// Додає новий товар до бази даних.
        /// </summary>
        /// <param name="product">Товар для додавання.</param>
        public void AddProduct(Product product)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string query = @"INSERT INTO Products (CategoryID, Name, Unit, Price, Quantity, LastDeliveryDate) 
                                 VALUES (@catId, @name, @unit, @price, @qty, @date)";
                try
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@unit", product.Unit);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
                        cmd.Parameters.AddWithValue("@date", product.LastDeliveryDate);
                        cmd.ExecuteNonQuery();
                    }

                    _mainWindow.AddLogMessage("Товар додано до бази даних.", "Інформація");
                }
                catch
                {
                    _mainWindow.AddLogMessage("Помилка додавання товару до бази даних.", "Error");
                }
            }
        }

        /// <summary>
        /// Оновлює дані існуючого товару у базі даних.
        /// </summary>
        /// <param name="product">Товар для оновлення.</param>
        public void UpdateProduct(Product product)
        {
            string query = @"UPDATE Products 
                                 SET CategoryID = @catId, Name = @name, Unit = @unit, Price = @price, 
                                     Quantity = @qty, LastDeliveryDate = @date 
                                 WHERE ProductID = @id";
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", product.ProductID);
                        cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@unit", product.Unit);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@qty", product.Quantity);
                        cmd.Parameters.AddWithValue("@date", product.LastDeliveryDate);
                        cmd.ExecuteNonQuery();
                    }
                }
                _mainWindow.AddLogMessage("Продукт оновлено в базі даних.", "Інформація");
            }
            catch
            {
                _mainWindow.AddLogMessage("Помилка оновлення товару в базі даних.", "Error");
            }
        }

        /// <summary>
        /// Видаляє товар з бази даних за ідентифікатором.
        /// </summary>
        /// <param name="productId">Ідентифікатор товару для видалення.</param>
        public void DeleteProduct(int productId)
        {
            string query = "DELETE FROM Products WHERE ProductID = @id";
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        cmd.ExecuteNonQuery();
                    }
                }
                _mainWindow.AddLogMessage("Товар видалено з бази даних.", "Інформація");
            }
            catch
            {
                _mainWindow.AddLogMessage("Помилка видалення товару з бази даних.", "Error");
            }
        }

        /// <summary>
        /// Пошук товарів за ключовим словом у назві.
        /// </summary>
        /// <param name="keyword">Ключове слово для пошуку.</param>
        /// <returns>Колекція знайдених товарів.</returns>
        public ObservableCollection<Product> SearchProducts(string keyword)
        {
            var products = new ObservableCollection<Product>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT p.ProductID, p.CategoryID, c.CategoryName, p.Name, p.Unit, p.Price, p.Quantity, p.LastDeliveryDate
                                 FROM Products p
                                 JOIN Categories c ON p.CategoryID = c.CategoryID
                                 WHERE p.Name LIKE @keyword";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductID = reader.GetInt32(0),
                                CategoryID = reader.GetInt32(1),
                                CategoryName = reader.GetString(2),
                                Name = reader.GetString(3),
                                Unit = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                Quantity = reader.GetInt32(6),
                                LastDeliveryDate = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return products;
        }
    }
}
