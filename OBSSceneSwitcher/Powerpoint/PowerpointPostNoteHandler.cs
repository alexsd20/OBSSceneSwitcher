using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;
using OBSSceneSwitcher.PowerpointCommands;

namespace OBSSceneSwitcher.Powerpoint;

internal sealed class PowerpointPostNoteHandler : IPowerpointPostNoteHandler
{
    private readonly IPowerPointSwitchSceneCommand _switchSceneCommand;
    private readonly ILogger? _logger;
    private readonly IOBSState _obsState;

    public PowerpointPostNoteHandler(IPowerPointSwitchSceneCommand switchSceneCommand, IOBSState obsState, ILogger<PowerpointPostNoteHandler>? _logger = default)
    {
        this._switchSceneCommand = switchSceneCommand;
        this._obsState = obsState;
        this._logger = _logger;
    }

    public void PostHandleNote(string note)
    {
        if (!_obsState.IsSceneHandled && !_obsState.Parameters.DoNotSwitchToDefaultScene)
        {
            _logger?.LogInformation("Switching to OBS default scene '{DefaultSceneName}'", _obsState.DefaultScene);
            _switchSceneCommand.SwitchScene(_obsState.DefaultScene);
        }
    }
}
