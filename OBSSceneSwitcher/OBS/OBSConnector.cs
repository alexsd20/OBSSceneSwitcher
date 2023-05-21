using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OBSSceneSwitcher.OBS;

internal class OBSConnector : IHostedService
{
    private readonly IOBSConnection _connection;
    private readonly ILogger? _logger;

    public OBSConnector(IOBSConnection connection, ILogger<OBSConnector>? logger = default)
    {
        this._connection = connection;
        this._logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _connection.Connect();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _connection.Disconnect();

        return Task.CompletedTask;
    }
}
