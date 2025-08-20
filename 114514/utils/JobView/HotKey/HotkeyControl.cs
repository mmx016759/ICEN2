
namespace icen.utils.JobView.HotKey;

public class HotkeyControl
{
    internal readonly string Name;
    internal IHotkeyResolver Slot;
    internal string ToolTip = "";

    internal HotkeyControl(string name) => Name = name;
}