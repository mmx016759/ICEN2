using ICEN2.白魔.界面.Hotkey;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.界面.TAB;
using ICEN2.白魔.设置.标签;
using ICEN2.白魔.设置.设置;
using JobViewWindow = ICEN2.utils.JobView.JobViewWindow;


namespace ICEN2.白魔.界面;

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