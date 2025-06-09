using Shop.source;
using Shop;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;

/// <summary>
/// Клас DepreciationReportViewModel використовується для формування та зберігання даних звіту по амортизації (списанню) товарів.
/// Завантажує дані з бази даних, формує колекцію рядків звіту та обчислює загальні втрати від амортизації.
/// Реалізує механізм сповіщення про зміни для підтримки двостороннього зв'язку з UI (INotifyPropertyChanged).
/// </summary>
public class DepreciationReportViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Посилання на головне вікно програми для доступу до рядка підключення.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// Колекція рядків звіту по амортизації для відображення у UI.
    /// </summary>
    public ObservableCollection<DepreciationReportItem> DepreciationItems { get; set; } = new ObservableCollection<DepreciationReportItem>();

    /// <summary>
    /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Загальна сума втрат від амортизації (сума TotalLoss по всіх рядках звіту).
    /// </summary>
    public double TotalDepreciationLoss => DepreciationItems.Sum(x => x.TotalLoss);

    /// <summary>
    /// Конструктор. Ініціалізує ViewModel, завантажує дані звіту по амортизації.
    /// </summary>
    /// <param name="mainWindow">Головне вікно програми.</param>
    public DepreciationReportViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        LoadDepreciationData();
    }

    /// <summary>
    /// Завантажує дані по амортизації з бази даних та формує колекцію рядків звіту.
    /// </summary>
    private void LoadDepreciationData()
    {
        using (var connection = new SQLiteConnection(_mainWindow.ConnectionString))
        {
            connection.Open();

            string query = @"
                        SELECT d.Date, p.Name AS ProductName, c.CategoryName, d.Quantity, p.Price
                        FROM Depreciations d
                        JOIN Products p ON d.ProductId = p.ProductId
                        JOIN Categories c ON p.CategoryId = c.CategoryId
                        ORDER BY d.Date DESC";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DepreciationItems.Add(new DepreciationReportItem
                        {
                            Date = Convert.ToDateTime(reader["Date"]),
                            ProductName = reader["ProductName"].ToString(),
                            CategoryName = reader["CategoryName"].ToString(),
                            Quantity = Convert.ToDouble(reader["Quantity"]),
                            Price = Convert.ToDouble(reader["Price"])
                        });
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalDepreciationLoss)));
                }
            }
        }
    }
}
