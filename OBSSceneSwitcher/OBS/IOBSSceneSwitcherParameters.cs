namespace OBSSceneSwitcher.OBS;

internal interface IOBSSceneSwitcherParameters
{
    bool DoNotSwitchToDefaultScene { get; set; }
    string OBSUrl { get;set; }
    string Password { get; set; }
    int ConnectionTimeout { get; set; }
}
