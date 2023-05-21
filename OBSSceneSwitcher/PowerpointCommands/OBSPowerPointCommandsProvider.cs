using Microsoft.Extensions.Logging;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class OBSPowerPointCommandsProvider : IPowerPointCommandsProvider
{
    private readonly List<IPowerPointCommand> commands;
    private readonly ILogger? logger;

    public OBSPowerPointCommandsProvider(IEnumerable<IPowerPointCommand> commands, ILogger<OBSPowerPointCommandsProvider>? logger = default)
    {
        this.logger = logger;
        this.commands = commands.ToList();
        this.logger?.LogDebug("Found {count} commands", this.commands.Count);
    }

    public IEnumerable<IPowerPointCommand> GetCommands(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            this.logger?.LogDebug("GetCommand: Empty line");
            yield break;
        }

        foreach (var command in commands)
        {
            if (line.StartsWith(command.CommandString, StringComparison.OrdinalIgnoreCase))
            {
                this.logger?.LogDebug("GetCommand: return '{CommandString}'", command.CommandString);
                yield return command;
            }
        }
    }
}
