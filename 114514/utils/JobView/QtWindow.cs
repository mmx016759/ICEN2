using System.Numerics;
using AEAssist;
using AEAssist.GUI;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using ICEN2.utils.JobView.HotKey;
using ImGuiNET;
using Keys = AEAssist.Define.HotKey.Keys;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace ICEN2.utils.JobView;

/// Qt窗口类
public class QtWindow
{
    public JobViewSave save;
    public readonly string name;
    
    /// 用于储存所有qt控件的字典
    private Dictionary<string, QtControl> QtDict = new();

    /// 动态按顺序储存qt名称的list，用于排序显示qt
    public List<string> QtNameList = [];

    /// 隐藏的qt列表
    public List<string> QtUnVisibleList => save.QtUnVisibleList;

    public Dictionary<string, HotkeyConfig> QtHotkeyConfig => save.QtHotkeyConfig;
    
    ///窗口拖动
    public bool LockWindow;
    
    /// 记录上一次保存的位置，用于检测位置变化
    private Vector2 lastSavedPosition;
    
    /// 构造函数中恢复窗口位置
    public QtWindow(JobViewSave save, string name)
    {
        this.save = save;
        this.name = name;
        
        // 恢复窗口位置
        RestoreWindowPosition();
        
        // 初始化上次保存的位置
        lastSavedPosition = save.QtWindowPos;
    }
    
    /// <summary>
    /// 保存窗口位置
    /// </summary>
    private void SaveWindowPosition()
    {
        // 只有在窗口未被锁定时才保存位置
        if (!LockWindow)
        {
            var pos = ImGui.GetWindowPos();
            // 确保位置有效且不为默认位置
            if (pos.X >= 0 && pos.Y >= 0 && (pos.X != 0 || pos.Y != 0))
            {
                // 检查位置是否发生了变化
                if (pos != lastSavedPosition)
                {
                    save.QtWindowPos = pos;
                    lastSavedPosition = pos;
                }
            }
        }
    }
    
    /// <summary>
    /// 强制保存当前位置（用于保存设置时调用）
    /// </summary>
    public void ForceSaveCurrentPosition()
    {
        if (!LockWindow)
        {
            var pos = ImGui.GetWindowPos();
            if (pos.X >= 0 && pos.Y >= 0)
            {
                save.QtWindowPos = pos;
                lastSavedPosition = pos;
            }
        }
    }
    
    /// <summary>
    /// 恢复窗口位置
    /// </summary>
    private void RestoreWindowPosition()
    {

            // 确保位置在屏幕范围内
            var displaySize = ImGui.GetIO().DisplaySize;
            var pos = save.QtWindowPos;
            
            // 限制位置在屏幕范围内
            pos.X = Math.Max(0, Math.Min(pos.X, displaySize.X - 100));
            pos.Y = Math.Max(0, Math.Min(pos.Y, displaySize.Y - 100));
            
            save.QtWindowPos = pos;

    }
    
    ///QT按钮一行有几个
    public int QtLineCount
    {
        get => save.QtLineCount;
        set => save.QtLineCount = value;
    }

    private class QtControl
    {
        public readonly string Name;
        public bool QtValue;
        public bool QtValueDefault;
        public string ToolTip = "";

        public Action<bool> OnClick;

        // 自定义按钮颜色（优先于默认颜色）
        public Vector4 Color;

        // 是否使用自定义颜色
        public bool UseColor;



        public QtControl(string name, bool qtValueDefault, bool useColor)
        {
            Name = name;
            QtValueDefault = qtValueDefault;
            OnClick = _ => { LogHelper.Info($"检测到按钮触发，改变qt \"{Name}\" 状态为 {QtValue}"); };
            Color = QtStyle.DefaultMainColor;
            UseColor = useColor;
            Reset();
        }

        public QtControl(string name, bool qtValueDefault, Action<bool> action, bool useColor)
        {
            Name = name;
            QtValueDefault = qtValueDefault;
            OnClick = action;
            Color = QtStyle.DefaultMainColor;
            UseColor = useColor;
            Reset();
        }

        public QtControl(string name, bool qtValueDefault, Action<bool> action, Vector4 color)
        {
            Name = name;
            QtValueDefault = qtValueDefault;
            OnClick = action;
            Color = color;
            UseColor = true;
            Reset();
        }

        ///重置qt状态
        public void Reset()
        {
            QtValue = QtValueDefault;
        }
    }

    /// <summary>
    /// 添加新的qt控件
    /// </summary>
    /// <param name="qtName">qt的名称</param>
    /// <param name="qtValueDefault">qt的bool默认值</param>
    public void AddQt(string qtName, bool qtValueDefault)
    {
        if (QtDict.ContainsKey(qtName))
            return;
        var qt = new QtControl(qtName, qtValueDefault, false);
        QtDict.Add(qtName, qt);
        if (!QtNameList.Contains(qtName))
        {
            QtNameList.Add(qtName);
        }
    }

