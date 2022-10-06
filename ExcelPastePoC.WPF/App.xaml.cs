using ExcelPastePoC.ViewModels;
using System.Windows;

namespace ExcelPastePoC.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            App.Current.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            App.Current.MainWindow.Show();
        }
    }
}
