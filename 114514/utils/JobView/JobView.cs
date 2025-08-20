using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View;
using AEAssist.Helper;
using Dalamud.Interface.Utility.Raii;
using icen.utils.JobView.HotKey;
using icen.白魔.Utilities;
using icen.白魔.Utilities.设置;
using ImGuiNET;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace icen.utils.JobView;

public class JobViewWindow : IRotationUI
{
    private Action saveSetting;
    private QtWindow qtWindow;
    private HotkeyWindow hotkeyWindow;
    private MainWindow mainWindow;
    private QtStyle style;

    private float userFontGlobalScale = 1.17f;

    // 运行状态动画相关
    private float statusAnimationTime = 0f;


    public Dictionary<string, Action<JobViewWindow>> ExternalTab = new();

    public Action? UpdateAction;

    /// <summary>
    /// 在当前职业循环插件中创建一个gui视图
    /// </summary>
    public JobViewWindow(JobViewSave jobViewSave, Action save, string name, ref Dictionary<string, HotKetSpell> Config,
        Dictionary<string, uint> Spell)
    {
        style = new QtStyle(jobViewSave);
        saveSetting = save;
        qtWindow = new QtWindow(jobViewSave, name);
        hotkeyWindow = new HotkeyWindow(jobViewSave, name + " hotkey", ref Config, Spell, save);
        mainWindow = new MainWindow(qtWindow, hotkeyWindow, ref style);
    }

    public void CreateHotKey()
    {
        hotkeyWindow.CreateHotkey();
    }

    /// <summary>
    /// 初始化主窗口风格
    /// </summary>
    public void SetMainStyle()
    {
        // this.userFontGlobalScale = ImGui.GetIO().FontGlobalScale;
        // ImGui.GetIO().FontGlobalScale = 1.17f;
        style.SetMainStyle();
        // 现代主题已在SetMainStyle内部应用，无需重复调用
    }

    /// <summary>
    /// 注销主窗口风格
    /// </summary>
    public void EndMainStyle()
    {
        style.EndMainStyle();
        // ImGui.GetIO().FontGlobalScale = this.userFontGlobalScale;
        // 现代主题已在EndMainStyle内部恢复，无需额外处理
    }

    /// <summary>
    /// 增加一栏说明
    /// </summary>
    /// <param name="tabName"></param>
    /// <param name="draw"></param>
    public void AddTab(string tabName, Action<JobViewWindow> draw)
    {
        ExternalTab.Add(tabName, draw);
    }

    /// <summary>
    /// 设置UI上的Update处理
    /// </summary>
    /// <param name="updateAction"></param>
    public void SetUpdateAction(Action updateAction)
    {
        UpdateAction = updateAction;
    }

    /// <summary>
    /// 添加新的qt控件
    /// </summary>
    /// <param name="name">qt的名称</param>
    /// <param name="qtValueDefault">qt的bool默认值</param>
    public void AddQt(string name, bool qtValueDefault)
    {
        qtWindow.AddQt(name, qtValueDefault);
    }


    /// <summary>
    /// 添加新的qt控件，并且自定义方法
    /// </summary>
    /// <param name="name">qt的名称</param>
    /// <param name="qtValueDefault">qt的bool默认值</param>
    /// <param name="action">按下时触发的方法</param>
    public void AddQt(string name, bool qtValueDefault, Action<bool> action)
    {
        qtWindow.AddQt(name, qtValueDefault, action);
    }

    public void AddQt(string name, bool qtValueDefault, string toolTip)
    {
        qtWindow.AddQt(name, qtValueDefault, toolTip);
    }

    public void AddQt(string name, bool qtValueDefault, Action<bool> action, Vector4 color)
    {
        qtWindow.AddQt(name, qtValueDefault, action, color);
    }

    public void RemoveAllQt()
    {
        qtWindow.RemoveAllQt();
    }

    // 设置每行按钮个数
    public void SetLineCount(int count)
    {
        if (count < 1)
            count = 1;
        qtWindow.QtLineCount = count;
    }


    /// 设置上一次add添加的hotkey的toolTip
    public void SetQtToolTip(string toolTip)
    {
        qtWindow.SetQtToolTip(toolTip);
    }

    /// 画一个新的Qt窗口
    public void DrawQtWindow()
    {
        try
        {
            // 使用现代化Qt窗口
            ModernQtWindow.DrawModernQtWindow(qtWindow, style, qtWindow.save);
        }
        catch (Exception e)
        {
            LogHelper.Error($"err: -- {e}");
        }
    }

