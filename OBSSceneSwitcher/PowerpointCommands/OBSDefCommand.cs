using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class OBSDefCommand : IPowerPointCommand
{
    private const string OBSDefCommandString = "OBSDEF:";
    private readonly ILogger? _logger;
    private readonly IOBSConnection _obsConnection;
    public OBSDefCommand(IOBSConnection obsConnection, ILogger<OBSDefCommand>? logger = default)
    {
        this._obsConnection = obsConnection;
        this._logger = logger;
    }

    public string CommandString => OBSDefCommandString;
    public void Execute(string line)
    {
        string scene = line[OBSDefCommandString.Length..];
        if (string.IsNullOrEmpty(scene))
        {
            _logger?.LogWarning("No scene found in line {line}", line);
            return;
        }

        scene = scene.Trim();
        _logger?.LogDebug("Set default scene to {scene}", scene);
        _obsConnection.SetDefaultScene(scene);
    }
}
