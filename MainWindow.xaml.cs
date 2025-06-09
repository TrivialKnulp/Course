using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Shop.Dialogs;
using Shop.source;
using Shop.source.Logging;
using Shop.ViewModels;

namespace Shop
{
    /// <summary>
    /// Головне вікно MainWindow є центральним елементом програми "Складська система".
    /// Відповідає за ініціалізацію підключення до бази даних, створення та завантаження ViewModel,
    /// обробку подій інтерфейсу користувача, логування, а також навігацію між вкладками.
    /// Всі підписи, повідомлення та вкладки, які бачить користувач, локалізовано українською мовою.
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ConnectionString { get; private set; } = string.Empty;
        public DatabaseService DBService { get; private set; }

        #region ViewModels
        private readonly CategoryViewModel _categoryViewModel;
        private readonly ProductViewModel _productViewModel;
        private readonly InvoicesViewModel _invoiceViewModel;
        private readonly DepreciationViewModel _depreciationViewModel;

        public CategoryViewModel CategoryViewModel => _categoryViewModel;
        public ProductViewModel ProductViewModel => _productViewModel;
        public InvoicesViewModel InvoicesViewModel => _invoiceViewModel;
        public DepreciationViewModel DepreciationViewModel => _depreciationViewModel;
        #endregion

