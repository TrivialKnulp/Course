using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Shop.Data
{
    /// <summary>
    /// Клас InvoiceRepository відповідає за доступ до даних накладних та їх деталей у базі даних.
    /// Забезпечує отримання списку всіх накладних та деталей конкретної накладної.
    /// </summary>
    public class InvoiceRepository
    {
        /// <summary>
        /// Посилання на головне вікно програми для доступу до рядка підключення.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Конструктор. Ініціалізує репозиторій накладних з посиланням на головне вікно.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public InvoiceRepository(MainWindow main)
        {
            _mainWindow = main;
        }

        /// <summary>
        /// Отримує список усіх накладних з бази даних.
        /// </summary>
        /// <returns>Список об'єктів Invoice.</returns>
        public List<Invoice> GetAllInvoices()
        {
            List<Invoice> invoices = new List<Invoice>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT * FROM Invoices", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        invoices.Add(new Invoice
                        {
                            InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                            SaleDate = reader["SaleDate"].ToString(),
                            TotalPrice = Convert.ToDouble(reader["TotalPrice"])
                        });
                    }
                }
            }

            return invoices;
        }

        /// <summary>
        /// Отримує деталі конкретної накладної за її ідентифікатором.
        /// </summary>
        /// <param name="invoiceId">Ідентифікатор накладної.</param>
        /// <returns>Список об'єктів InvoiceDetail.</returns>
        public List<InvoiceDetail> GetInvoiceDetails(int invoiceId)
        {
            List<InvoiceDetail> details = new List<InvoiceDetail>();

            using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT InvoiceID, SequenceNumber, ProductID, ProductName, Quantity, Unit, Discount, Price FROM InvoiceDetails WHERE InvoiceID = @InvoiceID", connection);
                command.Parameters.AddWithValue("@InvoiceID", invoiceId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        details.Add(new InvoiceDetail
                        {
                            InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                            SequenceNumber = Convert.ToInt32(reader["SequenceNumber"]),
                            ProductID = Convert.ToInt32(reader["ProductID"]),
                            ProductName = reader["ProductName"].ToString(),
                            Unit = reader["Unit"].ToString(),
                            Discount = Convert.ToDouble(reader["Discount"]),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            Price = Convert.ToDouble(reader["Price"])
                        });
                    }
                }
            }

            return details;
        }
    }

    /// <summary>
    /// Клас Invoice використовується для представлення накладної у системі.
    /// Містить ідентифікатор накладної, дату продажу та загальну суму.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Унікальний ідентифікатор накладної.
        /// </summary>
        public int InvoiceID { get; set; }

        /// <summary>
        /// Дата продажу.
        /// </summary>
        public string SaleDate { get; set; }

        /// <summary>
        /// Загальна сума накладної.
        /// </summary>
        public double TotalPrice { get; set; }
    }

    /// <summary>
    /// Клас InvoiceDetail використовується для представлення деталі накладної у системі.
    /// Містить ідентифікатор накладної, порядковий номер, ідентифікатор та назву товару, одиницю виміру, знижку, кількість та ціну.
    /// </summary>
    public class InvoiceDetail
    {
        /// <summary>
        /// Ідентифікатор накладної.
        /// </summary>
        public int InvoiceID { get; set; }

        /// <summary>
        /// Порядковий номер у накладній.
        /// </summary>
        public int SequenceNumber { get; set; }

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

        /// <summary>
        /// Знижка на товар.
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// Кількість товару.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Ціна одиниці товару.
        /// </summary>
        public double Price { get; set; }
    }
}
