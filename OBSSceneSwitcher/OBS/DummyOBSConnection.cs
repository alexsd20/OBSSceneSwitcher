using Microsoft.Extensions.Logging;

namespace OBSSceneSwitcher.OBS;

internal sealed class DummyOBSConnection : IOBSConnection, IOBSState
{
    private readonly ILogger? _logger;
    private readonly IOBSSceneSwitcherParameters _parameters;

    public DummyOBSConnection(IOBSSceneSwitcherParameters parameters, ILogger<DummyOBSConnection>? logger = default)
    {
        this._parameters = parameters;
        this._logger = logger;
    }

    public string DefaultScene { get; set; } = string.Empty;

    public bool IsRecording { get; set; }

    public bool IsConnected { get; set; }

    public IEnumerable<string> Scenes => throw new NotImplementedException();

    public bool IsSceneHandled { get; set; }

    public IOBSSceneSwitcherParameters Parameters => _parameters;

    public bool Connect()
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(Connect)}()");
        return true;
    }

    public bool Disconnect()
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(Disconnect)}()");
        return true;
    }

    public string GetDefaultScene()
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(GetDefaultScene)}()");
        return this.DefaultScene;
    }

    public bool SetDefaultScene(string scene)
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(SetDefaultScene)}()");
        this.DefaultScene = scene;
        return true;
    }

    public bool StartRecording()
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(StartRecording)}()");
        this.IsRecording = true;
        return true;
    }

    public bool StopRecording()
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(StopRecording)}()");
        this.IsRecording = false;
        return true;
    }

    public bool SwitchToScene(string scene)
    {
        _logger?.LogDebug($"{nameof(DummyOBSConnection)}.{nameof(SwitchToScene)}()");
        return true;
    }
}
