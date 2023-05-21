namespace OBSSceneSwitcher.OBS;

internal interface IOBSState
{
    string DefaultScene { get; set; }
    bool IsRecording { get; }
    bool IsConnected { get; }
    bool IsSceneHandled { get; set; }
    IOBSSceneSwitcherParameters Parameters { get; }
}
