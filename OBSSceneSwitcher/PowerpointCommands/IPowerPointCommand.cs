namespace OBSSceneSwitcher.PowerpointCommands
{
    internal interface IPowerPointCommand
    {
        internal string CommandString { get; }
        internal void Execute(string line);
    }
}