    /// 创建一个更改qt排序显示等设置的视图
    public void QtSettingView()
    {
        qtWindow.QtSettingView();
    }

    /// 获取指定名称qt的bool值
    public bool GetQt(string qtName)
    {
        return qtWindow.GetQt(qtName);
    }

    /// 设置指定qt的值
    public void SetQt(string qtName, bool qtValue)
    {
        qtWindow.SetQt(qtName, qtValue);
    }

    /// 反转指定qt的值
    /// <returns>成功返回true，否则返回false</returns>
    public bool ReverseQt(string qtName)
    {
        return qtWindow.ReverseQt(qtName);
    }

    /// 重置所有qt为默认值
    public void Reset()
    {
        qtWindow.Reset();
    }

    /// 给指定qt设置新的默认值
    public void NewDefault(string qtName, bool newDefault)
    {
        qtWindow.NewDefault(qtName, newDefault);
    }

    /// 将当前所有Qt状态记录为新的默认值
    public void SetDefaultFromNow()
    {
        qtWindow.SetDefaultFromNow();
    }

    /// 返回包含当前所有qt名字的数组 不要在update里调用
    public string[] GetQtArray()
    {
        return qtWindow.GetQtArray();
    }

    /// 画一个新的hotkey窗口
    public void DrawHotkeyWindow()
    {
        // 使用现代化Hotkey窗口
        hotkeyWindow.DrawHotkeyWindow(style);
        //ModernHotkeyWindow.DrawModernHotkeyWindow(hotkeyWindow, style);
    }

    /// <summary>
    /// 添加新的qt控件
    /// </summary>
    public void AddHotkey(string name, IHotkeyResolver slot)
    {
        hotkeyWindow.AddHotkey(name, slot);
    }

    /// <summary>
    /// 获取当前激活的hotkey列表
    /// </summary>
    /// <returns></returns>
    public List<string> GetActiveList() => hotkeyWindow.ActiveList;

    /// 设置上一次add添加的hotkey的toolTip
    public void SetHotkeyToolTip(string toolTip)
    {
        hotkeyWindow.SetHotkeyToolTip(toolTip);
    }

    /// 激活单个快捷键,mo无效
    public void SetHotkey(string name)
    {
        hotkeyWindow.SetHotkey(name);
    }

    /// 取消激活单个快捷键
    public void CancelHotkey(string name)
    {
        GetActiveList().Remove(name);
    }

    /// 返回包含当前所有hotkey名字的数组
    public string[] GetHotkeyArray()
    {
        return hotkeyWindow.GetHotkeyArray();
    }

    /// 用于draw一个更改hotkey排序显示等设置的视图
    public void HotkeySettingView()
    {
        hotkeyWindow.HotkeySettingView();
    }

    /// <summary>
    /// 运行键盘快捷键模块,一般放在update中
    /// </summary>
    public void RunHotkey()
    {
        hotkeyWindow.RunHotkey();
        qtWindow.RunHotkey();
    }

    public void Update()
    {
        RunHotkey();
    }


    /// <summary>
    /// 用于开关自动输出的控件组合
    /// </summary>
    public void MainControlView(ref bool buttonValue, ref bool stopButton)
    {
        mainWindow.MainControlView(ref buttonValue, ref stopButton, saveSetting);
    }

    private static string tempStyleStr = GlobalSetting.StyleStr;
    private static bool isError;


    ///风格设置控件
    public void ChangeStyleView()
    {
        // 现代主题选择
        ImGui.Text("选择主题预设:");
        ImGui.Separator();

        var themes = Enum.GetValues<ModernTheme.ThemePreset>();
        foreach (var theme in themes)
        {
            bool isSelected = style.CurrentTheme == theme;
            if (isSelected)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, style.ModernTheme.Colors.Primary);
            }

            if (ImGui.Button(theme.ToString(), new Vector2(120, 30)))
            {
                style.CurrentTheme = theme;
                saveSetting();
            }

            if (isSelected)
            {
                ImGui.PopStyleColor();
            }

            if (((int)theme + 1) % 2 != 0)
                ImGui.SameLine();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        if (ImGui.Checkbox("QT和快捷栏随主界面隐藏", ref GlobalSetting.QT快捷栏随主界面隐藏))
        {
            GlobalSetting.Instance.Save();
        }

