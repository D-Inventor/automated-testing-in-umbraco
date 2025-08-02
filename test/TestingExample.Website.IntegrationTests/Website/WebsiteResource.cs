using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestingExample.Website.IntegrationTests.Database;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.SyncHandlers.Models;

namespace TestingExample.Website.IntegrationTests.Website;

public class WebsiteResource(IDatabaseResource databaseResource)
        : WebApplicationFactory<Program>
{
    private readonly SemaphoreSlim _startSemaphore = new(1, 1);
    private readonly IDatabaseResource _databaseResource = databaseResource;
    private bool _started;

    public async Task StartAsync()
    {
        /* Websites are lazy loaded, so we need to prod the website to make it boot.
         * The wait handle is created before boot, so that we know for certain that boot hasn't finished before we start waiting (VERY IMPORTANT!!)
         * 
         * Sending a request to the server is the trigger that makes the website boot.
         * The wait handle will be triggered as soon as index rebuilding is complete. At that point, we know for sure that the website is completely ready.
         * 
         * Only one request can perform the start at a time, so we have to use the double-if pattern in combination with a semaphore to ensure that startup is really only called once.
         */
        if (!_started)
        {
            await _startSemaphore.WaitAsync();
            try
            {
                if (!_started)
                {
                    // The waitHandle task completes when the index rebuild is done.
                    ExamineWaitContext waitContext = Services.GetRequiredService<ExamineWaitContext>();
                    Task waitHandle = waitContext.SetAsync();

                    await CreateClient().GetAsync("/");
                    await waitHandle;

                    // After starting the website, we use uSync to reconstruct a full website.
                    await ImportContentAsync();
                    _started = true;
                }
            }
            finally
            {
                _startSemaphore.Release();
            }
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        string projectDir = Directory.GetCurrentDirectory();
        string configPath = Path.Combine(projectDir, "integration.settings.json");
        builder.ConfigureAppConfiguration(conf =>
        {
            var connectionString = _databaseResource.GetConnectionString();
            conf.AddJsonFile(configPath, optional: false);
            conf.AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:umbracoDbDSN", connectionString.ConnectionString),
                new KeyValuePair<string, string?>("ConnectionStrings:umbracoDbDSN_ProviderName", connectionString.Provider)
            ]);
        });

        builder.ConfigureServices(ConfigureServices);
        builder.UseSolutionRelativeContentRoot(Path.Join("src", typeof(Program).Assembly.GetName().Name!));
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // In order to force consistent behaviour with indexes, we add a type that prevents the rebuild from running on a background thread.
        // Additionally, the "ExamineWaitContext" allows us to receive a signal when the index rebuild is complete.
        services.Decorate<IIndexRebuilder, DecoratorIndexRebuilderNotifier>();
        services.AddSingleton<ExamineWaitContext>();

        /* The umbraco rebuild on startup handler gets removed, because the static fields inside of it break tests when running multiple at once.
         *   If examine related tests start breaking, check if the implementation of this type has changed.
         * The service is replaced with a custom notification handler which uses a singleton class instead of static fields
         */
        ServiceDescriptor sd = services.First(s => s.ImplementationType == typeof(RebuildOnStartupHandler));
        services.Remove(sd);
        services.Add(new UniqueServiceDescriptor(typeof(INotificationHandler<UmbracoRequestBeginNotification>), typeof(CustomRebuildOnStartupHandler), ServiceLifetime.Transient));
        services.AddSingleton<CustomRebuildOnStartupHandlerState>();

        // Add a configuration to move the examine index files into RAM.
        // Now we don't rely on the filesystem while running tests
        services.ConfigureOptions<ExamineInMemoryConfiguration>();
        
        // Supposedly, scheduled publishing does curses with locks which cause flaky tests with Sqlite.
        // This component disables scheduled publishing
        services.AddHostedService<SuspendScheduledPublishingHostedService>();
    }

    private Task<IEnumerable<uSyncAction>> ImportContentAsync()
    {
        var uSyncService = Services.GetRequiredService<ISyncService>();
        var uSyncConfig = Services.GetRequiredService<ISyncConfigService>();
        return uSyncService.StartupImportAsync(uSyncConfig.GetFolders(), true, new SyncHandlerOptions
        {
            Group = uSyncConfig.Settings.UIEnabledGroups
        });
    }
}