        #region MainWindow
        public MainWindow()
        {
            InitializeComponent();
            LogViewModel = new LogViewModel(this);
            DBService = new DatabaseService(this);

            // Перевірка наявності конфігураційного файлу, створення нового або вибір шляху до бази даних
            if (!System.IO.File.Exists(ConfigFilePath))
            {
                string folderPath = SelectFolderPathFromUser();

                if (!string.IsNullOrEmpty(folderPath))
                {
                    string dbPath = System.IO.Path.Combine(folderPath, "Data.db");
                    CreateConfigFile(dbPath);
                }
                else
                {
                    AddLogMessage("Теку не вибрано. Програма буде закрита.", "Error");
                    System.Windows.Application.Current.Shutdown();
                }
            }

            LogMessageDialog = IniHelper.GetLogMessagesDialogs(ConfigFilePath);
            DatabasePath = IniHelper.GetDatabaseFile(ConfigFilePath);

            // Перевірка шляху до бази даних
            if (string.IsNullOrEmpty(DatabasePath))
            {
                AddLogMessage("Шлях до файлу бази даних не вказано у файлі конфігурації \"config.ini\".", "Error");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            // Перевірка наявності файлу бази даних
            if (!System.IO.File.Exists(DatabasePath))
            {
                AddLogMessage("Файл бази даних не знайдено. Перевірте файл конфігурації \"config.ini\".", "Error");

                if (System.Windows.MessageBox.Show("Бажаєте створити нову базу даних?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Створення нової бази даних
                    SQLiteHelper.CreateDatabase(DatabasePath);
                    LogViewModel.AddLog("Створено нову базу даних.", "Info");

                    if (System.Windows.MessageBox.Show("Бажаєте створити тестові дані у базі даних?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SQLiteHelper.CreateTestData(DatabasePath);
                    }
                }
                else
                {
                    LogViewModel.AddLog("Програму буде закрито.", "Info");
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            }

            // Підключення до бази даних SQLite
            ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

            // Створення ViewModel
            _categoryViewModel = new CategoryViewModel(this);
            _productViewModel = new ProductViewModel(this);
            _invoiceViewModel = new InvoicesViewModel(this);
            _depreciationViewModel = new DepreciationViewModel(this);

            // Завантаження категорій, товарів, накладних, списань
            _categoryViewModel.LoadCategories();
            _productViewModel.LoadProducts();
            _invoiceViewModel.LoadInvoices();
            _depreciationViewModel.LoadDepreciations();

            this.DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (int width, int height) = IniHelper.GetWindowSize(ConfigFilePath);
            this.Width = width;
            this.Height = height;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Left = (screenWidth - width) / 2;
            this.Top = (screenHeight - height) / 2;
        }
        #endregion

        #region Settings
        // Дані з "config.ini"
        private const string ConfigFilePath = "config.ini";
        public bool LogMessageDialog;
        private string DatabasePath;
        private string SelectFolderPathFromUser()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Оберіть теку для бази даних";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return null;
        }
        private void CreateConfigFile(string dbPath)
        {
            string configContent = $@"
    [Database]
    File={dbPath}

    [Logging]
    ShowDialogs=OFF

    [Window]
    Width=1280
    Height=960";

            System.IO.File.WriteAllText(ConfigFilePath, configContent.Trim());
            System.Windows.MessageBox.Show("Файл конфігурації успішно створено.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Logging
        public LogViewModel LogViewModel { get; }

        public void AddLogMessage(string message, string level = "Info")
        {
            LogViewModel.AddLog(message, level);
        }
        #endregion

        #region Warehouse UI
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            DatabaseExportDialog dlg = new DatabaseExportDialog(this);
            dlg.ShowDialog();
        }
        private void ImportData_Click(object sender, RoutedEventArgs e)
        {
            ImportDialog dlg = new ImportDialog(this);
            dlg.ShowDialog();
        }
        #endregion

        #region Category UI
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            Dlg_Category dlg = new Dlg_Category(this);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                var newCategory = new Category { CategoryID = dlg.SelectedCategoryID, CategoryName = dlg.CategoryNameTextBox.Text };
                _categoryViewModel.AddCategory(newCategory);
                dataGridCategory.SelectedItem = newCategory;
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_categoryViewModel.SelectedCategory != null)
            {
                Dlg_Category dlg = new Dlg_Category(this);
                dlg.CategoryNameTextBox.Text = _categoryViewModel.SelectedCategory.CategoryName;
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    _categoryViewModel.SelectedCategory.CategoryName = dlg.CategoryNameTextBox.Text;
                    _categoryViewModel.EditCategory(_categoryViewModel.SelectedCategory);
                }
            }
            else
            {
                AddLogMessage("Оберіть категорію для редагування.", "Error");
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_categoryViewModel.SelectedCategory != null)
            {
                _categoryViewModel.DeleteCategory(_categoryViewModel.SelectedCategory);
            }
            else
            {
                AddLogMessage("Оберіть категорію для видалення.", "Error");
            }
        }

        private void DataGridCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGridCategory.SelectedItem is Category selectedCategory)
            {
                _categoryViewModel.SelectedCategory = selectedCategory;
            }
        }
        #endregion

        #region Product UI
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Dlg_Product dlg = new Dlg_Product(this);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                var newProduct = new Product
                {
                    Name = dlg.NameTextBox.Text,
                    CategoryID = Convert.ToInt32(dlg.CategoryComboBox.SelectedValue),
                    CategoryName = dlg.CategoryComboBox.Text,
                    Price = Convert.ToDouble(dlg.PriceTextBox.Text),
                    Unit = dlg.UnitTextBox.Text,
                    Quantity = 0,
                    LastDeliveryDate = ""
                };

                _productViewModel.AddProduct(newProduct);
                dataGridProducts.SelectedItem = newProduct;
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_productViewModel.SelectedProduct != null)
            {
                Dlg_Product dlg = new Dlg_Product(this);
                dlg.NameTextBox.Text = _productViewModel.SelectedProduct.Name;
                dlg.UnitTextBox.Text = _productViewModel.SelectedProduct.Unit;
                dlg.PriceTextBox.Text = _productViewModel.SelectedProduct.Price.ToString();
                dlg.CategoryComboBox.SelectedValue = _productViewModel.SelectedProduct.CategoryID;
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    _productViewModel.SelectedProduct.Name = dlg.NameTextBox.Text;
                    _productViewModel.SelectedProduct.Unit = dlg.UnitTextBox.Text;
                    _productViewModel.SelectedProduct.Price = Convert.ToDouble(dlg.PriceTextBox.Text);
                    _productViewModel.SelectedProduct.CategoryID = Convert.ToInt32(dlg.CategoryComboBox.SelectedValue);
                    _productViewModel.EditProduct(_productViewModel.SelectedProduct);
                }
            }
            else
            {
                AddLogMessage("Оберіть товар для редагування.", "Error");
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_productViewModel.SelectedProduct != null)
            {
                _productViewModel.DeleteProduct(_productViewModel.SelectedProduct);
            }
            else
            {
                AddLogMessage("Оберіть товар для видалення.", "Error");
            }

        }

        private void DataGridProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGridProducts.SelectedItem is Product selectedProduct)
            {
                _productViewModel.SelectedProduct = selectedProduct;
            }
        }

        private void SearchProductName_Click(object sender, RoutedEventArgs e)
        {
            Dlg_SearchProductName dlg = new Dlg_SearchProductName(this);
            dlg.ShowDialog();
        }

        private void SearchProductCategory_Click(object sender, RoutedEventArgs e)
        {
            Dlg_SearchProductByCategory dlg = new Dlg_SearchProductByCategory(this);
            dlg.ShowDialog();
        }

        private void SearchProductPrice_Click(object sender, RoutedEventArgs e)
        {
            Dlg_SearchProductByPrice dlg = new Dlg_SearchProductByPrice(this);
            dlg.ShowDialog();
        }

        private void SearchProductAvailability_Click(object sender, RoutedEventArgs e)
        {
            Dlg_SearchProductByAvailability dlg = new Dlg_SearchProductByAvailability(this);
            dlg.ShowDialog();
        }

        #endregion

        #region Finance UI
        private void PostGoodsReceipt_Click(object sender, RoutedEventArgs e)
        {
            DeliveryDialog dlg = new DeliveryDialog(this);
            dlg.ShowDialog();
            _productViewModel.LoadProducts();
        }
        private void CreateReceipt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CheckoutDialog(this);
            if (dialog.ShowDialog() == true)
            {
                _productViewModel.LoadProducts();
                _invoiceViewModel.LoadInvoices();
            }
        }
        private void SellWithDiscount_Click(object sender, RoutedEventArgs e)
        {
            List<Product> products;
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Products", conn);
                var reader = cmd.ExecuteReader();
                products = new List<Product>();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        Name = reader["Name"].ToString(),
                        Unit = reader["Unit"].ToString(),
                        Price = Convert.ToDouble(reader["Price"]),
                        Quantity = Convert.ToDouble(reader["Quantity"])
                    });
                }
            }

            var dialog = new DiscountSale(products, this);
            dialog.ShowDialog();
            _productViewModel.LoadProducts();
            _invoiceViewModel.LoadInvoices();
        }
        private void DepreciationOfGoods_Click(object sender, RoutedEventArgs e)
        {
            List<Product> products = _productViewModel.GetProducts();
            DepreciationDialog dlg = new DepreciationDialog(products, this);
            dlg.ShowDialog();
            _productViewModel.LoadProducts();
            _depreciationViewModel.LoadDepreciations();
        }
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InventoryDialog(this);
            dialog.Owner = this;
            dialog.ShowDialog();
        }
        #endregion

        #region Reports
        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SalesReportDialog(this);
            dialog.ShowDialog();
        }
        private void StockReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new StockReportDialog(this);
            dialog.ShowDialog();
        }
        private void DepreciationReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DepreciationReportDialog(this);
            dialog.ShowDialog();
        }
        private void DiscountReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DiscountReportDialog(this);
            dialog.ShowDialog();

        }
        #endregion
    }
}

