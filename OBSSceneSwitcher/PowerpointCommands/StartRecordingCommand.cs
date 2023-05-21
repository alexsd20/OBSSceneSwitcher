using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class StartRecordingCommand : IPowerPointCommand
{
    private const string StartCommandString = "**START";
    private readonly ILogger? _logger;
    private readonly IOBSConnection _obsConnection;
    public StartRecordingCommand(IOBSConnection obsConnection, ILogger<StartRecordingCommand>? logger = default)
    {
        this._obsConnection = obsConnection;
        this._logger = logger;
    }

    public string CommandString => StartCommandString;
    public void Execute(string line)
    {
        this._logger?.LogDebug("Start recording");
        this._obsConnection.StartRecording();
    }
}
