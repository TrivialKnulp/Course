using Shop.Data;
using Shop.source;
using System.Data.SQLite;

namespace Shop
{
    /// <summary>
    /// Клас DatabaseService відповідає за виконання операцій вставки (додавання) даних у базу даних SQLite.
    /// Дозволяє додавати категорії, товари, накладні, деталі накладних, поставки, амортизації та продажі зі знижкою.
    /// Всі операції виконуються через підключення до бази даних, визначене у головному вікні програми.
    /// </summary>
    public class DatabaseService
    {
        /// <summary>
        /// Посилання на головне вікно програми для доступу до рядка підключення.
        /// </summary>
        MainWindow _mainWindow;

        /// <summary>
        /// Конструктор. Ініціалізує сервіс бази даних з посиланням на головне вікно.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public DatabaseService(MainWindow main)
        {
            _mainWindow = main;
        }

        /// <summary>
        /// Додає нову категорію до таблиці Categories.
        /// </summary>
        /// <param name="category">Категорія для додавання.</param>
        public void InsertCategory(Category category)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Categories (CategoryID, CategoryName) VALUES (@id, @name)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", category.CategoryID);
                    cmd.Parameters.AddWithValue("@name", category.CategoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Додає новий товар до таблиці Products.
        /// </summary>
        /// <param name="product">Товар для додавання.</param>
        public void InsertProduct(Product product)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Products (ProductID, Name, CategoryId, Quantity, Unit, Price, LastDeliveryDate) " +
                    "VALUES (@id, @name, @catId, @qty, @unit, @price, @date)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.ProductID);
                    cmd.Parameters.AddWithValue("@name", product.Name);
                    cmd.Parameters.AddWithValue("@catId", product.CategoryID);
                    cmd.Parameters.AddWithValue("@qty", product.Quantity);
                    cmd.Parameters.AddWithValue("@unit", product.Unit);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@date", product.LastDeliveryDate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Додає нову накладну до таблиці Invoices.
        /// </summary>
        /// <param name="invoice">Накладна для додавання.</param>
        public void InsertInvoices(Invoice invoice)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Invoices (InvoiceID, SaleDate, TotalPrice) " +
                    "VALUES (@id, @date, @totalprice)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", invoice.InvoiceID);
                    cmd.Parameters.AddWithValue("@date", invoice.SaleDate);
                    cmd.Parameters.AddWithValue("@totalprice", invoice.TotalPrice);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Додає новий запис до таблиці InvoiceDetails (деталі накладної).
        /// </summary>
        /// <param name="invoiceDetail">Деталь накладної для додавання.</param>
        public void InsertInvoiceDetails(InvoiceDetail invoiceDetail)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("INSERT OR IGNORE INTO InvoiceDetails (InvoiceID, SequenceNumber, ProductID, ProductName, Quantity, Unit, Discount, Price) " +
                    "VALUES (@id, @sequenceNo, @productID, @productName, @quantity, @unit, @discount, @price)", conn))
                {
                    cmd.Parameters.AddWithValue("@id", invoiceDetail.InvoiceID);
                    cmd.Parameters.AddWithValue("@sequenceNo", invoiceDetail.SequenceNumber);
                    cmd.Parameters.AddWithValue("@productID", invoiceDetail.ProductID);
                    cmd.Parameters.AddWithValue("@productName", invoiceDetail.ProductName);
                    cmd.Parameters.AddWithValue("@quantity", invoiceDetail.Quantity);
                    cmd.Parameters.AddWithValue("@unit", invoiceDetail.Unit);
                    cmd.Parameters.AddWithValue("@price", invoiceDetail.Price);
                    cmd.Parameters.AddWithValue("@discount", invoiceDetail.Discount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Додає нову поставку до таблиці Deliveries.
        /// </summary>
        /// <param name="delivery">Поставка для додавання.</param>
        public void InsertDeliveries(Delivery delivery)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                var insertCmd = new SQLiteCommand("INSERT INTO Deliveries (ProductID, Quantity, DeliveryDate) VALUES (@ProductID, @Quantity, @Date)", conn);
                insertCmd.Parameters.AddWithValue("@ProductID", delivery.ProductID);
                insertCmd.Parameters.AddWithValue("@Quantity", delivery.Quantity);
                insertCmd.Parameters.AddWithValue("@Date", delivery.DeliveryDate);
                insertCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Додає новий запис про амортизацію до таблиці Depreciations.
        /// </summary>
        /// <param name="depreciation">Амортизація для додавання.</param>
        public void InsertDepreciations(Depreciation depreciation)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                var insertCmd = new SQLiteCommand("INSERT OR IGNORE INTO Depreciations (DepreciationID, ProductID, ProductName, Quantity, Reason, Date) " +
                    "VALUES (@DepreciationID, @ProductID, @ProductName, @Quantity, @Reason, @Date)", conn);
                insertCmd.Parameters.AddWithValue("@DepreciationID", depreciation.DepreciationID);
                insertCmd.Parameters.AddWithValue("@ProductID", depreciation.ProductID);
                insertCmd.Parameters.AddWithValue("@ProductName", depreciation.ProductName);
                insertCmd.Parameters.AddWithValue("@Quantity", depreciation.Quantity);
                insertCmd.Parameters.AddWithValue("@Reason", depreciation.Reason);
                insertCmd.Parameters.AddWithValue("@Date", depreciation.Date);
                insertCmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Додає новий запис про продаж зі знижкою до таблиці DiscountSales.
        /// </summary>
        /// <param name="discount">Продаж зі знижкою для додавання.</param>
        public void InsertDiscountSales(DiscountSales discount)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                var insertCmd = new SQLiteCommand("INSERT INTO DiscountSales (ProductID, Quantity, OriginalPrice, DiscountPercent, Date) VALUES (@ProductID, @Quantity, @OriginalPrice, @DiscountPercent, @Date)", conn);
                insertCmd.Parameters.AddWithValue("@ProductID", discount.ProductID);
                insertCmd.Parameters.AddWithValue("@Quantity", discount.Quantity);
                insertCmd.Parameters.AddWithValue("@OriginalPrice", discount.OriginalPrice);
                insertCmd.Parameters.AddWithValue("@DiscountPercent", discount.DiscountPercent);
                insertCmd.Parameters.AddWithValue("@Date", discount.Date);
                insertCmd.ExecuteNonQuery();
            }
        }
    }
}
