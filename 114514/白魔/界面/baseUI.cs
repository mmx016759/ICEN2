using icen.白魔.Utilities;
using icen.白魔.Utilities.标签;
using icen.白魔.Utilities.设置;
using icen.白魔.View.Hotkey;
using icen.白魔.View.QT;
using icen.白魔.View.TAB;
using JobViewWindow = icen.utils.JobView.JobViewWindow;


namespace icen.白魔.View;

public static class baseUI
{
    public static JobViewWindow UI { get; private set; }
    public static JobViewWindow QT { get; private set; }
    
    public static void Build()
    {
        QT = UI = new JobViewWindow(默认值.实例.JobViewSave, 默认值.实例.Save, "ICEN WhiteMage",ref 默认值.实例.HotKey设置, 白魔Hotkey技能设置.List);
        UI.AddTab("通用", 设置标签页.DrawGeneral);
        UI.AddTab("时间轴", TimeLine.DrawTimeLine);
        UI.AddTab("Dev", Dev.DrawDev);
        白魔Qt.CreateQt();
        UI.CreateHotKey();
        UI.DrawQtWindow();
    }
}