using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Shop.Dialogs
{
    public partial class DatabaseExportDialog : Window
    {
        private MainWindow _mainWindow;
        private readonly List<string> tables = new List<string>() 
            { "Categories", "Products", "Invoices", "InvoiceDetails", "DiscountSales", "Deliveries", "Depreciations" };

        public DatabaseExportDialog(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder for the export";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    PathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text) || !Directory.Exists(PathTextBox.Text))
            {
                _mainWindow.AddLogMessage("Please select a valid folder!", "Error");
                return;
            }

            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                if (RadioJson.IsChecked == true)
                    ExportAsJson(tempDir);
                else if (RadioCsv.IsChecked == true)
                    ExportAsCsv(tempDir);
                else if (RadioSqlite.IsChecked == true)
                    ExportAsSQLite(tempDir);

                string backupPath = Path.Combine(PathTextBox.Text, $"Backup_{DateTime.Now:yyyy-MM-dd_HH-mm}.zip");
                ZipFile.CreateFromDirectory(tempDir, backupPath);
                StatusText.Text = $"Backup saved at:\n{backupPath}";

                Directory.Delete(tempDir, true);
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }

        private void ExportAsJson(string directory)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();

                foreach (var table in tables)
                {
                    var cmd = new SQLiteCommand($"SELECT * FROM {table}", conn);
                    var reader = cmd.ExecuteReader();

                    var rows = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                        rows.Add(row);
                    }

                    var json = JsonConvert.SerializeObject(rows, Formatting.Indented);
                    File.WriteAllText(Path.Combine(directory, table + ".json"), json, Encoding.UTF8);
                }
            }
        }

        private void ExportAsCsv(string directory)
        {
            using (var conn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                conn.Open();

                foreach (var table in tables)
                {
                    var cmd = new SQLiteCommand($"SELECT * FROM {table}", conn);
                    var reader = cmd.ExecuteReader();

                    using (var writer = new StreamWriter(Path.Combine(directory, table + ".csv"), false, Encoding.UTF8))
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                            writer.Write(reader.GetName(i) + (i < reader.FieldCount - 1 ? ";" : ""));
                        writer.WriteLine();

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var val = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                                writer.Write(val + (i < reader.FieldCount - 1 ? ";" : ""));
                            }
                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        private void ExportAsSQLite(string directory)
        {
            string newDbPath = Path.Combine(directory, "ExportedDatabase.sqlite");
            SQLiteConnection.CreateFile(newDbPath);
            using (var sourceConn = new SQLiteConnection(_mainWindow.ConnectionString))
            {
                using (var destConn = new SQLiteConnection($"Data Source={newDbPath}"))
                {
                    sourceConn.Open();
                    destConn.Open();

                    foreach (var table in tables)
                    {
                        // clone structure
                        string schemaCmd = $"SELECT sql FROM sqlite_master WHERE type='table' AND name='{table}'";
                        string tableSchema = new SQLiteCommand(schemaCmd, sourceConn).ExecuteScalar()?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(tableSchema))
                            new SQLiteCommand(tableSchema, destConn).ExecuteNonQuery();

                        // Copy data
                        var readCmd = new SQLiteCommand($"SELECT * FROM {table}", sourceConn);
                        var reader = readCmd.ExecuteReader();

                        while (reader.Read())
                        {
                            var columns = new List<string>();
                            var values = new List<string>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                object val = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                values.Add(val == null ? "NULL" :
                                    (val is string || val is DateTime)
                                        ? $"'{val.ToString().Replace("'", "''")}'"
                                        : val.ToString());
                            }

                            var insert = $"INSERT INTO {table} ({string.Join(",", columns)}) VALUES ({string.Join(",", values)});";
                            new SQLiteCommand(insert, destConn).ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
