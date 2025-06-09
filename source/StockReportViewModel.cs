using Shop.source;
using Shop;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;

/// <summary>
/// Клас StockReportViewModel використовується для формування та зберігання даних звіту по залишках товарів на складі.
/// Завантажує дані з бази даних, формує колекцію рядків звіту та обчислює загальну вартість залишків.
/// Реалізує механізм сповіщення про зміни для підтримки двостороннього зв'язку з UI (INotifyPropertyChanged).
/// </summary>
public class StockReportViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Посилання на головне вікно програми для доступу до рядка підключення.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// Колекція рядків звіту по залишках для відображення у UI.
    /// </summary>
    public ObservableCollection<StockReportItem> StockItems { get; set; } = new ObservableCollection<StockReportItem>();

    /// <summary>
    /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Загальна вартість залишків (сума TotalValue по всіх рядках звіту).
    /// </summary>
    public double TotalInventoryValue => StockItems.Sum(x => x.TotalValue);

    /// <summary>
    /// Конструктор. Ініціалізує ViewModel, завантажує дані звіту по залишках.
    /// </summary>
    /// <param name="main">Головне вікно програми.</param>
    public StockReportViewModel(MainWindow main)
    {
        _mainWindow = main;
        LoadStockData();
    }

    /// <summary>
    /// Завантажує дані по залишках з бази даних та формує колекцію рядків звіту.
    /// </summary>
    private void LoadStockData()
    {
        using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
        {
            connection.Open();

            string query = @"
            SELECT c.CategoryName, p.Name AS ProductName, p.Quantity, p.Unit, p.Price
            FROM Products p
            JOIN Categories c ON p.CategoryID = c.CategoryID
            ORDER BY c.CategoryName, p.Name";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StockItems.Add(new StockReportItem
                        {
                            CategoryName = reader["CategoryName"].ToString(),
                            ProductName = reader["ProductName"].ToString(),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            Unit = reader["Unit"].ToString(),
                            Price = Convert.ToDouble(reader["Price"])
                        });
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalInventoryValue)));
                }
            }
        }
    }
}
