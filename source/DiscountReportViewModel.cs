using Shop.source;
using Shop;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;

/// <summary>
/// Клас DiscountReportViewModel використовується для формування та зберігання даних звіту по продажах зі знижкою.
/// Завантажує дані з бази даних, формує колекцію рядків звіту та обчислює загальну суму знижок.
/// Реалізує механізм сповіщення про зміни для підтримки двостороннього зв'язку з UI (INotifyPropertyChanged).
/// </summary>
public class DiscountReportViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Посилання на головне вікно програми для доступу до рядка підключення.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// Колекція рядків звіту по продажах зі знижкою для відображення у UI.
    /// </summary>
    public ObservableCollection<DiscountReportItem> DiscountItems { get; set; } = new ObservableCollection<DiscountReportItem>();

    /// <summary>
    /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Загальна сума знижок (сума DiscountAmount по всіх рядках звіту).
    /// </summary>
    public double TotalDiscount => DiscountItems.Sum(x => x.DiscountAmount);

    /// <summary>
    /// Конструктор. Ініціалізує ViewModel, завантажує дані звіту по продажах зі знижкою.
    /// </summary>
    /// <param name="mainWindow">Головне вікно програми.</param>
    public DiscountReportViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        LoadDiscountData();
    }

    /// <summary>
    /// Завантажує дані по продажах зі знижкою з бази даних та формує колекцію рядків звіту.
    /// </summary>
    private void LoadDiscountData()
    {
        using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
        {
            connection.Open();

            string query = @"
            SELECT s.Date, p.Name AS ProductName, c.CategoryName, s.Quantity, s.OriginalPrice, s.DiscountPercent
            FROM DiscountSales s
            JOIN Products p ON s.ProductId = p.ProductId
            JOIN Categories c ON p.CategoryId = c.CategoryId
            ORDER BY s.Date DESC";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DiscountItems.Add(new DiscountReportItem
                        {
                            SaleDate = Convert.ToDateTime(reader["Date"]),
                            ProductName = reader["ProductName"].ToString(),
                            CategoryName = reader["CategoryName"].ToString(),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            OriginalPrice = Convert.ToDouble(reader["OriginalPrice"]),
                            DiscountPercent = Convert.ToDouble(reader["DiscountPercent"])
                        });
                    }
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalDiscount)));
            }
        }
    }
}
