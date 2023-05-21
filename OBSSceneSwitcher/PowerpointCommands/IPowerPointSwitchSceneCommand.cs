namespace OBSSceneSwitcher.PowerpointCommands;

internal interface IPowerPointSwitchSceneCommand : IPowerPointCommand
{
    bool SwitchScene(string scene);
    bool SwitchDefaultScene();
    string GetDefaultSceneName();
}
