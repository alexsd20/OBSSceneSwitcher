using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Office.Interop.PowerPoint;
using OBSSceneSwitcher.OBS;
using OBSSceneSwitcher.Powerpoint;
using OBSSceneSwitcher.PowerpointCommands;
using OBSWebsocketDotNet;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("The OBS scene switcher");

        using IHost host = CreateHostBuilder(args).Build();
        //using IServiceScope scope = host.Services.CreateScope();
        //IServiceProvider services = scope.ServiceProvider;

        await host.StartAsync();
        await host.WaitForShutdownAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<PowerpointConnector>();
                services.AddHostedService<OBSConnector>();

                services.AddSingleton<IPowerPointCommandsProvider, OBSPowerPointCommandsProvider>();
                //services.AddSingleton<IOBSConnection, DummyOBSConnection>();
                services.AddSingleton<OBSConnection>();
                services.AddSingleton<IOBSConnection>(x => x.GetRequiredService<OBSConnection>());
                services.AddSingleton<IOBSState>(x => x.GetRequiredService<OBSConnection>());

                services.AddSingleton<IOBSWebsocket, OBSWebsocket>();
                services.AddSingleton<IOBSSceneSwitcherParameters>(new OBSSceneSwitcherParameters()
                {
                    OBSUrl = "ws://127.0.0.1:4455",
                    Password = "",
                });

                services.AddSingleton<EApplication_Event>(new Application());
                services.AddTransient<IPowerPointCommand, OBSCommand>();
                services.AddTransient<IPowerPointSwitchSceneCommand, OBSCommand>();
                services.AddTransient<IPowerPointCommand, OBSDefCommand>();
                services.AddTransient<IPowerPointCommand, StartRecordingCommand>();
                services.AddTransient<IPowerPointCommand, StopRecordingCommand>();
                services.AddTransient<IPowerPointCommand, SetParameterCommand>();
                services.AddTransient<IPowerpointNoteHandler, PowerpointNoteHandler>();
                services.AddTransient<IPowerpointSlideNote, PowerpointSlideNote>();
                services.AddTransient<IPowerpointPreNoteHandler, PowerpointPreNoteHandler>();
                services.AddTransient<IPowerpointPostNoteHandler, PowerpointPostNoteHandler>();
            })
        .ConfigureLogging(options =>
        {
            options.AddConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build().GetSection("Logging"));

            options.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "[HH:mm:ss] ";
            });

            options.SetMinimumLevel(LogLevel.Trace);
        });
}