using System;

namespace Shop.source.Logging
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class LogViewModel : INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        public ObservableCollection<LogEntry> Logs { get; private set; }

        public LogViewModel(MainWindow main)
        {
            Logs = new ObservableCollection<LogEntry>();
            _mainWindow = main;
        }

        public void AddLog(string message, string level = "Info")
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Level = level
            };
            if (_mainWindow.LogMessageDialog || level.ToUpper() == "ERROR" || level.ToUpper() == "EXCEPTION")
            {
                if (level.ToUpper() == "ERROR")
                    MessageBox.Show(message, level, MessageBoxButton.OK, MessageBoxImage.Error);
                else if (level.ToUpper() == "EXCEPTION")
                    MessageBox.Show(message, level, MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (level.ToUpper() == "INFO")
                    MessageBox.Show(message, level, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(message, level, MessageBoxButton.OK);
            }
            Logs.Insert(0, entry);

            OnPropertyChanged(nameof(Logs));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
