using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExcelPastePoC.ClipboardSupport;
using ExcelPastePoC.DataConversion;
using ExcelPastePoC.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace ExcelPastePoC.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private DataModel _selectedItem;
        private readonly CsvToDataTableConverter _csvConverter;

        public MainViewModel()
        {
            _csvConverter = new CsvToDataTableConverter();
            Index = new ObservableCollection<DataModel>();
            PasteCmd = new RelayCommand(OnPaste, () => CanPaste);
            ClearCmd = new RelayCommand(OnClear, () => CanClear);
            OutputCmd = new RelayCommand(OnOutput, () => CanOutput);
        }

        public IRelayCommand PasteCmd { get; }

        public IRelayCommand ClearCmd { get; }

        public IRelayCommand OutputCmd { get; }

        public bool CanPaste => !string.IsNullOrEmpty(GetCsv());

        public bool CanClear => Index.Count > 0;

        public bool CanOutput => Index.Count > 0;

        public ObservableCollection<DataModel> Index { get; }

        public DataModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private void UpdateIndexFromDataTable(DataTable dt)
        {
            if (dt?.Rows?.Count == 0)
            {
                throw new InvalidOperationException(
                    "The data pasted is not compatible with the " +
                    "importing library. Please ensure the sheet " +
                    "copied from is in a simple form, for instance " +
                    "it should not contain hidden columns, special " +
                    "characters or line breaks. Either that or " +
                    "today we just don't like you :p");
            }
            if (dt is null)
            {
                return;
            }
            foreach (DataRow dr in dt.Rows)
            {
                var row = dr.ToModel<DataModel>();
                Index.Add(row);
            }
        }

        private string GetCsv()
        {
            return SystemClipboardFacade.GetAsCsv();
        }

        private void UpdateCanExecutes()
        {
            PasteCmd.NotifyCanExecuteChanged();
            ClearCmd.NotifyCanExecuteChanged();
            OutputCmd.NotifyCanExecuteChanged();
        }

        private void OnPaste()
        {
            var dt = _csvConverter.GetDataTableFromCsv(GetCsv());
            UpdateIndexFromDataTable(dt);
            UpdateCanExecutes();
        }

        private void OnClear()
        {
            SystemClipboardFacade.Clear();
            Index.Clear();
            UpdateCanExecutes();
        }

        private void OnOutput()
        {
            // TODO
            UpdateCanExecutes();
        }

    }
}
