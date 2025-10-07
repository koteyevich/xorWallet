using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services;
using xorWallet.Services.Interfaces;
using xorWallet.Settings;

namespace xorWallet;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(void (hostContext, services) =>
            {
                ConfigureServices(services, hostContext.Configuration);
            })
            .Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() => logger.LogInformation("Application started"));
        lifetime.ApplicationStopping.Register(() => logger.LogInformation("Application stopping"));
        lifetime.ApplicationStopped.Register(() => logger.LogInformation("Application stopped"));

        await host.StartAsync();

        await host.WaitForShutdownAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();

            services.Configure<SentrySettings>(configuration.GetSection("Sentry"));
            builder.AddSentry(options =>
            {
                var sentrySettings = configuration.GetRequiredSection("Sentry").Get<SentrySettings>();
                options.Dsn = sentrySettings.Dsn;
                options.Environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                options.MinimumBreadcrumbLevel = sentrySettings.MinimumBreadcrumbLevel;
                options.MaxBreadcrumbs = sentrySettings.MaxBreadcrumbs;
                options.MinimumEventLevel = sentrySettings.MinimumEventLevel;
            });
        });

        #region Database

        var dbSettings = configuration.GetRequiredSection(nameof(DBSettings)).Get<DBSettings>();
        services.Configure<DBSettings>(configuration.GetSection(nameof(DBSettings)));
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMongoDB(dbSettings?.AtlasURI, dbSettings?.DatabaseName);
        });
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICheckService, CheckService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        #endregion

        #region Bot

        services.AddSingleton<IBotInfo, BotInfo>();

        var botSettings = configuration.GetRequiredSection("Bot").Get<BotSettings>();
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botSettings?.Token ?? ""));
        services.AddHostedService<BotService>();

        SetupTypes(services);

        services.AddScoped<ICommandRegistryService, CommandRegistryService>();
        services.AddScoped<ICallbackRegistryService, CallbackRegistryService>();
        services.AddScoped<StartUrlGenerator>();
        services.AddScoped<Get>();

        #endregion
    }

    /// <summary>
    /// This is a method that scans the assembly for commands, callbacks. etc. and adds them to DI automatically.
    /// This means that add your new functionality is hands-free so you can just focus on the developing.
    /// </summary>
    private static void SetupTypes(IServiceCollection services)
    {
        var commands = typeof(Program).Assembly.GetTypes()
            .Where(t =>
                typeof(ICommand).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
        foreach (var type in commands)
        {
            Console.WriteLine("Adding command: " + type.Name);
            services.AddTransient(typeof(ICommand), type);
        }

        var callbacks = typeof(Program).Assembly.GetTypes()
            .Where(t =>
                typeof(ICallback).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
        foreach (var type in callbacks)
        {
            Console.WriteLine("Adding callback: " + type.Name);
            services.AddTransient(typeof(ICallback), type);
        }

        var startFunctions = typeof(Program).Assembly.GetTypes()
            .Where(t =>
                typeof(IStartFunction).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
        foreach (var type in startFunctions)
        {
            Console.WriteLine("Adding start function: " + type.Name);
            services.AddTransient(typeof(IStartFunction), type);
        }
    }
}