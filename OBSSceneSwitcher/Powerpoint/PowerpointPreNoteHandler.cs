using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.Powerpoint;

internal sealed class PowerpointPreNoteHandler : IPowerpointPreNoteHandler
{
    private readonly ILogger? _logger;
    private readonly IOBSState _obsState;

    public PowerpointPreNoteHandler(IOBSState obsState, ILogger<PowerpointPreNoteHandler>? _logger = default)
    {
        this._obsState = obsState;
        this._logger = _logger;
    }

    public void PreHandleNote(string note)
    {
        _logger?.LogDebug("Resetting scene handled state");
        _obsState.IsSceneHandled = false;
    }
}
