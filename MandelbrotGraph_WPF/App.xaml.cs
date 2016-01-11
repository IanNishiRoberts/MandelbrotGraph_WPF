using System.Windows;
using MandelbrotGraph_WPF.ViewModel;

namespace MandelbrotGraph_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();

            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            
            window.Show();
        }
    }
}
