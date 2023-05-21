namespace OBSSceneSwitcher.OBS;

internal sealed class OBSSceneSwitcherParameters : IOBSSceneSwitcherParameters
{
    public bool DoNotSwitchToDefaultScene { get; set; } = false;
    public string OBSUrl { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ConnectionTimeout { get; set; } = 5000;
}