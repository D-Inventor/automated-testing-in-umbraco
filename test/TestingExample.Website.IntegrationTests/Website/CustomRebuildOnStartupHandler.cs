using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.Models;

namespace TestingExample.Website.IntegrationTests.Website;

/// <summary>
/// This class replaces <see cref="RebuildOnStartupHandler"/> because that class uses some unnecessary static properties that break integration tests
/// </summary>
public sealed class CustomRebuildOnStartupHandler(
    ISyncBootStateAccessor syncBootStateAccessor,
    IIndexRebuilder backgroundIndexRebuilder,
    IRuntimeState runtimeState,
    CustomRebuildOnStartupHandlerState state)
    : INotificationAsyncHandler<UmbracoRequestBeginNotification>
{
    // This method is an async version of the original handler,
    //    using the singleton state object instead of static fields
    public async Task HandleAsync(UmbracoRequestBeginNotification notification, CancellationToken cancellationToken)
    {
        if (runtimeState.Level != RuntimeLevel.Run)
        {
            return;
        }

        if (!state._isReady)
        {
            Task? initTask = null;
            lock (state._isReadyLock)
            {
                if (!state._isReady)
                {
                    initTask = InitializeAsync();
                }
            }

            if (initTask != null)
                await initTask;
        }
    }

    private Task<Attempt<IndexRebuildResult>> InitializeAsync()
    {
        state._isReady = true;

        SyncBootState bootState = syncBootStateAccessor.GetSyncBootState();

        return backgroundIndexRebuilder.RebuildIndexesAsync(
            bootState != SyncBootState.ColdBoot,
            null,
            useBackgroundThread: false);
    }
}
