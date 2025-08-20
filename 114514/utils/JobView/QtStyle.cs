using System.Numerics;
using AEAssist.CombatRoutine;
using AEAssist.Helper;
using Dalamud.Interface.Style;
using ImGuiNET;

// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToConstant.Global

namespace ICEN2.utils.JobView;

public class QtStyle
{
    // 保存对JobViewSave的引用
    private readonly JobViewSave save;
    public JobViewSave Save => save;
    
    // 现代主题实例 - 从保存的设置中读取，而不是硬编码
    private ModernTheme modernTheme;
    public ModernTheme ModernTheme => modernTheme;
    
    // 标记现代主题是否已应用
    private bool isModernThemeApplied = false;
    
    // 主题预设
    private ModernTheme.ThemePreset lastTheme;
    
    /// <summary>
    /// 检测主题是否发生变化
    /// </summary>
    public bool CurrentThemeChanged 
    { 
        get 
        {
            var changed = lastTheme != save.CurrentTheme;
            if (changed)
            {
                LogHelper.Debug($"QtStyle: 主题变化检测到，lastTheme: {lastTheme}, 当前主题: {save.CurrentTheme}");
            }
            return changed;
        }
    }
    
    /// <summary>
    /// 更新最后记录的主题
    /// </summary>
    public void UpdateLastTheme()
    {
        lastTheme = save.CurrentTheme;
    }
    
    /// <summary>
    /// 检查并自动同步主题变化
    /// </summary>
    public void CheckAndSyncTheme()
    {
        if (CurrentThemeChanged)
        {
            LogHelper.Debug($"QtStyle: CheckAndSyncTheme - 重新创建主题实例: {save.CurrentTheme}");
            // 重新创建 ModernTheme 实例以确保颜色更新
            modernTheme = new ModernTheme(save.CurrentTheme);
            lastTheme = save.CurrentTheme;
            LogHelper.Debug($"QtStyle: CheckAndSyncTheme - 主题实例已更新");
        }
    }
    
    public ModernTheme.ThemePreset CurrentTheme 
    { 
        get => save.CurrentTheme;
        set 
        {
            if (save.CurrentTheme != value)
            {
                LogHelper.Debug($"QtStyle: 设置新主题: {value}, 旧主题: {save.CurrentTheme}");
                save.CurrentTheme = value;
                lastTheme = value;
                modernTheme.ApplyPreset(value);
                LogHelper.Debug($"QtStyle: 主题已更新，lastTheme: {lastTheme}");
            }
        }
    }
    
    // 构造函数中正确初始化主题
    public QtStyle(JobViewSave save)
    {
        this.save = save;
        // 从保存的设置中读取主题，如果没有保存过则使用默认值
        var savedTheme = save.CurrentTheme;
        modernTheme = new ModernTheme(savedTheme);
        lastTheme = savedTheme; // 初始化lastTheme
    }
    
    public float OverlayScale => SettingMgr.GetSetting<GeneralSettings>().OverlayScale;
    public Vector2 QtButtonSize => save.QtButtonSize * OverlayScale;

    public Vector2 QtButtonSizeOrigin
    {
        get => save.QtButtonSize;
        set => save.QtButtonSize = value;
    }
    public Vector2 HotkeySizeOrigin
    {
        get => save.QtHotkeySize;
        set => save.QtHotkeySize = value;
    }

    //标题栏风格 无滚动条 无标题 不可调整大小
    public const ImGuiWindowFlags QtWindowFlag =
        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoTitleBar |
        ImGuiWindowFlags.NoResize;
    //默认风格
    public static Vector4 DefaultMainColor = new(161 / 255f, 47 / 255f, 114 / 255f, 0.8f);
    public static float DefaultQtWindowBgAlpha = 0.3f;
    public static Vector2 DefaultButtonSize = new(95, 40);
    public static Vector2 DefaultHotkeySize = new(56, 56);

