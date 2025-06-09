using System.Collections.ObjectModel;
using Shop.Data;
using System.ComponentModel;

namespace Shop.ViewModels
{
    /// <summary>
    /// Клас InvoicesViewModel є посередником між інтерфейсом користувача та даними накладних і їхніх деталей.
    /// Забезпечує завантаження списку накладних, вибір накладної, завантаження деталей накладної та сповіщення UI про зміни.
    /// Реалізує механізм INotifyPropertyChanged для підтримки двостороннього зв'язку з інтерфейсом.
    /// </summary>
    public class InvoicesViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Колекція накладних для відображення у UI.
        /// </summary>
        private ObservableCollection<Invoice> _invoices;

        /// <summary>
        /// Колекція деталей вибраної накладної для відображення у UI.
        /// </summary>
        private ObservableCollection<InvoiceDetail> _invoiceDetails;

        /// <summary>
        /// Поточно вибрана накладна.
        /// </summary>
        private Invoice _selectedInvoice;

        /// <summary>
        /// Посилання на головне вікно програми.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Репозиторій для доступу до даних накладних.
        /// </summary>
        private readonly InvoiceRepository _invoiceRepository;

        /// <summary>
        /// Колекція накладних для відображення у UI.
        /// </summary>
        public ObservableCollection<Invoice> Invoices
        {
            get { return _invoices; }
            set
            {
                _invoices = value;
                OnPropertyChanged(nameof(Invoices));
            }
        }

        /// <summary>
        /// Колекція деталей вибраної накладної для відображення у UI.
        /// </summary>
        public ObservableCollection<InvoiceDetail> InvoiceDetails
        {
            get { return _invoiceDetails; }
            set
            {
                _invoiceDetails = value;
                OnPropertyChanged(nameof(InvoiceDetails));
            }
        }

        /// <summary>
        /// Репозиторій для доступу до даних накладних (тільки для читання).
        /// </summary>
        public InvoiceRepository InvoiceRepository
        {
            get { return _invoiceRepository; }
        }

        /// <summary>
        /// Поточно вибрана накладна.
        /// </summary>
        public Invoice SelectedInvoice
        {
            get { return _selectedInvoice; }
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged(nameof(SelectedInvoice));
                UpdateInvoiceDetails();
            }
        }

        /// <summary>
        /// Завантажує всі накладні з бази даних та очищає деталі.
        /// </summary>
        public void LoadInvoices()
        {
            Invoices = new ObservableCollection<Invoice>(_invoiceRepository.GetAllInvoices());
            InvoiceDetails = new ObservableCollection<InvoiceDetail>();
        }

        /// <summary>
        /// Конструктор. Ініціалізує ViewModel, завантажує список накладних.
        /// </summary>
        /// <param name="mainWindow">Головне вікно програми.</param>
        public InvoicesViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _invoiceRepository = new InvoiceRepository(_mainWindow);

            // Завантажити всі накладні з бази даних
            Invoices = new ObservableCollection<Invoice>(_invoiceRepository.GetAllInvoices());
            InvoiceDetails = new ObservableCollection<InvoiceDetail>();
        }

        /// <summary>
        /// Оновлює колекцію деталей для вибраної накладної.
        /// </summary>
        private void UpdateInvoiceDetails()
        {
            if (SelectedInvoice != null)
            {
                // Завантажити деталі для вибраної накладної
                var details = _invoiceRepository.GetInvoiceDetails(SelectedInvoice.InvoiceID);
                InvoiceDetails.Clear();

                foreach (var detail in details)
                {
                    InvoiceDetails.Add(detail);
                }
            }
        }

        /// <summary>
        /// Подія для сповіщення про зміну властивостей (реалізація INotifyPropertyChanged).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Викликає подію PropertyChanged для вказаної властивості.
        /// </summary>
        /// <param name="propertyName">Назва властивості.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