    public void AddQt(string qtName, bool qtValueDefault, string toolTip)
    {
        if (QtDict.ContainsKey(qtName))
            return;
        var qt = new QtControl(qtName, qtValueDefault, false)
        {
            ToolTip = toolTip
        };
        QtDict.Add(qtName, qt);
        if (!QtNameList.Contains(qtName))
        {
            QtNameList.Add(qtName);
        }
    }


    /// <summary>
    /// 添加新的qt控件
    /// </summary>
    /// <param name="qtName">qt的名称</param>
    /// <param name="qtValueDefault">qt的bool默认值</param>
    /// <param name="action">按下时触发的方法</param>
    public void AddQt(string qtName, bool qtValueDefault, Action<bool> action)
    {
        if (QtDict.ContainsKey(qtName))
            return;
        var qt = new QtControl(qtName, qtValueDefault, action, false);
        QtDict.Add(qtName, qt);
        if (!QtNameList.Contains(qtName))
        {
            QtNameList.Add(qtName);
        }
    }

    public void AddQt(string qtName, bool qtValueDefault, Action<bool> action, Vector4 color)
    {
        if (QtDict.ContainsKey(qtName))
            return;
        var qt = new QtControl(qtName, qtValueDefault, action, color);
        QtDict.Add(qtName, qt);
        if (!QtNameList.Contains(qtName))
        {
            QtNameList.Add(qtName);
        }
    }

    public void RemoveAllQt()
    {
        QtDict.Clear();
    }


    /// 设置上一次add添加的hotkey的toolTip
    public void SetQtToolTip(string toolTip)
    {
        QtDict[QtDict.Keys.ToArray()[^1]].ToolTip = toolTip;
    }

    /// 获取指定名称qt的bool值
    public bool GetQt(string qtName)
    {
        return QtDict.ContainsKey(qtName) && QtDict[qtName].QtValue;
    }

    /// 设置指定qt的值
    /// <returns>成功返回true，否则返回false</returns>
    public bool SetQt(string qtName, bool qtValue)
    {
        if (!QtDict.TryGetValue(qtName, out QtControl? value))
            return false;
        value.QtValue = qtValue;
        return true;
    }

    /// 反转指定qt的值
    /// <returns>成功返回true，否则返回false</returns>
    public bool ReverseQt(string qtName)
    {
        if (!QtDict.TryGetValue(qtName, out QtControl? value))
            return false;
        value.QtValue = !value.QtValue;
        return true;
    }

    /// 重置所有qt为默认值
    public void Reset()
    {
        if (!save.AutoReset)
            return;
        foreach (var qt in QtDict.Select(_qt => _qt.Value))
        {
            qt.Reset();
            LogHelper.Info("重置所有qt为默认值");
        }
    }

    /// 给指定qt设置新的默认值
    public void NewDefault(string qtName, bool newDefault)
    {
        if (!QtDict.TryGetValue(qtName, out QtControl? value))
            return;
        value.QtValueDefault = newDefault;
        LogHelper.Info($"改变qt \"{value.Name}\" 默认值为 {value.QtValueDefault}");
    }

    /// 将当前所有Qt状态记录为新的默认值
    public void SetDefaultFromNow()
    {
        foreach (var qt in QtDict.Select(_qt => _qt.Value))
            if (qt.QtValueDefault != qt.QtValue)
            {
                qt.QtValueDefault = qt.QtValue;
                LogHelper.Info($"改变qt \"{qt.Name}\" 默认值为 {qt.QtValueDefault}");
            }
    }

    /// 返回包含当前所有qt名字的数组
    public string[] GetQtArray()
    {
        return QtDict.Keys.ToArray();
    }

    /// 画一个新的Qt窗口
    public void DrawQtWindow(QtStyle style)
    {
        if (!save.ShowQT)
            return;
        
        // 检查并同步主题变化
        style.CheckAndSyncTheme();
        
        style.SetQtStyle();

        int visibleCount = QtNameList.Count - QtUnVisibleList.Count;
        var line = Math.Ceiling((float)visibleCount / QtLineCount);
        var row = visibleCount < QtLineCount ? visibleCount : QtLineCount;
        var width = (row - 1) * 6 + 16 + style.QtButtonSize.X * row;
        var height = (line - 1) * 6 + 16 + style.QtButtonSize.Y * line;
        
        // 设置窗口大小和位置
        ImGui.SetNextWindowSize(new Vector2(width, (int)height));
        
        ImGui.SetNextWindowPos(save.QtWindowPos, ImGuiCond.Once);

        
        var flag = LockWindow ? QtStyle.QtWindowFlag | ImGuiWindowFlags.NoMove : QtStyle.QtWindowFlag;
        ImGui.Begin($"###Qt_Window{name}", flag);

        int i = 0;
        foreach (var qtName in QtNameList)
        {
            if (!QtDict.ContainsKey(qtName))
                continue;
            if (QtUnVisibleList.Contains(qtName))
                continue;

            var qt = QtDict[qtName];
            if (qt.UseColor)
            {
                if (QtSwitchButton(qtName, ref qt.QtValue, qt.Color, ref style))
                    qt.OnClick(qt.QtValue);
            }
            else
            {
                if (QtSwitchButton(qtName, ref qt.QtValue, ref style))
                    qt.OnClick(qt.QtValue);
            }


            if (qt.ToolTip != "")
                ImGuiHelper.SetHoverTooltip(qt.ToolTip);
            if (i % QtLineCount != QtLineCount - 1)
                ImGui.SameLine();
            i++;
        }



        // 保存窗口位置
        SaveWindowPosition();
        
        ImGui.End();
        style.EndQtStyle();

    }

