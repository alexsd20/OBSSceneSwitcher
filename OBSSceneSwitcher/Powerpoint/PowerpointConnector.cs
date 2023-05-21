using Microsoft.Extensions.Hosting;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Extensions.Logging;

namespace OBSSceneSwitcher.Powerpoint;

internal sealed class PowerpointConnector : IHostedService
{
    private readonly IPowerpointNoteHandler _powerpointNoteProcessor;
    private readonly IPowerpointSlideNote _powerpointSlideNote;
    private readonly ILogger? _logger;
    private readonly EApplication_Event _eApplication;

    public PowerpointConnector(
        EApplication_Event eApplication,
        IPowerpointNoteHandler powerpointNoteProcessor,
        IPowerpointSlideNote powerpointSlideNote,
        ILogger<PowerpointConnector>? logger = default)
    {
        this._eApplication = eApplication;
        this._powerpointNoteProcessor = powerpointNoteProcessor;
        this._logger = logger;
        this._powerpointSlideNote = powerpointSlideNote;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Connecting to PowerPoint...");

        _eApplication.SlideShowNextSlide += App_SlideShowNextSlide;

        _logger?.LogInformation("Connected to PowerPoint");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _eApplication.SlideShowNextSlide -= App_SlideShowNextSlide;
        return Task.CompletedTask;
    }

    private void App_SlideShowNextSlide(SlideShowWindow window)
    {
        if (window is null)
        {
            _logger?.LogWarning("Window is null");
            return;
        }

        string note = _powerpointSlideNote.GetSlideNote(window);

        _logger?.LogDebug("Text: '{note}'", note);

        _powerpointNoteProcessor.HandleNote(note);
    }
}