    public Vector4 ColorFalse => new(25 / 255f, 25 / 255f, 25 / 255f, MainColor.W);
    public Vector4 ColorDark => new(47 / 255f, 47 / 255f, 47 / 255f, MainColor.W);
    public Vector4 ColorDark2 => new(81 / 255f, 81 / 255f, 81 / 255f, MainColor.W);

    public Vector4 ColorHovered => new((MainColor.X * 255 + 20) / 255f, (MainColor.Y * 255 + 20) / 255f,
        (MainColor.Z * 255 + 20) / 255f, MainColor.W);

    public Vector4 ColorActive => new((MainColor.X * 255 + 50) / 255f, (MainColor.Y * 255 + 50) / 255f,
        (MainColor.Z * 255 + 50) / 255f, MainColor.W);

    /// 主颜色
    public Vector4 MainColor
    {
        get => save.MainColor;
        set => save.MainColor = value;
    }

    /// Qt窗口背景透明度
    public float QtWindowBgAlpha
    {
        get => save.QtWindowBgAlpha;
        set => save.QtWindowBgAlpha = value;
    }

    
    public static StyleModel MUIStyle = StyleModel.Deserialize(GlobalSetting.StyleStr);


    /// <summary>
    /// 初始化主窗口风格
    /// </summary>
    public void SetMainStyle()
    {
        // ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0, 0, 0, 0));
        // ImGui.PushStyleColor(ImGuiCol.FrameBg, ColorDark);
        // ImGui.PushStyleColor(ImGuiCol.PopupBg, DefaultMainColor with { X = 0, Y = 0, Z = 0 });
        // ImGui.PushStyleColor(ImGuiCol.ResizeGrip, ColorFalse);
        // ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, MainColor);
        // ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, MainColor);
        // ImGui.PushStyleColor(ImGuiCol.CheckMark, ColorActive);
        // ImGui.PushStyleColor(ImGuiCol.SliderGrab, MainColor);
        // ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, ColorActive);
        // ImGui.PushStyleColor(ImGuiCol.Button, ColorDark2);
        // ImGui.PushStyleColor(ImGuiCol.ButtonActive, ColorActive);
        // ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ColorHovered);
        // ImGui.PushStyleColor(ImGuiCol.Header, MainColor);
        // ImGui.PushStyleColor(ImGuiCol.HeaderActive, ColorActive);
        // ImGui.PushStyleColor(ImGuiCol.HeaderHovered, ColorHovered);
        // ImGui.PushStyleColor(ImGuiCol.Tab, ColorDark);
        // ImGui.PushStyleColor(ImGuiCol.TabActive, MainColor);
        // ImGui.PushStyleColor(ImGuiCol.TabHovered, ColorHovered);
        MUIStyle.Push();
        
        // 应用现代主题，并标记已应用
        if (!isModernThemeApplied)
        {
            modernTheme.Apply();
            isModernThemeApplied = true;
        }
        // ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(37,37,37,0));
    }

    /// <summary>
    /// 注销主窗口风格，绝对不能少
    /// </summary>
    public void EndMainStyle()
    {
        MUIStyle.Pop();
        
        // 回收现代主题样式
        if (isModernThemeApplied)
        {
            modernTheme.Restore();
            isModernThemeApplied = false;
        }
        // ImGui.PopStyleColor(1);
    }

    public void SetQtStyle()
    {
        ImGui.SetNextWindowSize(new Vector2(300, 450), ImGuiCond.FirstUseEver);
        // ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 8));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(6, 6));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, QtWindowBgAlpha));
    }

    public void EndQtStyle()
    {
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(1);
    }


