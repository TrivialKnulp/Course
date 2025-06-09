using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.source
{
    /// <summary>
    /// Статичний клас SQLiteHelper надає допоміжні методи для створення бази даних SQLite та тестових даних.
    /// Дозволяє створити структуру таблиць та заповнити їх початковими тестовими записами.
    /// </summary>
    public static class SQLiteHelper
    {
        /// <summary>
        /// Створює файл бази даних та всі необхідні таблиці, якщо файл ще не існує.
        /// </summary>
        /// <param name="DatabaseFile">Шлях до файлу бази даних.</param>
        /// <returns>true, якщо база даних була створена; false, якщо файл вже існує.</returns>
        public static bool CreateDatabase(string DatabaseFile)
        {
            // Check if the database file exists
            if (!System.IO.File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
                using (var connection = new SQLiteConnection($"Data Source={DatabaseFile};Version=3;"))
                {
                    connection.Open();

                    string createTables = @"
                            CREATE TABLE IF NOT EXISTS Categories (
                                CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
                                CategoryName TEXT NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS Products (
                                ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                                CategoryID INTEGER NOT NULL,
                                Name TEXT NOT NULL,
                                Unit TEXT NOT NULL,        
                                Price REAL DEFAULT 0 NOT NULL,       
                                Quantity REAL DEFAULT 0 NOT NULL, 
                                LastDeliveryDate TEXT,
                                FOREIGN KEY(CategoryID) REFERENCES Categories(CategoryID) ON DELETE CASCADE
                            );

                            CREATE TABLE IF NOT EXISTS Invoices (
                                InvoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                                SaleDate TEXT NOT NULL,    
                                TotalPrice REAL DEFAULT 0 NOT NULL  
                            );

                            CREATE TABLE IF NOT EXISTS InvoiceDetails (
                                InvoiceID INTEGER NOT NULL,
                                SequenceNumber INTEGER NOT NULL,
                                ProductID INTEGER NOT NULL,
                                ProductName TEXT NOT NULL,
                                Quantity REAL NOT NULL,
                                Unit TEXT NOT NULL, 
                                Discount REAL NOT NULL,
                                Price REAL NOT NULL,
                                FOREIGN KEY(InvoiceID) REFERENCES Invoices(InvoiceID) ON DELETE CASCADE,
                                FOREIGN KEY(ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
                            );

                            CREATE TABLE IF NOT EXISTS Deliveries (
                                DeliveryID INTEGER PRIMARY KEY AUTOINCREMENT,
                                ProductID INTEGER NOT NULL,
                                Quantity REAL NOT NULL,
                                DeliveryDate TEXT NOT NULL,
                                FOREIGN KEY(ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
                            );

                            CREATE TABLE IF NOT EXISTS Depreciations (
                                DepreciationID INTEGER PRIMARY KEY AUTOINCREMENT,
                                ProductID INTEGER NOT NULL,
                                ProductName TEXT NOT NULL,
                                Quantity REAL NOT NULL,
                                Reason TEXT,
                                Date TEXT NOT NULL,
                                FOREIGN KEY(ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
                            );

                            CREATE TABLE IF NOT EXISTS DiscountSales (
                             ProductID INTEGER NOT NULL,
                             Quantity REAL NOT NULL,
                             OriginalPrice REAL NOT NULL,
                             DiscountPercent REAL NOT NULL,
                             Date Text NOT NULL
                            );

                        ";
                    using (var command = new SQLiteCommand(createTables, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Створює тестові дані у базі даних для демонстрації та тестування функціоналу.
        /// </summary>
        /// <param name="DatabaseFile">Шлях до файлу бази даних.</param>
        /// <returns>true, якщо тестові дані були успішно додані; false у разі помилки.</returns>
        public static bool CreateTestData(string DatabaseFile)
        {
            SQLiteConnection connection = null;
            try
            {
                connection = new SQLiteConnection($"Data Source={DatabaseFile};Version=3;");
                connection.Open();
                string insertCategories = @"
    INSERT INTO Categories (CategoryName) VALUES 
    ('Electronics'),
    ('Furniture'),
    ('Clothing'),
    ('Food'),
    ('Office Supplies');

    INSERT INTO Products (CategoryID, Name, Unit, Price, Quantity, LastDeliveryDate) VALUES 
    (1, 'Laptop', 'Piece', 1200.50, 50, '2025-04-01'),
    (1, 'Smartphone', 'Piece', 800.00, 100, '2025-04-03'),
    (1, 'Headphones', 'Piece', 150.00, 200, '2025-04-05'),
    (1, 'Tablet', 'Piece', 400.00, 150, '2025-04-07'),
    (1, 'Smartwatch', 'Piece', 250.00, 80, '2025-04-10'),
    (2, 'Sofa', 'Piece', 600.00, 30, '2025-04-02'),
    (2, 'Chair', 'Piece', 120.00, 120, '2025-04-06'),
    (2, 'Table', 'Piece', 250.00, 60, '2025-04-08'),
    (2, 'Shelf', 'Piece', 150.00, 75, '2025-04-09'),
    (2, 'Bed', 'Piece', 500.00, 40, '2025-04-11'),
    (3, 'T-Shirt', 'Piece', 20.00, 500, '2025-04-03'),
    (3, 'Jeans', 'Piece', 40.00, 200, '2025-04-07'),
    (3, 'Jacket', 'Piece', 80.00, 100, '2025-04-09'),
    (3, 'Sneakers', 'Pair', 50.00, 150, '2025-04-10'),
    (3, 'Pants', 'Piece', 30.00, 300, '2025-04-12'),
    (4, 'Apple', 'kg', 3.00, 500, '2025-04-02'),
    (4, 'Bread', 'Piece', 2.50, 400, '2025-04-04'),
    (4, 'Milk', 'L', 1.20, 600, '2025-04-06'),
    (4, 'Eggs', 'Pack', 2.00, 300, '2025-04-08'),
    (4, 'Cheese', 'kg', 5.00, 200, '2025-04-10'),
    (5, 'Pen', 'Piece', 1.50, 1000, '2025-04-01'),
    (5, 'Paper', 'Pack', 10.00, 1500, '2025-04-03'),
    (5, 'Binder', 'Piece', 5.00, 500, '2025-04-05'),
    (5, 'Notebook', 'Piece', 2.00, 800, '2025-04-07'),
    (5, 'Notepad', 'Piece', 3.00, 700, '2025-04-09'),
    (1, 'PC Monitor', 'Piece', 250.00, 100, '2025-04-11'),
    (2, 'Armchair', 'Piece', 150.00, 90, '2025-04-12'),
    (3, 'Sweater', 'Piece', 25.00, 350, '2025-04-14'),
    (4, 'Banana', 'kg', 2.50, 400, '2025-04-06'),
    (5, 'Calculator', 'Piece', 10.00, 600, '2025-04-08');

    INSERT INTO Deliveries (ProductID, Quantity, DeliveryDate) VALUES 
    (1, 20, '2025-04-10'),
    (2, 50, '2025-04-12'),
    (3, 30, '2025-04-15'),
    (4, 40, '2025-04-16'),
    (5, 10, '2025-04-18');

    INSERT INTO Invoices (SaleDate, TotalPrice) VALUES
    ('2025-04-01', 2400.50),
    ('2025-04-03', 1600.00),
    ('2025-04-05', 450.00),
    ('2025-04-07', 850.00),
    ('2025-04-09', 500.00),
    ('2025-04-10', 1500.00),
    ('2025-04-12', 750.00),
    ('2025-04-14', 1200.00),
    ('2025-04-15', 700.00),
    ('2025-04-18', 950.00);

    INSERT INTO InvoiceDetails (InvoiceID, SequenceNumber, ProductID, ProductName, Quantity, Unit, Discount, Price) VALUES
    (1, 1, 1, 'Laptop', 2, 'Piece', 0, 2400.50),
    (1, 2, 2, 'Smartphone', 1, 'Piece', 0, 800.00),
    (2, 1, 3, 'Headphones', 3, 'Piece', 0, 450.00),
    (2, 2, 4, 'Tablet', 2, 'Piece', 0, 800.00),
    (3, 1, 5, 'Smartwatch', 1, 'Piece', 0, 250.00),
    (3, 2, 6, 'Sofa', 2, 'Piece', 0, 1200.00),
    (4, 1, 7, 'Chair', 5, 'Piece', 0, 600.00),
    (4, 2, 8, 'Table', 3, 'Piece', 0, 750.00),
    (5, 1, 9, 'Shelf', 2, 'Piece', 0, 300.00),
    (5, 2, 10, 'Bed', 1, 'Piece', 0, 500.00),
    (6, 1, 11, 'T-Shirt', 3, 'Piece', 0, 60.00),
    (6, 2, 12, 'Jeans', 2, 'Piece', 0, 80.00),
    (7, 1, 13, 'Jacket', 1, 'Piece', 0, 80.00),
    (7, 2, 14, 'Sneakers', 2, 'Pair', 0, 100.00),
    (8, 1, 15, 'Pants', 4, 'Piece', 0, 120.00),
    (8, 2, 16, 'Apple', 10, 'kg', 0, 30.00),
    (9, 1, 17, 'Bread', 2, 'Piece', 0, 5.00),
    (9, 2, 18, 'Milk', 5, 'L', 0, 6.00),
    (10, 1, 19, 'Eggs', 1, 'Pack', 0, 2.00),
    (10, 2, 20, 'Cheese', 3, 'kg', 0, 15.00);

    INSERT INTO Depreciations (ProductID, ProductName, Quantity, Reason, Date) VALUES 
    (1, 'Laptop', 5, 'Defective', '2025-04-10'),
    (2, 'Chair', 3, 'Worn Out', '2025-04-12');

    INSERT INTO DiscountSales (ProductID, Quantity, OriginalPrice, DiscountPercent, Date) VALUES 
    (1, 10, 1200.50, 10, '2025-04-15'),
    (2, 15, 150.00, 20, '2025-04-16');

    ";
                using (var command = new SQLiteCommand(insertCategories, connection))
                {
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка при створенні тестових даних: " + ex.Message);
                return false;
            }
            finally
            {
                connection?.Close();
            }
        }
    }
}
