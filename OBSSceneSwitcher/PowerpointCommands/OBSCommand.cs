using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class OBSCommand : IPowerPointSwitchSceneCommand
{
    private const string OBSCommandString = "OBS:";
    private readonly ILogger? _logger;
    private readonly IOBSConnection _obsConnection;
    private readonly IOBSState _obsState;

    public OBSCommand(IOBSConnection obsConnection, IOBSState obsState, ILogger<OBSDefCommand>? logger = default)
    {
        this._obsConnection = obsConnection;
        this._obsState = obsState;
        this._logger = logger;
    }

    public string CommandString => OBSCommandString;
    public void Execute(string line)
    {
        if (_obsState.IsSceneHandled)
        {
            _logger?.LogWarning("Multiple scene definitions found. Line '{line}' ignored", line);
            return;
        }

        string scene = line[OBSCommandString.Length..].Trim();
        if (string.IsNullOrEmpty(scene))
        {
            this._logger?.LogWarning("No scene found in line '{line}'", line);
            return;
        }

        SwitchScene(scene);
    }

    public bool SwitchScene(string scene)
    {
        if (string.IsNullOrEmpty(scene))
        {
            this._logger?.LogWarning("No scene specified");
            return false;
        }

        this._logger?.LogDebug("Switching to scene {scene}", scene);
        _obsState.IsSceneHandled = _obsConnection.SwitchToScene(scene);

        if (_obsState.IsSceneHandled)
        {
            this._logger?.LogDebug("Scene handled");
        }

        return _obsState.IsSceneHandled;
    }

    public bool SwitchDefaultScene()
    {
        this._logger?.LogDebug("Switching to default scene");
        return SwitchScene(this._obsConnection.GetDefaultScene());
    }

    public string GetDefaultSceneName()
    {
        return this._obsConnection.GetDefaultScene();
    }
}
