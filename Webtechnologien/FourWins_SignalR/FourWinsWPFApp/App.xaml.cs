//-----------------------------------------------------------------------
// <copyright file="LoginVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Tamara Mayer, Dawid Styczynski</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Windows;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Vms;
    using FourWinsWPFApp.Services;
    using FourWinsWPFApp.Models;
    using FourWinsWPFApp.EventArguments;
    using SharedData.SharedHubData.Interfaces;
    using SharedData.SharedHubData.ConcreteTypes;
    using SharedData.LobbyData;
    using FourWinsWPFApp.VMs;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddConsole();
                });

                services.AddTransient<IMessageFormatConverterService<string>, JsonConverterService>();
                services.AddTransient<IClientFactoryService<SignalRClient>, ClientFactoryService>();

                services.AddSingleton<IObjectMapService<string, Challenge>, GenericMappingService<string, Challenge>>();
                services.AddSingleton<ILoginVM<ErrorOccurredEventArgs>, LoginVM>(); 
                services.AddSingleton<ILobbyVM, LobbyVM>();
                services.AddSingleton<IActiveGamesVM, ActiveGamesVM>();
                services.AddSingleton<Lobby>();

               // services.AddHostedService<MainWindow>();
            });
        }

        /// <summary>
        /// Handles the startup event and creates a DI container.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleStartup(object source, StartupEventArgs e)
        {
            var host = this.CreateHostBuilder().Build();
            var loginViewModel = host.Services.GetRequiredService<ILoginVM<ErrorOccurredEventArgs>>();

            ILogger<ApplicationVM> consoleLogger = host.Services.GetRequiredService<ILogger<ApplicationVM>>();

            new ApplicationVM(loginViewModel, consoleLogger);
        }
    }
}