        if (ImGui.Checkbox("关闭UI动态效果", ref GlobalSetting.Instance.关闭动效))
        {
            GlobalSetting.Instance.Save();
        }

        if (ImGui.Button("显示/隐藏QT"))
        {
            GlobalSetting.Instance.QtShow = !GlobalSetting.Instance.QtShow;
            GlobalSetting.Instance.Save();
        }

        ImGui.SameLine();
        if (ImGui.Button("显示/隐藏快捷栏"))
        {
            GlobalSetting.Instance.HotKeyShow = !GlobalSetting.Instance.HotKeyShow;
            GlobalSetting.Instance.Save();
        }
        // // 高级风格设置
        // if (ImGui.CollapsingHeader("高级风格设置"))
        // {
        //     ImGui.InputText("设置风格", ref tempStyleStr, 9999);
        //     if (ImGui.Button("保存风格 ###保存"))
        //     {
        //         try
        //         {
        //             QtStyle.MUIStyle = StyleModel.Deserialize(tempStyleStr);
        //             GlobalSetting.StyleStr = tempStyleStr;
        //             GlobalSetting.Instance.Save();
        //             isError = false;
        //         }
        //         catch (Exception e)
        //         {
        //             LogHelper.Debug(e.Message);
        //             isError = true;
        //         }
        //     }
        //
        //     if (isError)
        //     {
        //         ImGui.Text("错误的样式信息!");
        //     }
        // }


        ImGui.Dummy(new Vector2(1, 3));

        //QT按钮一行个数
        var input = qtWindow.QtLineCount;
        if (ImGui.InputInt("Qt按钮每行个数", ref input))
        {
            if (input < 1)
                qtWindow.QtLineCount = 1;
            else
                qtWindow.QtLineCount = input;
        }

        //hotkey按钮一行个数
        input = hotkeyWindow.HotkeyLineCount;
        if (ImGui.InputInt("快捷键每行个数", ref input))
        {
            if (input < 1)
                hotkeyWindow.HotkeyLineCount = 1;
            else
                hotkeyWindow.HotkeyLineCount = input;
        }

        //QT透明度
        var qtBackGroundAlpha = style.QtWindowBgAlpha;
        if (ImGui.SliderFloat("背景透明度", ref qtBackGroundAlpha, 0f, 1f, "%.1f"))
        {
            style.QtWindowBgAlpha = qtBackGroundAlpha;
        }

        var smallWindowSize = GlobalSetting.Instance.缩放后窗口大小;
        if (ImGui.InputFloat2("缩放后窗口大小", ref smallWindowSize))
        {
            GlobalSetting.Instance.缩放后窗口大小 = smallWindowSize;
            GlobalSetting.Instance.Save();
        }

        // 按钮大小
        var buttonSize = style.QtButtonSizeOrigin;
        if (ImGui.InputFloat2("按钮大小", ref buttonSize))
        {
            style.QtButtonSizeOrigin = buttonSize;
        }

        // 热键大小
        // 按钮大小
        var hotKeySize = style.HotkeySizeOrigin;
        if (ImGui.InputFloat2("热键大小", ref hotKeySize))
        {
            style.HotkeySizeOrigin = hotKeySize;
        }


        ImGui.Dummy(new Vector2(1, 3));

        var lockWindow = hotkeyWindow.LockWindow;
        if (ImGui.Checkbox("Hotkey窗口不可拖动", ref lockWindow))
        {
            hotkeyWindow.LockWindow = lockWindow;
        }

        var lockQtWindow = qtWindow.LockWindow;
        if (ImGui.Checkbox("Qt窗口不可拖动", ref lockQtWindow))
        {
            qtWindow.LockWindow = lockQtWindow;
        }


