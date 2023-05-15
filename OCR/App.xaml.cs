using Microsoft.Extensions.DependencyInjection;
using OCR.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private void Initialize()
        {
            // Check Process Duplication
            Process[] processArray = Process.GetProcessesByName("OCR");
            if (processArray.Length > 1)
            {
                Application.Current.Shutdown();
                return;
            }
        }

            private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }

        public App()
        {           
            Services = ConfigureServices();
            Initialize();
            this.InitializeComponent();

            
        }
    }
}
