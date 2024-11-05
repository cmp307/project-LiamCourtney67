using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.ViewModels;
using ScottishGlenAssetTracking.Views.Asset;
using ScottishGlenAssetTracking.Views.Employee;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Configuration for the application.
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Host for the application.
        /// </summary>
        public static IHost AppHost { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Initialize the configuration for the application.
            InitializeConfiguration();

            this.InitializeComponent();

            // Create the host for the application.
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => { ConfigureServices(services); } )
                .Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Start the application host.
            await AppHost.StartAsync();

            // Warm up the database.
            WarmUpDatabase();

            // Inject the main window and set the title.
            m_window = AppHost.Services.GetRequiredService<MainWindow>();
            m_window.Title = "Scottish Glen";

            // Maximize the window
            if (m_window.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }

            m_window.Activate();
        }

        /// <summary>
        /// Initializes the configuration for the application.
        /// </summary>
        private void InitializeConfiguration()
        {
            // Load the configuration from the appsettings.json file.
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Get the connection string for the database.
            var connectionString = Configuration.GetConnectionString("ScottishGlenDatabase");

            // Add the database context to the services.
            services.AddDbContext<ScottishGlenContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Add services to the services collection.
            services.AddSingleton<AssetService>();
            services.AddSingleton<EmployeeService>();
            services.AddSingleton<DepartmentService>();

            // Add view models to the services collection.
            services.AddTransient<AddAssetViewModel>();
            services.AddTransient<ViewAssetViewModel>();
            services.AddTransient<AddEmployeeViewModel>();
            services.AddTransient<ViewEmployeeViewModel>();

            // Add views to the services collection.
            services.AddTransient<AddAsset>();
            services.AddTransient<ViewAsset>();
            services.AddTransient<AddEmployee>();
            services.AddTransient<ViewEmployee>();

            // Add the main window to the services collection.
            services.AddSingleton<MainWindow>();
        }

        /// <summary>
        /// Warms up the database by executing a query.
        /// </summary>
        private void WarmUpDatabase()
        {
            // Create a scope and get the database context.
            using (var scope = AppHost.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ScottishGlenContext>();

                try
                {
                    // Execute a query to warm up the database.
                    context.Departments.FirstOrDefault();
                }
                catch (Exception)
                {
                    // Ignore any exception, only need to warm up the database.
                }
            }
        }

        private Window m_window;
    }
}
