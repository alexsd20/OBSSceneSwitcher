namespace OBSSceneSwitcher.OBS;

internal interface IOBSConnection
{
    bool Connect();
    bool Disconnect();
    bool SwitchToScene(string scene);
    bool StartRecording();
    bool StopRecording();
    string GetDefaultScene();
    bool SetDefaultScene(string scene);
}
