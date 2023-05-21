using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class StopRecordingCommand : IPowerPointCommand
{
    private const string StartCommandString = "**STOP";
    private readonly ILogger? _logger;
    private readonly IOBSConnection _obsConnection;
    public StopRecordingCommand(IOBSConnection obsConnection, ILogger<StopRecordingCommand>? logger = default)
    {
        this._obsConnection = obsConnection;
        this._logger = logger;
    }

    public string CommandString => StartCommandString;
    public void Execute(string line)
    {
        this._logger?.LogDebug("Stop recording");
        this._obsConnection.StopRecording();
    }
}
