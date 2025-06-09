using Newtonsoft.Json;
using Shop.Data;
using Shop.source;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Shop.Dialogs
{
    public partial class ImportDialog : Window
    {
        private MainWindow _mainWindow;
        public string SelectedPath { get; private set; }
        public ImportDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (RadioDirectory.IsChecked == true)
            {
                var dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedPath = dialog.SelectedPath;
                    PathBox.Text = SelectedPath;
                }
            }
            else if (RadioZip.IsChecked == true)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "ZIP-Dateien (*.zip)|*.zip"
                };
                if (dialog.ShowDialog() == true)
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    Directory.CreateDirectory(tempPath);
                    ZipFile.ExtractToDirectory(dialog.FileName, tempPath);
                    SelectedPath = tempPath;
                    PathBox.Text = $"{dialog.FileName} (extrahiert nach {tempPath})";
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedPath) || !Directory.Exists(SelectedPath))
            {
                _mainWindow.AddLogMessage("Please select a valid folder.", "Error");
                return;
            }

            try
            {
                if (JsonCheck.IsChecked == true)
                    ImportJson(SelectedPath);

                if (CsvCheck.IsChecked == true)
                    ImportCsv(SelectedPath);

                if (SqliteCheck.IsChecked == true)
                    ImportSQLite(SelectedPath);

                _mainWindow.AddLogMessage("Import finished successfully!", "Info");
                DialogResult = true;
            }
            catch (Exception ex)
            {
                _mainWindow.AddLogMessage($"Error while importing data: {ex.Message}", "Error");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void ImportJson(string folderPath)
        {
            // Categories
            var catPath = Path.Combine(folderPath, "Categories.json");
            if (File.Exists(catPath))
            {
                var json = File.ReadAllText(catPath);
                var categories = JsonConvert.DeserializeObject<List<Category>>(json);
                foreach (var c in categories)
                    _mainWindow.DBService.InsertCategory(c);
            }

            // Products
            var prodPath = Path.Combine(folderPath, "Products.json");
            if (File.Exists(prodPath))
            {
                var json = File.ReadAllText(prodPath);
                var products = JsonConvert.DeserializeObject<List<Product>>(json);
                foreach (var p in products)
                    _mainWindow.DBService.InsertProduct(p);
            }

            // Invoices
            var invPath = Path.Combine(folderPath, "Invoices.json");
            if (File.Exists(invPath))
            {
                var json = File.ReadAllText(invPath);
                var invoices = JsonConvert.DeserializeObject<List<Invoice>>(json);
                foreach (var i in invoices)
                    _mainWindow.DBService.InsertInvoices(i);
            }

            // InvoiceDetails
            var detPath = Path.Combine(folderPath, "InvoiceDetails.json");
            if (File.Exists(detPath))
            {
                var json = File.ReadAllText(detPath);
                var details = JsonConvert.DeserializeObject<List<InvoiceDetail>>(json);
                foreach (var d in details)
                    _mainWindow.DBService.InsertInvoiceDetails(d);
            }

            // DiscountSales
            var repPath = Path.Combine(folderPath, "DiscountSales.json");
            if (File.Exists(repPath))
            {
                var json = File.ReadAllText(repPath);
                var discounts = JsonConvert.DeserializeObject<List<DiscountSales>>(json);
                foreach (var r in discounts)
                    _mainWindow.DBService.InsertDiscountSales(r);
            }
        }

        public void ImportCsv(string folderPath)
        {
            // Categories
            var catFile = Path.Combine(folderPath, "Categories.csv");
            if (File.Exists(catFile))
            {
                var lines = File.ReadAllLines(catFile);
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    var cat = new Category
                    {
                        CategoryID = int.Parse(parts[0]),
                        CategoryName = parts[1]
                    };
                    _mainWindow.DBService.InsertCategory(cat);
                }
            }

            // Products
            var prodFile = Path.Combine(folderPath, "Products.csv");
            if (File.Exists(prodFile))
            {
                var lines = File.ReadAllLines(prodFile);
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    var prod = new Product
                    {
                        ProductID = int.Parse(parts[0]),
                        CategoryID = int.Parse(parts[1]),
                        Name = parts[2],
                        Unit = parts[3],
                        Price = double.Parse(parts[4], CultureInfo.InvariantCulture),
                        Quantity = double.Parse(parts[5], CultureInfo.InvariantCulture),
                        LastDeliveryDate = parts[6]
                    };
                    _mainWindow.DBService.InsertProduct(prod);
                }
            }

            // Invoices
            var invFile = Path.Combine(folderPath, "Invoices.csv");
            if (File.Exists(invFile))
            {
                var lines = File.ReadAllLines(invFile);
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    var inv = new Invoice
                    {
                        InvoiceID = int.Parse(parts[0]),
                        SaleDate = parts[1],
                        TotalPrice = double.Parse(parts[2], CultureInfo.InvariantCulture)
                    };
                    _mainWindow.DBService.InsertInvoices(inv);
                }
            }

            // InvoiceDetails
            var detFile = Path.Combine(folderPath, "InvoiceDetails.csv");
            if (File.Exists(detFile))
            {
                var lines = File.ReadAllLines(detFile);
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    var detail = new InvoiceDetail
                    {
                        InvoiceID = int.Parse(parts[0]),
                        SequenceNumber = int.Parse(parts[1]),
                        ProductID = int.Parse(parts[2]),
                        ProductName = parts[3],
                        Quantity = double.Parse(parts[4], CultureInfo.InvariantCulture),
                        Unit = parts[5],
                        Price = double.Parse(parts[6], CultureInfo.InvariantCulture)
                    };
                    _mainWindow.DBService.InsertInvoiceDetails(detail);
                }
            }

            // DiscountSales
            var repFile = Path.Combine(folderPath, "DiscountSales.csv");
            if (File.Exists(repFile))
            {
                var lines = File.ReadAllLines(repFile);
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    var rep = new DiscountSales
                    {
                        ProductID = int.Parse(parts[0]),
                        Quantity = double.Parse(parts[1], CultureInfo.InvariantCulture),
                        OriginalPrice = double.Parse(parts[2], CultureInfo.InvariantCulture),
                        DiscountPercent = double.Parse(parts[3], CultureInfo.InvariantCulture),
                        Date = parts[4]
                    };
                    _mainWindow.DBService.InsertDiscountSales(rep);
                }
            }
        }

        public void ImportSQLite(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                _mainWindow.AddLogMessage(string.Format("SQLite-File \"{0}\" not found.", sourceFilePath), "Error");
                return;
            }

            using (var sourceConnection = new SQLiteConnection($"Data Source={sourceFilePath};Version=3;"))
            {
                sourceConnection.Open();

                // Import categories
                using (var cmd = new SQLiteCommand("SELECT * FROM Categories", sourceConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cat = new Category
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString()
                            };
                            _mainWindow.DBService.InsertCategory(cat);
                        }
                    }
                }

                // Import products
                using (var cmd = new SQLiteCommand("SELECT * FROM Products", sourceConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var prod = new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                Name = reader["Name"].ToString(),
                                Unit = reader["Unit"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                                Quantity = Convert.ToDouble(reader["Quantity"]),
                                LastDeliveryDate = reader["LastDeliveryDate"].ToString()
                            };
                            _mainWindow.DBService.InsertProduct(prod);
                        }
                    }
                }

                // Import invoices
                using (var cmd = new SQLiteCommand("SELECT * FROM Invoices", sourceConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var invoice = new Invoice
                            {
                                InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                                SaleDate = reader["SaleDate"].ToString(),
                                TotalPrice = Convert.ToDouble(reader["TotalPrice"])
                            };
                            _mainWindow.DBService.InsertInvoices(invoice);
                        }
                    }
                }

                //Import invoices details
                using (var cmd = new SQLiteCommand("SELECT * FROM InvoiceDetails", sourceConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var detail = new InvoiceDetail
                            {
                                InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                                SequenceNumber = Convert.ToInt32(reader["SequenceNumber"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                Quantity = Convert.ToDouble(reader["Quantity"]),
                                Unit = reader["Unit"].ToString(),
                                Price = Convert.ToDouble(reader["Price"])
                            };
                            _mainWindow.DBService.InsertInvoiceDetails(detail);
                        }
                    }
                }

                // Import Reports
                using (var cmd = new SQLiteCommand("SELECT * FROM DiscountSales", sourceConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var discounts = new DiscountSales
                            {
                                ProductID = Convert.ToInt32(reader["ReportID"]),
                                Date = reader["Date"].ToString(),
                                Quantity = Convert.ToDouble(reader["Quantity"]),
                                OriginalPrice = Convert.ToDouble(reader["RiginalPrice"]),
                                DiscountPercent = Convert.ToDouble(reader["DiscountPercent"])
                            };
                            _mainWindow.DBService.InsertDiscountSales(discounts);
                        }
                    }
                }
                sourceConnection.Close();
            }
        }
    }
}
