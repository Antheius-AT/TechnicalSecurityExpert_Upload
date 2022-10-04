//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer
{
    using System.Threading;
    using FourWins_SignalRServer.ClientContract;
    using FourWins_SignalRServer.Hubs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging.AzureAppServices;
    using SharedData.LobbyData;
    using SharedData.SharedHubData.ConcreteTypes;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.ConcreteServices;
    using SignalRServices.EventArgs;
    using SignalRServices.Interfaces.ServiceInterfaces.GameServices;
    using SignalRServices.Interfaces.ServiceInterfaces.LobbyServices;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;
    using SignalRServices.ServiceData;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddLogging();

            services.AddSignalR();

            services.AddTransient<IMessageFormatConverterService<string>, JsonConverterService>();
            services.AddTransient<IBackgroundTimerService, BackgroundTimerService>();

            services.AddSingleton<IGameService, GameService>();

            services.AddSingleton<IObjectMapService<string, ISignalRClient>, GenericMappingService<string, ISignalRClient>>();
            services.AddSingleton<IObjectMapService<Challenge, CancellationTokenSource>, GenericMappingService<Challenge, CancellationTokenSource>>();
            services.AddSingleton<IObjectMapService<string, GameData>, GenericMappingService<string, GameData>>();
            services.AddSingleton<IObjectMapService<string, CancellationTokenSource>, GenericMappingService<string, CancellationTokenSource>>();
            services.AddSingleton<IObjectMapService<string, SignalRGameClient>, GenericMappingService<string, SignalRGameClient>>();

            services.AddSingleton<IHubLinkService<string, GameData, ObjectRemovedEventArgs>, DefaultHubLinkService>();
            services.AddSingleton<IBackgroundQueueService, BackgroundLobbyQueueService>();

            services.Configure<AzureFileLoggerOptions>(options =>
            {
                options.FileName = "azure_diagnostics_";
                options.FileSizeLimit = 50 * 1024;
                options.RetainedFileCountLimit = 5;
            });
            services.Configure<AzureBlobLoggerOptions>(options =>
            {
                options.BlobName = "log.txt";
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The webhost environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LobbyHub>("/fwLobby/{requestedUsername}");
                endpoints.MapHub<GameHub>("/fwGame/{requestedUsername}");
            });
        }
    }
}