    /// 用于draw一个更改qt排序显示等设置的视图
    public void QtSettingView()
    {
        ImGui.Checkbox("显示QT控件", ref save.ShowQT);
        ImGui.Checkbox("战斗结束qt自动重置回战斗前状态", ref save.AutoReset);
        
        // 添加标签页切换按钮
        ImGui.Separator();
        
        ImGui.TextDisabled("   *左键拖动改变qt顺序，右键点击qt显示更多操作");
        for (var i = 0; i < QtNameList.Count; i++)
        {
            var item = QtNameList[i];
            var visible = !QtUnVisibleList.Contains(item) ? "显示" : "隐藏";

            if (visible == "隐藏")
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0, 0, 1));

            ImGui.Selectable($"   {visible}        {item}");

            if (visible == "隐藏")
                ImGui.PopStyleColor(1);

            //排序        
            if (ImGui.IsItemActive() && !ImGui.IsItemHovered())
            {
                var n_next = i + (ImGui.GetMouseDragDelta(0).Y < 0f ? -1 : 1);
                if (n_next < 0 || n_next >= QtNameList.Count)
                    continue;
                QtNameList[i] = QtNameList[n_next];
                QtNameList[n_next] = item;
                ImGui.ResetMouseDragDelta();
            }

            //右键
            ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1);
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.2f, 0.2f, 0.2f, 1));
            if (ImGuiHelper.IsRightMouseClicked())
                ImGui.OpenPopup($"###hotkeyPopup{name + i}");
            if (ImGui.BeginPopup($"###hotkeyPopup{name + i}"))
            {
                //显示隐藏
                var vis = !QtUnVisibleList.Contains(item);
                if (ImGui.Checkbox("显示", ref vis))
                {
                    if (!vis)
                        QtUnVisibleList.Add(item);
                    else
                        QtUnVisibleList.Remove(item);
                }

                if (!QtHotkeyConfig.TryGetValue(item, out var hotkeyConfig))
                {
                    hotkeyConfig = new HotkeyConfig();
                    QtHotkeyConfig[item] = hotkeyConfig;
                }

                //快捷键设置
                ImGuiHelper.KeyInput("快捷键设置", ref hotkeyConfig.Keys);
                ImGuiHelper.DrawEnum("组合键", ref hotkeyConfig.ModifierKey);


                if (ImGui.Button("重置"))
                {
                    QtHotkeyConfig.Remove(item);
                }


                ImGui.EndPopup();
            }

            ImGui.PopStyleColor(1);
            ImGui.PopStyleVar(1);

        }
    }


    /// 重新包装的开关按钮控件，一般用于QT按钮
    private bool QtSwitchButton(string label, ref bool buttonValue, ref QtStyle style)
    {
        // 使用当前主题的Primary颜色，而不是固定的MainColor
        var themeColor = style.ModernTheme.Colors.Primary;
        return QtSwitchButton(label, ref buttonValue, themeColor, ref style);
    }

    /// 重新包装的开关按钮控件，接受自定义颜色而不是全局颜色
    private bool QtSwitchButton(string label, ref bool buttonValue, Vector4 color, ref QtStyle style)
    {
        var size = style.QtButtonSize;
        var ret = false;

        ImGui.BeginChild($"###Hotkey_QTControl{name + label}", size, false, QtStyle.QtWindowFlag);

        Vector4 falseColor = new(25 / 255f, 25 / 255f, 25 / 255f, color.W);
        if (buttonValue)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.SetCursorPos(size * 0.05f);
            if (ImGui.Button(label, size * 0.9f))
            {
                ret = true;
                buttonValue = !buttonValue;
            }

            ImGui.PopStyleColor(3);
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Button, falseColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, falseColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, falseColor);
            ImGui.SetCursorPos(size * 0.05f);
            if (ImGui.Button(label, size * 0.9f))
            {
                ret = true;
                buttonValue = !buttonValue;
            }

            ImGui.PopStyleColor(3);
        }

        if (QtHotkeyConfig.TryGetValue(label, out var hotkeyConfig) && hotkeyConfig.Keys != Keys.None)
        {
            //显示快捷键
            var text = HotkeyWindow.HotkeySplice(hotkeyConfig.ModifierKey, hotkeyConfig.Keys);
            ImGui.SetCursorPos(new Vector2(0, 0));
            ImGui.Text(text);
        }

        ImGui.EndChild();

        return ret;
    }


    public void RunHotkey()
    {
        foreach (QtControl? control in from hotkey in QtHotkeyConfig 
                 where hotkey.Value.Keys != Keys.None 
                 where Core.Resolve<MemApiHotkey>().CheckState(hotkey.Value.ModifierKey, hotkey.Value.Keys) 
                 select QtDict[hotkey.Key])
        {
            control.QtValue = !control.QtValue;
        }
    }
}