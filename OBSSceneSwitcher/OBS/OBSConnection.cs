using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace OBSSceneSwitcher.OBS;

internal sealed class OBSConnection : IOBSConnection, IDisposable, IOBSState
{
    private const int ConnectionTryTimeout = 100;

    private readonly ILogger? _logger;
    private readonly IOBSWebsocket _obs;
    private GetSceneListInfo? sceneListInfo;
    private readonly Dictionary<string, HashSet<string>> scenes = new(StringComparer.OrdinalIgnoreCase);

    public string DefaultScene { get; set; } = string.Empty;
    public bool IsRecording => IsConnected && _obs.GetRecordStatus().IsRecording;
    public bool IsSceneHandled { get; set; }
    public IOBSSceneSwitcherParameters Parameters { get; init; }
    public bool IsConnected => _obs.IsConnected;

    public OBSConnection(IOBSWebsocket obs, IOBSSceneSwitcherParameters parameters, ILogger<OBSConnection>? logger = default)
    {
        this._logger = logger;
        this._obs = obs;
        this.Parameters = parameters;
    }

    public bool Connect() => Connect(false);

    public bool Connect(bool sync)
    {
        if (_obs.IsConnected)
        {
            _logger?.LogTrace("Already connected to OBS. Disconnect first.");
            Disconnect(true);
        }

        if (string.IsNullOrEmpty(Parameters.OBSUrl))
        {
            _logger?.LogError("No OBS URL configured");
            return false;
        }

        _logger?.LogInformation("Connecting to OBS at {OBSUrl}", Parameters.OBSUrl);

        _obs.ConnectAsync(Parameters.OBSUrl, Parameters.Password);

        _obs.Connected += (sender, args) =>
        {
            _logger?.LogInformation("Connected to OBS at {OBSUrl}", Parameters.OBSUrl);
            ResetScenes();
        };

        this._obs.Disconnected += (sender, args) =>
        {
            _logger?.LogInformation("Disconnected from OBS at {OBSUrl}", Parameters.OBSUrl);
            ResetScenes();
        };

        if (sync)
        {
            int tries = Parameters.ConnectionTimeout / ConnectionTryTimeout;
            while (!_obs.IsConnected && tries > 0)
            {
                Thread.Sleep(ConnectionTryTimeout);
                tries--;
            }
        }

        return _obs.IsConnected;
    }

    private void ResetScenes()
    {
        sceneListInfo = default;
        scenes.Clear();
    }
    public bool Disconnect() => Disconnect(false);

    public bool Disconnect(bool sync)
    {
        if (!_obs.IsConnected)
        {
            _logger?.LogWarning("Not connected to OBS");
            return false;
        }

        _logger?.LogInformation("Disconnecting from OBS");
        _obs.Disconnect();

        if (sync)
        {
            int tries = Parameters.ConnectionTimeout / ConnectionTryTimeout;
            while (_obs.IsConnected && tries > 0)
            {
                Thread.Sleep(ConnectionTryTimeout);
                tries--;
            }
        }

        return !_obs.IsConnected;
    }

    public string GetDefaultScene()
    {
        return DefaultScene;
    }

    public bool SetDefaultScene(string scene)
    {
        DefaultScene = GetOBSSceneName(scene) ?? string.Empty;

        if (string.IsNullOrEmpty(DefaultScene))
        {
            _logger?.LogWarning("The '{scene}' scene not found", scene);
        }

        return true;
    }

    public bool StartRecording()
    {
        _logger?.LogTrace("Start recording");
        if (!CheckOBSConnected())
        {
            return false;
        }

        _obs.StartRecord();

        return true;
    }

    public bool StopRecording()
    {
        _logger?.LogTrace("Stop recording");

        if (!CheckOBSConnected())
        {
            return false;
        }

        _obs.StopRecord();

        return true;
    }

    public bool SwitchToScene(string scene)
    {
        _logger?.LogTrace("Switching to scene {scene}", scene);

        if (!CheckOBSConnected())
        {
            return false;
        }

        string? sceneName = GetOBSSceneName(scene);
        if (sceneName is null)
        {
            return false;
        }

        _obs.SetCurrentProgramScene(sceneName);

        return true;
    }

    private string? GetOBSSceneName(string scene)
    {
        if (!CheckOBSConnected())
        {
            return null;
        }

        if (sceneListInfo is null)
        {
            PopulateScenes();
        }

        if (!scenes.TryGetValue(scene, out var list))
        {
            return null;
        }

        return list.Count > 1 && list.Contains(scene) ? scene : list.FirstOrDefault();
    }

    private bool CheckOBSConnected()
    {
        if (!_obs.IsConnected)
        {
            _logger?.LogWarning("Not connected to OBS. Trying to connect...");
            if (!Connect(true))
            {
                return false;
            }
        }

        return true;
    }

    private void PopulateScenes()
    {
        sceneListInfo = _obs.GetSceneList();
        scenes.Clear();

        sceneListInfo.Scenes.ForEach(scene =>
        {
            if (!scenes.TryGetValue(scene.Name, out var list))
            {
                list = new HashSet<string>();
                scenes.Add(scene.Name, list);
            }

            list.Add(scene.Name);
        });
    }

    #region IDisposable Support
    private bool disposed = false;

    public void Dispose()
    {
        Dispose(disposing: true);

        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed resources.
            ResetScenes();
        }

        if (_obs.IsConnected)
        {
            _obs.Disconnect();
        }

        // Note disposing has been done.
        disposed = true;
    }

    ~OBSConnection()
    {
        Dispose(disposing: false);
    }
    #endregion
}
