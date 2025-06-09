using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Shop.source
{
    /// <summary>
    /// Клас DepreciationRepository відповідає за доступ до даних амортизації (списання) товарів у базі даних.
    /// Забезпечує отримання всіх записів амортизації для подальшого використання у ViewModel або звітах.
    /// </summary>
    class DepreciationRepository
    {
        /// <summary>
        /// Рядок підключення до бази даних SQLite.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Посилання на головне вікно для доступу до налаштувань підключення.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Конструктор. Ініціалізує репозиторій амортизації з посиланням на головне вікно.
        /// </summary>
        /// <param name="main">Головне вікно програми.</param>
        public DepreciationRepository(MainWindow main)
        {
            _mainWindow = main;
            _connectionString = _mainWindow.ConnectionString;
        }

        /// <summary>
        /// Отримує всі записи амортизації з бази даних.
        /// </summary>
        /// <returns>Колекція об'єктів Depreciation.</returns>
        public ObservableCollection<Depreciation> GetAllDepreciations()
        {
            var depreciations = new ObservableCollection<Depreciation>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT DepreciationID, ProductID, Productname, Quantity, Reason, Date
                                    FROM Depreciations";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            depreciations.Add(new Depreciation
                            {
                                DepreciationID = reader.GetInt32(0),
                                ProductID = reader.GetInt32(1),
                                ProductName = reader.GetString(2),
                                Quantity = reader.GetDouble(3),
                                Reason = reader.GetString(4),
                                Date = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return depreciations;
        }
    }
}
