using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class CheckoutDialog : Window
    {
        private readonly SQLiteConnection _connection;
        private readonly MainWindow _mainWindow;
        private readonly ObservableCollection<CartItem> _cartItems = new ObservableCollection<CartItem>();

        public CheckoutDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            _connection = new SQLiteConnection(_mainWindow.ConnectionString);
            _connection.Open();
            LoadAvailableProducts();
            CartDataGrid.ItemsSource = _cartItems;
        }

        private void LoadAvailableProducts()
        {
            var cmd = new SQLiteCommand("SELECT ProductID, Name, Unit, Price, Quantity FROM Products WHERE Quantity > 0", _connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var item = new CartItem
                {
                    ProductID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Unit = reader.GetString(2),
                    Price = reader.GetDouble(3),
                    Quantity = 0,
                    Available = reader.GetDouble(4)
                };

                item.QuantityChanged += CartItem_QuantityChanged;

                _cartItems.Add(item);
            }

            UpdateTotal();
        }

        private void CartItem_QuantityChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            double total = _cartItems.Sum(p => p.Total);
            TotalTextBlock.Text = total.ToString("F2") + " €";
        }

        private void CompletePurchase_Click(object sender, RoutedEventArgs e)
        {
            var itemsToBuy = _cartItems.Where(p => p.Quantity > 0).ToList();

            if (!itemsToBuy.Any())
            {
                _mainWindow.AddLogMessage("Please enter product and Quantity.", "Error");
                return;
            }

            foreach (var item in itemsToBuy)
            {
                if (item.Quantity > item.Available)
                {
                    _mainWindow.AddLogMessage($"Not enough stock for {item.ProductName}", "Error");
                    return;
                }
            }

            using (var transaction = _connection.BeginTransaction())
            {
                var saleDate = DateTime.Now.ToString("yyyy-MM-dd");
                var totalPrice = itemsToBuy.Sum(p => p.Total);

                var insertInvoice = new SQLiteCommand("INSERT INTO Invoices (SaleDate, TotalPrice) VALUES (@Date, @Total)", _connection);
                insertInvoice.Parameters.AddWithValue("@Date", saleDate);
                insertInvoice.Parameters.AddWithValue("@Total", totalPrice);
                insertInvoice.ExecuteNonQuery();

                long invoiceId = _connection.LastInsertRowId;

                int sequence = 1;
                foreach (var item in itemsToBuy)
                {
                    var detailCmd = new SQLiteCommand(@"
                INSERT INTO InvoiceDetails (InvoiceID, SequenceNumber, ProductID, ProductName, Quantity, Unit, Discount, Price)
                VALUES (@InvoiceID, @Seq, @ProductID, @Name, @Qty, @Unit, @Discount, @Price)", _connection);

                    detailCmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    detailCmd.Parameters.AddWithValue("@Seq", sequence++);
                    detailCmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                    detailCmd.Parameters.AddWithValue("@Name", item.ProductName);
                    detailCmd.Parameters.AddWithValue("@Qty", item.Quantity);
                    detailCmd.Parameters.AddWithValue("@Unit", item.Unit);
                    detailCmd.Parameters.AddWithValue("@Discount", 0);
                    detailCmd.Parameters.AddWithValue("@Price", item.Price);
                    detailCmd.ExecuteNonQuery();

                    var updateStock = new SQLiteCommand("UPDATE Products SET Quantity = Quantity - @Qty WHERE ProductID = @ID", _connection);
                    updateStock.Parameters.AddWithValue("@Qty", item.Quantity);
                    updateStock.Parameters.AddWithValue("@ID", item.ProductID);
                    updateStock.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            _mainWindow.AddLogMessage("Purchase completed successfully.\nReceipt has been created.", "Info");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class CartItem : INotifyPropertyChanged
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Unit { get; set; } = "";
        public double Price { get; set; }
        public double Available { get; set; }

        private double _quantity;
        public double Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Total));
                    QuantityChanged?.Invoke(this, EventArgs.Empty); 
                }
            }
        }

        public double Total => Math.Round(Quantity * Price, 2);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Optional: Event, to send to Upper UI
        public event EventHandler QuantityChanged;
    }

}