        //重置按钮
        if (ImGui.Button("重置风格 ###重置"))
        {
            style.Reset();
            isError = false;
        }
    }

    /// <summary>
    /// 强制保存所有窗口位置
    /// </summary>
    public void ForceSaveAllWindowPositions()
    {
        // 强制保存QT窗口位置
        qtWindow.ForceSaveCurrentPosition();
    }

    public bool IsCustomMain()
    {
        return true;
    }

    public void OnDrawUI()
    {
        SetMainStyle();
        try
        {
            #region 加载UI

            var triggerlineName = "";
            if (AI.Instance.TriggerlineData.CurrTriggerLine != null)
            {
                triggerlineName =
                    $"| {AI.Instance.TriggerlineData.CurrTriggerLine.Author}-{AI.Instance.TriggerlineData.CurrTriggerLine.Name}";
            }

            var title =
                string.Format("{0} | {1} {2} ###aeassist",
                    GlobalSetting.title, // 作者名
                    AI.Instance.BattleData.CurrBattleTimeInSec, // 战斗时间（秒）
                    triggerlineName // {时间轴名字}
                );

            if (OverlayManager.Instance.Visible)
            {
                // 根据小窗口状态设置窗口大小约束
                if (style.Save.SmallWindow)
                {
                    // 小窗口模式：锁定窗口大小
                    var smallSize = GlobalSetting.Instance.缩放后窗口大小 * style.OverlayScale;
                    ImGui.SetNextWindowSizeConstraints(smallSize, smallSize);
                }
                else
                {
                    // 正常模式：允许调整大小
                    ImGui.SetNextWindowSizeConstraints(new Vector2(0, 0), new Vector2(float.MaxValue, float.MaxValue));
                }

                //标题栏风格 无滚动条 不会通过鼠标滚轮滚动内容
                if (ImGui.Begin(title, ref OverlayManager.Instance.Visible,
                        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
                {
                    ImGui.SetWindowFontScale(SettingMgr.GetSetting<GeneralSettings>().OverlayScale);

                    // 绘制顶部运行状态栏
                    //DrawTopStatusBar(Share.CombatRun, PlayerOptions.Instance.Stop);

                    MainControlView(ref Share.CombatRun, ref PlayerOptions.Instance.Stop);
                    UpdateAction?.Invoke();
                    //saveSetting();
                    //tab标签页
                    ImGui.Dummy(new Vector2(0, 5));
                    using (var bar = ImRaii.TabBar("###tab"))
                    {
                        if (bar.Success)
                        {
                            foreach (var v in ExternalTab)
                            {
                                if (v.Key == "Dev")
                                {
                                    continue;
                                }

                                using var item = ImRaii.TabItem(v.Key);
                                if (item.Success)
                                {
                                    using var child = ImRaii.Child($"###tab{v.Key}");
                                    if (child.Success)
                                        v.Value.Invoke(this);
                                }
                            }

                            using (var item = ImRaii.TabItem("Qt"))
                            {
                                if (item.Success)
                                {
                                    using var child = ImRaii.Child($"###Qt");
                                    if (child.Success)
                                        QtSettingView();
                                }
                            }

                            using (var item = ImRaii.TabItem("Hotkey"))
                            {
                                if (item.Success)
                                {
                                    using var child = ImRaii.Child($"###Hotkey");
                                    if (child.Success)
                                        HotkeySettingView();
                                }
                            }

                            using (var item = ImRaii.TabItem("风格"))
                            {
                                if (item.Success)
                                {
                                    using var child = ImRaii.Child($"###风格");
                                    if (child.Success)
                                        ChangeStyleView();
                                }
                            }

                            if (ExternalTab.ContainsKey("Dev"))
                            {
                                using (var item = ImRaii.TabItem("Dev"))
                                {
                                    if (item.Success)
                                    {
                                        using var child = ImRaii.Child($"###tabDev");
                                        if (child.Success)
                                            ExternalTab["Dev"].Invoke(this);
                                    }
                                }
                            }
                        }
                    }

                    ImGui.End();
                }
            }

            if (GlobalSetting.Instance.QtShow && GlobalSetting.Instance.TempQtShow)
            {
                if (!OverlayManager.Instance.Visible)
                {
                    if (!GlobalSetting.QT快捷栏随主界面隐藏)
                    {
                        DrawQtWindow();
                    }
                }
                else
                {
                    DrawQtWindow();
                }
            }

      
                SettingsButtonWindow.Draw();

                设置界面.Draw();
            
            
            
            if (GlobalSetting.Instance.HotKeyShow && GlobalSetting.Instance.TempHotShow)
            {
                if (!OverlayManager.Instance.Visible)
                {
                    if (!GlobalSetting.QT快捷栏随主界面隐藏)
                    {
                        DrawHotkeyWindow();
                    }
                }
                else
                {
                    DrawHotkeyWindow();
                }
            }

            #endregion
        }
        catch (Exception e)
        {
            LogHelper.Error(e.Message);
        }
        finally
        {
            EndMainStyle();
        }
    }
}