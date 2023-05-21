using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.PowerPoint;

namespace OBSSceneSwitcher.Powerpoint;

internal class PowerpointSlideNote : IPowerpointSlideNote
{
    private readonly ILogger? _logger;

    public PowerpointSlideNote(ILogger<PowerpointSlideNote>? logger = default)
    {
        this._logger = logger;
    }

    public string GetSlideNote(SlideShowWindow window)
    {
        if (window is null)
        {
            _logger?.LogWarning("window is null");
            return string.Empty;
        }

        var slide = window.View?.Slide;
        if (slide is null)
        {
            _logger?.LogWarning("slide is null");
            return string.Empty;
        }

        _logger?.LogInformation("Moved to Slide Number {SlideNumber}", slide.SlideNumber);

        return GetSlideNoteText(slide);
    }

    private static string GetSlideNoteText(Slide? slide)
    {
        try
        {
            return slide?.NotesPage?.Shapes[2]?.TextFrame?.TextRange?.Text ?? string.Empty;
        }
        catch
        {
        }

        return string.Empty;
    }

}
