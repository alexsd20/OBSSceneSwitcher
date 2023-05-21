using Microsoft.Office.Interop.PowerPoint;

namespace OBSSceneSwitcher.Powerpoint;

internal interface IPowerpointSlideNote
{
    string GetSlideNote(SlideShowWindow window);
}
