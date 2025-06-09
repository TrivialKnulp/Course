using Shop.source;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class InventoryDialog : Window, INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        public ObservableCollection<InventoryItem> InventoryItems { get; set; } = new ObservableCollection<InventoryItem>();

        public double TotalValue => InventoryItems.Sum(i => i.Subtotal);

        public InventoryDialog(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            DataContext = this;
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();

                var cmd = new SQLiteCommand("SELECT Name, Unit, Price FROM Products", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        InventoryItems.Add(new InventoryItem
                        {
                            ProductName = reader["Name"].ToString(),
                            Unit = reader["Unit"].ToString(),
                            Price = Convert.ToDouble(reader["Price"]),
                            CountedQuantity = 0
                        });
                    }

                    foreach (var item in InventoryItems)
                    {
                        item.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Subtotal")
                                OnPropertyChanged(nameof(TotalValue));
                        };
                    }
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void DataGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            OnPropertyChanged(nameof(TotalValue));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
