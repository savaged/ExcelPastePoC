using CommunityToolkit.Mvvm.ComponentModel;

namespace ExcelPastePoC.Models
{
    public class DataModel : ObservableObject
    {
        private decimal _field1;
        private decimal _field2;
        private decimal _field3;
        private decimal _field4;
        private decimal _field5;

        public decimal Field1
        {
            get => _field1;
            set => SetProperty(ref _field1, value);
        }

        public decimal Field2
        {
            get => _field2;
            set => SetProperty(ref _field2, value);
        }

        public decimal Field3
        {
            get => _field3;
            set => SetProperty(ref _field3, value);
        }

        public decimal Field4
        {
            get => _field4;
            set => SetProperty(ref _field4, value);
        }

        public decimal Field5
        {
            get => _field5;
            set => SetProperty(ref _field5, value);
        }
    }
}
