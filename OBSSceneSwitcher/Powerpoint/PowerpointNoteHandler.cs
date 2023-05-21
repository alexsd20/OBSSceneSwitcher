using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OBSSceneSwitcher.OBS;
using OBSSceneSwitcher.PowerpointCommands;

namespace OBSSceneSwitcher.Powerpoint;

internal class PowerpointNoteHandler : IPowerpointNoteHandler
{
    private readonly IPowerPointCommandsProvider _commandsProvider;
    private readonly ILogger? _logger;
    private readonly List<IPowerpointPreNoteHandler> preNoteHandlers;
    private readonly List<IPowerpointPostNoteHandler> postNoteHandlers;

    public PowerpointNoteHandler(
        IServiceProvider serviceProvider,
        IPowerPointCommandsProvider commandsProvider,
        ILogger<PowerpointNoteHandler>? logger = default)
    {
        this._commandsProvider = commandsProvider;
        this._logger = logger;

        this.preNoteHandlers = serviceProvider.GetServices<IPowerpointPreNoteHandler>().ToList();
        this.postNoteHandlers = serviceProvider.GetServices<IPowerpointPostNoteHandler>().ToList();
    }

    public void HandleNote(string note)
    {
        PrehandleNote(note);
        HandleNoteInternal(note);
        PosthandleNote(note);
    }

    private void HandleNoteInternal(string note)
    {
        var notereader = new StringReader(note);
        string? line;
        while ((line = notereader.ReadLine()) is not null)
        {
            if (string.IsNullOrEmpty(line))
            {
                _logger?.LogDebug("Empty line");
                continue;
            }

            _logger?.LogTrace("Line '{line}'", line);

            var commands = _commandsProvider.GetCommands(line).ToList();
            if (!commands.Any())
            {
                _logger?.LogDebug("No command found in '{line}'", line);
                continue;
            }

            foreach (var command in commands)
            {
                _logger?.LogDebug("Found '{CommandString}' command ", command.CommandString);
                ExecuteCommand(command, line);
            }
        }
    }

    private void PosthandleNote(string note)
    {
        _logger?.LogTrace("Posthandle note");
        foreach (var postNoteHandler in postNoteHandlers)
        {
            try
            {
                postNoteHandler.PostHandleNote(note);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in post note handler");
            }
        }
    }

    private void PrehandleNote(string note)
    {
        _logger?.LogTrace("Prehandle note");
        foreach (var preNoteHandler in preNoteHandlers)
        {
            try
            {
                preNoteHandler.PreHandleNote(note);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in pre note handler");
            }
        }
    }

    private void ExecuteCommand(IPowerPointCommand command, string line)
    {
        try
        {
            _logger?.LogTrace("Executing command '{CommandString}'", command.CommandString);
            command.Execute(line);
            _logger?.LogTrace("Executed command '{CommandString}'", command.CommandString);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing command '{CommandString}' in '{line}'", command.CommandString, line);
        }
    }
}
