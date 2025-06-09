using Shop;
using Shop.source;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;

/// <summary>
/// Клас SalesReportViewModel використовується для формування та зберігання даних звіту по продажах товарів.
/// Завантажує дані з бази даних, формує колекцію рядків звіту та обчислює загальну суму продажів.
/// </summary>
public class SalesReportViewModel
{
    /// <summary>
    /// Посилання на головне вікно програми для доступу до рядка підключення.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// Колекція рядків звіту по продажах для відображення у UI.
    /// </summary>
    public ObservableCollection<SaleReportItem> Sales { get; set; } = new ObservableCollection<SaleReportItem>();

    /// <summary>
    /// Загальна сума продажів (сума TotalPrice по всіх рядках звіту).
    /// </summary>
    public double TotalSales => Sales.Sum(x => x.TotalPrice);

    /// <summary>
    /// Конструктор. Ініціалізує ViewModel, завантажує дані звіту по продажах.
    /// </summary>
    /// <param name="main">Головне вікно програми.</param>
    public SalesReportViewModel(MainWindow main)
    {
        _mainWindow = main;
        LoadSalesReport();
    }

    /// <summary>
    /// Завантажує дані по продажах з бази даних та формує колекцію рядків звіту.
    /// </summary>
    private void LoadSalesReport()
    {
        using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
        {
            connection.Open();
            string query = @"
                SELECT i.SaleDate, d.ProductName, d.Quantity, d.Unit, d.Price
                FROM InvoiceDetails d
                JOIN Invoices i ON d.InvoiceID = i.InvoiceID
                ORDER BY i.SaleDate DESC";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Sales.Add(new SaleReportItem
                    {
                        SaleDate = DateTime.Parse(reader["SaleDate"].ToString()),
                        ProductName = reader["ProductName"].ToString(),
                        Quantity = Convert.ToDouble(reader["Quantity"]),
                        Unit = reader["Unit"].ToString(),
                        UnitPrice = Convert.ToDouble(reader["Price"])
                    });
                }
            }
        }
    }
}
