namespace OBSSceneSwitcher.PowerpointCommands;

internal interface IPowerPointCommandsProvider
{
    IEnumerable<IPowerPointCommand> GetCommands(string line);
}
