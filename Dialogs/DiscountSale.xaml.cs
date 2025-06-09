using Shop.source;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace Shop.Dialogs
{
    public partial class DiscountSale : Window
    {
        private MainWindow _mainWindow;
        public ObservableCollection<SaleItem> SaleItems { get; set; } = new ObservableCollection<SaleItem>();

        public DiscountSale(List<Product> availableProducts, MainWindow main)
        {
            _mainWindow = main;
            InitializeComponent();
            foreach (var p in availableProducts)
            {
                SaleItems.Add(new SaleItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.Name,
                    Unit = p.Unit,
                    Price = p.Price,
                    Quantity = 0,
                    Discount = 0
                });
            }

            dataGridSale.ItemsSource = SaleItems;
            SaleItems.CollectionChanged += (sender, e) => UpdateTotal();
            foreach (var item in SaleItems)
                item.PropertyChanged += (sender, e) => UpdateTotal();
        }

        private void UpdateTotal()
        {
            double total = SaleItems.Sum(i => i.Total);
            textTotalSum.Text = total.ToString("F2") +  "€";
        }

        private void FinalizeSale_Click(object sender, RoutedEventArgs e)
        {
            var soldItems = SaleItems.Where(i => i.Quantity > 0).ToList();
            if (!soldItems.Any())
            {
                _mainWindow.AddLogMessage("No items to sell.", "Error");
                return;
            }

            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();

                try
                {
                    var saleDate = DateTime.Now.ToString("yyyy-MM-dd");
                    // Insert Invoice
                    var insertInvoiceCmd = new SQLiteCommand("INSERT INTO Invoices (SaleDate, TotalPrice) VALUES (@Date, @Total)", conn);
                    insertInvoiceCmd.Parameters.AddWithValue("@Date", saleDate);
                    insertInvoiceCmd.Parameters.AddWithValue("@Total", soldItems.Sum(i => i.Total));
                    insertInvoiceCmd.ExecuteNonQuery();

                    long invoiceId = conn.LastInsertRowId;

                    // Insert InvoiceDetails
                    int seq = 1;
                    foreach (var item in soldItems)
                    {
                        var cmd = new SQLiteCommand("INSERT INTO InvoiceDetails (InvoiceID, SequenceNumber, ProductID, ProductName, Quantity, Unit, Discount, Price) " +
                            "VALUES (@InvoiceID, @Seq, @ProductID, @ProductName, @Quantity, @Unit, @Discount, @Price)", conn);

                        cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                        cmd.Parameters.AddWithValue("@Seq", seq++);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@ProductName", item.ProductName);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@Unit", item.Unit);
                        cmd.Parameters.AddWithValue("@Discount", item.Discount);
                        cmd.Parameters.AddWithValue("@Price", item.Total);
                        cmd.ExecuteNonQuery();

                        // Update product quantity
                        var updateCmd = new SQLiteCommand("UPDATE Products SET Quantity = Quantity - @Qty WHERE ProductID = @ProductID", conn);
                        updateCmd.Parameters.AddWithValue("@Qty", item.Quantity);
                        updateCmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        updateCmd.ExecuteNonQuery();

                        // Insert into DiscountSales
                        var discountCmd = new SQLiteCommand("INSERT INTO DiscountSales (ProductId, Quantity, OriginalPrice, DiscountPercent, Date) " +
                            "VALUES (@ProductId, @Quantity, @OriginalPrice, @DiscountPercent, @Date)", conn);
                        discountCmd.Parameters.AddWithValue("@ProductId", item.ProductID);
                        discountCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        discountCmd.Parameters.AddWithValue("@OriginalPrice", item.Price);
                        discountCmd.Parameters.AddWithValue("@DiscountPercent", item.Discount);
                        discountCmd.Parameters.AddWithValue("@Date", saleDate);
                        discountCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    _mainWindow.AddLogMessage("Discount sale completed.", "Info");
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _mainWindow.AddLogMessage("Error during sale: " + ex.Message);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