    public void Reset()
    {
        string str =
            "DS1H4sIAAAAAAAACqVY23LbNhD9FQ+fHQ8JEACpt9hq484kHU/sNE3fIAmWWNGiSlHKxZN/7wLEgiBIJbLiFwjgnoO9YbHwcySjSXIVX0azaPIc/R1NMj35ZMbvl9E8msR6YWFHZaViKxVfMZB6BI7LaGmXV3Ys7Chn9se/FpxaMDVbrO3X0o5PgSKpkdoE2Ha1ClaJWd0OlNSr/8FnY2kN2pofOyvSwIL5sbcLB8v42c6/2PGrI2ae9d9Gt5PSLtOetfMK7HyOHtSXxn5P7Hcz/mPHj2YEeS04LXZyVqrFcHcDMKMDfCw2i+rz9dIJJ4SleUwshMQiS2mSWiQV5is1BPFVHqe5ECnQ3KyKcuGxONMssNWxNfWu2u63vmyGwhlKZ7iB5r6u6oWqnTjJOBEpQw0ZT0XMcovUyoKS3LOwhd+vJNj5E/1awO+1fFKefiSPk5wn6BGcGZybGXXTDn1bHVTtxyBFfdGX+keIej1vikN3aDjNScKERbqZgbuZ4TBRoiYUD0VT+uonQuScEae/yBghOWqRwkcqcufwmGUZobzjCVQ63ftjZDdVWcrtzvPLmXzv1GZ/LWvfzKRNTkuUcEPMMU7c/I2YeT+vQalZj+tH2evk39S6UllE2pqBVujQcLQCY+OlWI9kkCrU/HEXdgbhckcQ1NbzY2RBuF7KdbNS8/U7Wa+DagNWcFMK0J+Q9Xruq1EWcM56Xjma9CEizHuX8i7bhX+i901TbY6cT6oDy1BPE5VkgA1d3tU1w5EirOXwzrhPEp6Ml6hxq6Rf01hGRRJr737Caexy181MDtLEwQeJM1rnIcwOEfoZ3Yxq+sFRW1nLpvIKL8W0thaatHYm4kUxZAj1PJso9PgLed6rXfFNvamL7sY/n+J8o2AGghmnPb6zbYOP+kxDmdd11jt9CdzNhBJ3lecij2PukiMmhBOXIqYeMnN/DCtSDNkb5xnmdtKeFjLeFPSpArNewGQbGjn7sHms5nv/0kiEviYwe0nLiST6AgEN0iMkv6rQtJqvi83yrlaHQnUNRdKWR+dsF75Wp6FvLM9vT9vmq99sIAEWWm/ru7Jq3hYbtesOMOrtGpJenXKAfkC7ou7qOWUB7LbYNdUSuhK3F6aiGGmZeohjm2GV6fWrulNta5N/l7tbPB9xg8HYrq6pq43nPJZAMHOXGDClHLelgjHO+DjR22K5wg5bR8+FEfdnIe59r2v+UcfQib8uf9bFA4LbNv5elWreKL+r1p2QbTDsPq5xo/ppMa3lclpX2wdZL9WxrTrlcoD8KQ+3YHvZt//YPq39gGmfDZC/Ifi4YSJATosnzzR8A+ATAO0iutmrFrJscaeBwBn6DQgtdTSJ/pKbArrOiw9/XLy6uCmVrC+uy72K4JXaPq7kIFOD5tE9iZwHuiIrglPh9ynwGj5B6rR3mjrp+ffYuUY45wjnHv3LyS6HLV6M4fY5V4Mk5yM7FwMp3DVpW1BPFh/2ekdbqo8+xtZHs9gv8GXHGLoa3j6eYFfOcFP3EPKkut4SrtXRw9PK4f8TOqNT2rqw75yu34BXW/eyRg17zoH/OwRhYTEWd5+z68uyDI9r6jg580PdXRiZPsi98Bh2KwmiULph9fv/zDrolOkRAAA=";
        MUIStyle = StyleModel.Deserialize(str);
        GlobalSetting.StyleStr = str;
        GlobalSetting.Instance.Save();
        save.QtWindowBgAlpha = DefaultQtWindowBgAlpha;
        save.QtButtonSize = DefaultButtonSize;
        save.QtHotkeySize = DefaultHotkeySize;
    }
}