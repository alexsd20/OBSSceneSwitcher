using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OBSSceneSwitcher.OBS;

namespace OBSSceneSwitcher.PowerpointCommands;

internal sealed class SetParameterCommand : IPowerPointCommand
{
    public string CommandString => "SetParameter:";
    private readonly ILogger? _logger;
    private readonly IOBSState _obsState;

    public SetParameterCommand(IOBSState obsState, ILogger<SetParameterCommand>? _logger = default)
    {
        this._obsState = obsState;
        this._logger = _logger;
    }

    void IPowerPointCommand.Execute(string line)
    {
        string parameterLine = line[CommandString.Length..].Trim();
        if (string.IsNullOrEmpty(parameterLine))
        {
            _logger?.LogWarning("Invalid parameter line '{parameterLine}'", parameterLine);
            return;
        }

        string[] parameterParts = parameterLine.Split('=');

        if (parameterParts.Length != 2)
        {
            _logger?.LogWarning("Invalid parameter line '{parameterLine}'", parameterLine);
            return;
        }

        string parameterName = parameterParts[0].Trim();
        string parameterValue = parameterParts[1].Trim();

        _logger?.LogInformation("Set {parameterName} to '{parameterValue}'", parameterName, parameterValue);

        if (string.Equals(parameterName, nameof(IOBSSceneSwitcherParameters.DoNotSwitchToDefaultScene), StringComparison.OrdinalIgnoreCase))
        {
            if (!bool.TryParse(parameterValue, out bool value))
            {
                _logger?.LogWarning("Wrong parameter value '{parameterValue}'", parameterValue);
                return;
            }

            _obsState.Parameters.DoNotSwitchToDefaultScene = value;
        }
        else if (string.Equals(parameterName, nameof(IOBSSceneSwitcherParameters.OBSUrl), StringComparison.OrdinalIgnoreCase))
        {
            _obsState.Parameters.OBSUrl = parameterValue;
        }
        else if (string.Equals(parameterName, nameof(IOBSSceneSwitcherParameters.Password), StringComparison.OrdinalIgnoreCase))
        {
            _obsState.Parameters.Password = parameterValue;
        }
        else
        {
            _logger?.LogWarning("Unknown parameter '{parameterName}'", parameterName);
        }
    }
}
