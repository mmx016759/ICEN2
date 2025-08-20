using System.Numerics;
using ImGuiNET;

namespace icen.utils.JobView;

/// <summary>
/// 现代化UI主题系统
/// </summary>
public class ModernTheme
{
    // 主题预设
    public enum ThemePreset
    {
        深色模式,
        浅色模式,
        午夜蓝,
        樱花粉,
        森林绿,
        科技紫,
        海洋蓝,      // 新增
        日落橙,      // 新增
        清新绿,      // 新增
        咖啡棕,      // 新增
        熔岩红,       // 新增
        沙漠金,      // 新增：沙漠金色调
        冰川蓝,      // 新增：冰川蓝白色调
        翡翠绿,      // 新增：深翡翠绿色调
        石板灰,       // 新增：中性石板灰色调
        冰雪白  // 新增主题
    }

    // 颜色定义
    public struct ColorScheme
    {
        public Vector4 Primary;           // 主色调
        public Vector4 Secondary;         // 次要色调
        public Vector4 Accent;           // 强调色
        public Vector4 Background;       // 背景色
        public Vector4 Surface;          // 表面色
        public Vector4 Text;             // 文本色
        public Vector4 TextSecondary;    // 次要文本色
        public Vector4 Success;          // 成功色
        public Vector4 Warning;          // 警告色
        public Vector4 Error;            // 错误色
        public Vector4 Border;           // 边框色
        public Vector4 Shadow;           // 阴影色
    }

    // 当前配色方案
    public ColorScheme Colors { get; set; }
    
    // 保存原始ImGui样式
    private ImGuiStylePtr? savedStyle = null;
    private readonly Dictionary<ImGuiCol, Vector4> savedColors = new Dictionary<ImGuiCol, Vector4>();
    private bool isApplied = false;

    // 圆角半径
    public float BorderRadius { get; set; } = 8f;

    // 阴影强度
    public float ShadowStrength { get; set; } = 0.3f;

    // 动画速度
    public float AnimationSpeed { get; set; } = 0.15f;

    // 预设主题
    private static readonly Dictionary<ThemePreset, ColorScheme> Presets = new()
    {
        [ThemePreset.深色模式] = new ColorScheme
        {
            Primary = new Vector4(0.2f, 0.6f, 1f, 1f),          // 天蓝色
            Secondary = new Vector4(0.1f, 0.8f, 0.8f, 1f),      // 青色
            Accent = new Vector4(1f, 0.4f, 0.7f, 1f),          // 粉色
            Background = new Vector4(0.08f, 0.08f, 0.1f, 0.95f), // 深灰背景
            Surface = new Vector4(0.12f, 0.12f, 0.14f, 1f),    // 表面色
            Text = new Vector4(0.95f, 0.95f, 0.95f, 1f),       // 白色文本
            TextSecondary = new Vector4(0.7f, 0.7f, 0.7f, 1f), // 灰色文本
            Success = new Vector4(0.2f, 0.8f, 0.4f, 1f),       // 绿色
            Warning = new Vector4(1f, 0.7f, 0.2f, 1f),         // 橙色
            Error = new Vector4(1f, 0.3f, 0.3f, 1f),           // 红色
            Border = new Vector4(0.3f, 0.3f, 0.35f, 0.5f),     // 边框色
            Shadow = new Vector4(0f, 0f, 0f, 0.5f)             // 阴影色
        },
        [ThemePreset.浅色模式] = new ColorScheme
        {
            Primary = new Vector4(0.1f, 0.5f, 0.9f, 1f),
            Secondary = new Vector4(0f, 0.7f, 0.7f, 1f),
            Accent = new Vector4(0.9f, 0.3f, 0.6f, 1f),
            Background = new Vector4(0.98f, 0.98f, 0.98f, 0.95f),
            Surface = new Vector4(1f, 1f, 1f, 1f),
            Text = new Vector4(0.1f, 0.1f, 0.1f, 1f),
            TextSecondary = new Vector4(0.4f, 0.4f, 0.4f, 1f),
            Success = new Vector4(0.1f, 0.7f, 0.3f, 1f),
            Warning = new Vector4(0.9f, 0.6f, 0.1f, 1f),
            Error = new Vector4(0.9f, 0.2f, 0.2f, 1f),
            Border = new Vector4(0.8f, 0.8f, 0.82f, 0.5f),
            Shadow = new Vector4(0f, 0f, 0f, 0.2f)
        },
        [ThemePreset.午夜蓝] = new ColorScheme
        {
            Primary = new Vector4(0.1f, 0.3f, 0.8f, 1f),
            Secondary = new Vector4(0.2f, 0.5f, 0.9f, 1f),
            Accent = new Vector4(0.5f, 0.8f, 1f, 1f),
            Background = new Vector4(0.05f, 0.05f, 0.15f, 0.95f),
            Surface = new Vector4(0.08f, 0.1f, 0.2f, 1f),
            Text = new Vector4(0.9f, 0.92f, 0.95f, 1f),
            TextSecondary = new Vector4(0.6f, 0.65f, 0.75f, 1f),
            Success = new Vector4(0.3f, 0.85f, 0.5f, 1f),
            Warning = new Vector4(1f, 0.8f, 0.3f, 1f),
            Error = new Vector4(1f, 0.4f, 0.4f, 1f),
            Border = new Vector4(0.2f, 0.3f, 0.5f, 0.5f),
            Shadow = new Vector4(0f, 0f, 0.05f, 0.6f)
        },
        [ThemePreset.樱花粉] = new ColorScheme
        {
            Primary = new Vector4(1f, 0.6f, 0.8f, 1f),
            Secondary = new Vector4(1f, 0.8f, 0.9f, 1f),
            Accent = new Vector4(0.9f, 0.3f, 0.5f, 1f),
            Background = new Vector4(1f, 0.95f, 0.97f, 0.95f),
            Surface = new Vector4(1f, 0.98f, 0.99f, 1f),
            Text = new Vector4(0.3f, 0.2f, 0.25f, 1f),
            TextSecondary = new Vector4(0.6f, 0.5f, 0.55f, 1f),
            Success = new Vector4(0.4f, 0.8f, 0.5f, 1f),
            Warning = new Vector4(1f, 0.7f, 0.4f, 1f),
            Error = new Vector4(0.9f, 0.3f, 0.4f, 1f),
            Border = new Vector4(1f, 0.8f, 0.85f, 0.5f),
            Shadow = new Vector4(0.8f, 0.6f, 0.7f, 0.2f)
        },
        [ThemePreset.森林绿] = new ColorScheme
        {
            Primary = new Vector4(0.2f, 0.7f, 0.4f, 1f),
            Secondary = new Vector4(0.4f, 0.8f, 0.5f, 1f),
            Accent = new Vector4(0.8f, 0.9f, 0.3f, 1f),
            Background = new Vector4(0.08f, 0.12f, 0.08f, 0.95f),
            Surface = new Vector4(0.12f, 0.18f, 0.12f, 1f),
            Text = new Vector4(0.95f, 0.98f, 0.95f, 1f),
            TextSecondary = new Vector4(0.7f, 0.8f, 0.7f, 1f),
            Success = new Vector4(0.3f, 0.9f, 0.5f, 1f),
            Warning = new Vector4(1f, 0.85f, 0.4f, 1f),
            Error = new Vector4(0.9f, 0.4f, 0.3f, 1f),
            Border = new Vector4(0.3f, 0.5f, 0.35f, 0.5f),
            Shadow = new Vector4(0f, 0.05f, 0f, 0.5f)
        },
        [ThemePreset.科技紫] = new ColorScheme
        {
            Primary = new Vector4(0.6f, 0.3f, 0.9f, 1f),
            Secondary = new Vector4(0.8f, 0.5f, 1f, 1f),
            Accent = new Vector4(0.3f, 0.9f, 0.9f, 1f),
            Background = new Vector4(0.1f, 0.08f, 0.15f, 0.95f),
            Surface = new Vector4(0.15f, 0.12f, 0.2f, 1f),
            Text = new Vector4(0.95f, 0.95f, 1f, 1f),
            TextSecondary = new Vector4(0.75f, 0.75f, 0.85f, 1f),
            Success = new Vector4(0.4f, 0.9f, 0.6f, 1f),
            Warning = new Vector4(1f, 0.7f, 0.5f, 1f),
            Error = new Vector4(1f, 0.5f, 0.5f, 1f),
            Border = new Vector4(0.5f, 0.3f, 0.7f, 0.5f),
            Shadow = new Vector4(0.05f, 0f, 0.1f, 0.6f)
        },
            [ThemePreset.海洋蓝] = new ColorScheme
    {
        Primary = new Vector4(0.1f, 0.5f, 0.8f, 1f),
        Secondary = new Vector4(0.3f, 0.7f, 0.9f, 1f),
        Accent = new Vector4(0.9f, 0.6f, 0.2f, 1f),
        Background = new Vector4(0.07f, 0.12f, 0.18f, 0.95f),
        Surface = new Vector4(0.1f, 0.2f, 0.3f, 1f),
        Text = new Vector4(0.9f, 0.95f, 1f, 1f),
        TextSecondary = new Vector4(0.6f, 0.75f, 0.85f, 1f),
        Success = new Vector4(0.2f, 0.8f, 0.6f, 1f),
        Warning = new Vector4(1f, 0.8f, 0.4f, 1f),
        Error = new Vector4(1f, 0.4f, 0.4f, 1f),
        Border = new Vector4(0.2f, 0.4f, 0.6f, 0.5f),
        Shadow = new Vector4(0f, 0.1f, 0.2f, 0.6f)
    },
    [ThemePreset.日落橙] = new ColorScheme
    {
        Primary = new Vector4(1f, 0.5f, 0.2f, 1f),
        Secondary = new Vector4(1f, 0.7f, 0.3f, 1f),
        Accent = new Vector4(0.3f, 0.8f, 0.9f, 1f),
        Background = new Vector4(0.15f, 0.1f, 0.08f, 0.95f),
        Surface = new Vector4(0.25f, 0.15f, 0.1f, 1f),
        Text = new Vector4(1f, 0.95f, 0.9f, 1f),
        TextSecondary = new Vector4(0.85f, 0.75f, 0.65f, 1f),
        Success = new Vector4(0.8f, 0.7f, 0.2f, 1f),
        Warning = new Vector4(1f, 0.6f, 0.3f, 1f),
        Error = new Vector4(1f, 0.3f, 0.2f, 1f),
        Border = new Vector4(0.6f, 0.4f, 0.2f, 0.5f),
        Shadow = new Vector4(0.2f, 0.1f, 0f, 0.6f)
    },
    [ThemePreset.清新绿] = new ColorScheme
    {
        Primary = new Vector4(0.3f, 0.8f, 0.4f, 1f),
        Secondary = new Vector4(0.5f, 0.9f, 0.6f, 1f),
        Accent = new Vector4(0.9f, 0.6f, 0.2f, 1f),
        Background = new Vector4(0.95f, 0.98f, 0.96f, 0.95f),
        Surface = new Vector4(0.98f, 1f, 0.99f, 1f),
        Text = new Vector4(0.1f, 0.2f, 0.15f, 1f),
        TextSecondary = new Vector4(0.4f, 0.5f, 0.45f, 1f),
        Success = new Vector4(0.4f, 0.9f, 0.5f, 1f),
        Warning = new Vector4(1f, 0.8f, 0.3f, 1f),
        Error = new Vector4(1f, 0.4f, 0.4f, 1f),
        Border = new Vector4(0.8f, 0.9f, 0.85f, 0.5f),
        Shadow = new Vector4(0.6f, 0.8f, 0.7f, 0.2f)
    },
    [ThemePreset.咖啡棕] = new ColorScheme
    {
        Primary = new Vector4(0.6f, 0.4f, 0.2f, 1f),
        Secondary = new Vector4(0.8f, 0.6f, 0.4f, 1f),
        Accent = new Vector4(0.4f, 0.7f, 0.8f, 1f),
        Background = new Vector4(0.95f, 0.92f, 0.88f, 0.95f),
        Surface = new Vector4(0.98f, 0.95f, 0.92f, 1f),
        Text = new Vector4(0.2f, 0.15f, 0.1f, 1f),
        TextSecondary = new Vector4(0.5f, 0.4f, 0.35f, 1f),
        Success = new Vector4(0.5f, 0.7f, 0.4f, 1f),
        Warning = new Vector4(0.9f, 0.7f, 0.3f, 1f),
        Error = new Vector4(0.8f, 0.4f, 0.3f, 1f),
        Border = new Vector4(0.7f, 0.6f, 0.5f, 0.5f),
        Shadow = new Vector4(0.3f, 0.25f, 0.2f, 0.3f)
    },
    [ThemePreset.熔岩红] = new ColorScheme
    {
        Primary = new Vector4(1f, 0.3f, 0.2f, 1f),
        Secondary = new Vector4(1f, 0.6f, 0.3f, 1f),
        Accent = new Vector4(0.9f, 0.9f, 0.3f, 1f),
        Background = new Vector4(0.15f, 0.05f, 0.03f, 0.95f),
        Surface = new Vector4(0.25f, 0.1f, 0.05f, 1f),
        Text = new Vector4(1f, 0.95f, 0.9f, 1f),
        TextSecondary = new Vector4(0.9f, 0.7f, 0.65f, 1f),
        Success = new Vector4(0.9f, 0.7f, 0.2f, 1f),
        Warning = new Vector4(1f, 0.8f, 0.4f, 1f),
        Error = new Vector4(1f, 0.4f, 0.3f, 1f),
        Border = new Vector4(0.8f, 0.4f, 0.3f, 0.5f),
        Shadow = new Vector4(0.25f, 0.05f, 0f, 0.6f)
    },
        [ThemePreset.沙漠金] = new ColorScheme
    {
        Primary = new Vector4(0.8f, 0.6f, 0.2f, 1f),      // 主金
        Secondary = new Vector4(0.9f, 0.7f, 0.3f, 1f),    // 亮金
        Accent = new Vector4(0.4f, 0.7f, 0.8f, 1f),       // 蓝绿对比
        Background = new Vector4(0.95f, 0.9f, 0.8f, 0.95f), // 沙色
        Surface = new Vector4(1f, 0.96f, 0.88f, 1f),      // 浅沙
        Text = new Vector4(0.25f, 0.2f, 0.1f, 1f),        // 深棕
        TextSecondary = new Vector4(0.5f, 0.4f, 0.3f, 1f),// 中棕
        Success = new Vector4(0.5f, 0.7f, 0.3f, 1f),      // 橄榄绿
        Warning = new Vector4(0.9f, 0.6f, 0.2f, 1f),      // 橙金
        Error = new Vector4(0.8f, 0.3f, 0.2f, 1f),        // 陶红
        Border = new Vector4(0.7f, 0.6f, 0.4f, 0.5f),     // 古铜
        Shadow = new Vector4(0.3f, 0.25f, 0.15f, 0.3f)    // 深沙影
    },
    [ThemePreset.冰川蓝] = new ColorScheme
    {
        Primary = new Vector4(0.4f, 0.7f, 0.9f, 1f),      // 冰川蓝
        Secondary = new Vector4(0.7f, 0.85f, 1f, 1f),     // 浅冰蓝
        Accent = new Vector4(1f, 0.7f, 0.4f, 1f),         // 暖橙对比
        Background = new Vector4(0.95f, 0.97f, 1f, 0.95f), // 冰雪白
        Surface = new Vector4(0.98f, 0.99f, 1f, 1f),      // 纯白
        Text = new Vector4(0.1f, 0.2f, 0.3f, 1f),         // 深蓝灰
        TextSecondary = new Vector4(0.4f, 0.5f, 0.6f, 1f),// 中蓝灰
        Success = new Vector4(0.3f, 0.8f, 0.7f, 1f),      // 绿松石
        Warning = new Vector4(1f, 0.8f, 0.4f, 1f),        // 琥珀
        Error = new Vector4(0.9f, 0.4f, 0.5f, 1f),        // 冰莓红
        Border = new Vector4(0.8f, 0.88f, 0.95f, 0.5f),   // 淡蓝灰
        Shadow = new Vector4(0.7f, 0.8f, 0.9f, 0.2f)      // 浅蓝影
    },
    [ThemePreset.翡翠绿] = new ColorScheme
    {
        Primary = new Vector4(0.2f, 0.6f, 0.4f, 1f),      // 翡翠绿
        Secondary = new Vector4(0.4f, 0.8f, 0.6f, 1f),    // 亮翡翠
        Accent = new Vector4(0.9f, 0.7f, 0.3f, 1f),       // 黄金对比
        Background = new Vector4(0.92f, 0.96f, 0.93f, 0.95f), // 玉白
        Surface = new Vector4(0.96f, 0.99f, 0.97f, 1f),   // 浅玉
        Text = new Vector4(0.1f, 0.2f, 0.15f, 1f),        // 墨绿
        TextSecondary = new Vector4(0.3f, 0.4f, 0.35f, 1f),// 中绿灰
        Success = new Vector4(0.3f, 0.8f, 0.5f, 1f),      // 鲜绿
        Warning = new Vector4(0.9f, 0.8f, 0.3f, 1f),      // 金绿
        Error = new Vector4(0.8f, 0.3f, 0.3f, 1f),        // 朱红
        Border = new Vector4(0.7f, 0.8f, 0.75f, 0.5f),    // 竹青
        Shadow = new Vector4(0.6f, 0.7f, 0.65f, 0.2f)     // 淡绿影
    },
    [ThemePreset.石板灰] = new ColorScheme
    {
        Primary = new Vector4(0.5f, 0.5f, 0.55f, 1f),     // 中性灰
        Secondary = new Vector4(0.7f, 0.7f, 0.75f, 1f),   // 银灰
        Accent = new Vector4(0.9f, 0.6f, 0.3f, 1f),       // 陶土橙
        Background = new Vector4(0.96f, 0.96f, 0.97f, 0.95f), // 灰白
        Surface = new Vector4(1f, 1f, 1f, 1f),            // 纯白
        Text = new Vector4(0.2f, 0.2f, 0.22f, 1f),        // 炭灰
        TextSecondary = new Vector4(0.45f, 0.45f, 0.5f, 1f), // 中灰
        Success = new Vector4(0.4f, 0.7f, 0.5f, 1f),      // 灰绿
        Warning = new Vector4(0.9f, 0.7f, 0.4f, 1f),      // 陶橙
        Error = new Vector4(0.8f, 0.4f, 0.4f, 1f),        // 灰红
        Border = new Vector4(0.8f, 0.8f, 0.82f, 0.5f),    // 淡灰
        Shadow = new Vector4(0.7f, 0.7f, 0.75f, 0.2f)     // 银影
    },
    [ThemePreset.冰雪白] = new ColorScheme
    {
        Primary = new Vector4(0.75f, 0.85f, 0.95f, 1f),    // 冰蓝色
        Secondary = new Vector4(0.9f, 0.95f, 1f, 1f),      // 浅冰蓝
        Accent = new Vector4(0.3f, 0.6f, 0.9f, 1f),        // 深海蓝
        Background = new Vector4(1f, 1f, 1f, 0.97f),       // 纯白背景
        Surface = new Vector4(0.98f, 0.99f, 1f, 1f),       // 微微偏蓝的白
        Text = new Vector4(0.1f, 0.15f, 0.25f, 1f),        // 深蓝灰文本
        TextSecondary = new Vector4(0.4f, 0.5f, 0.6f, 1f), // 中蓝灰
        Success = new Vector4(0.3f, 0.8f, 0.7f, 1f),       // 绿松石
        Warning = new Vector4(1f, 0.8f, 0.4f, 1f),        // 琥珀黄
        Error = new Vector4(0.9f, 0.4f, 0.5f, 1f),        // 冰莓红
        Border = new Vector4(0.85f, 0.9f, 0.95f, 0.4f),    // 淡蓝灰边框
        Shadow = new Vector4(0.7f, 0.8f, 0.9f, 0.15f)      // 极淡蓝影
    }
    };

    public ModernTheme(ThemePreset preset = ThemePreset.深色模式)
    {
        ApplyPreset(preset);
    }

    public void ApplyPreset(ThemePreset preset)
    {
        Colors = Presets[preset];
    }

    /// <summary>
    /// 应用主题到ImGui
    /// </summary>
    public void Apply()
    {
        // 如果已经应用，先恢复再重新应用
        if (isApplied)
        {
            Restore();
        }
        
        var style = ImGui.GetStyle();
        
        // 保存原始样式值
        SaveOriginalStyle(style);
        isApplied = true;

        // 窗口圆角
        style.WindowRounding = BorderRadius;
        style.ChildRounding = BorderRadius;
        style.FrameRounding = BorderRadius * 0.5f;
        style.PopupRounding = BorderRadius;
        style.ScrollbarRounding = BorderRadius * 0.5f;
        style.GrabRounding = BorderRadius * 0.5f;
        style.TabRounding = BorderRadius * 0.5f;

        // 间距和内边距
        style.WindowPadding = new Vector2(12, 12);
        style.FramePadding = new Vector2(8, 6);
        style.ItemSpacing = new Vector2(8, 8);
        style.ItemInnerSpacing = new Vector2(6, 6);
        style.IndentSpacing = 20;
        style.ScrollbarSize = 14;
        style.GrabMinSize = 12;

        // 边框
        style.WindowBorderSize = 1;
        style.ChildBorderSize = 1;
        style.PopupBorderSize = 1;
        style.FrameBorderSize = 0;
        style.TabBorderSize = 0;

        // 对齐
        style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0f, 0.5f);

        // 颜色
        style.Colors[(int)ImGuiCol.Text] = Colors.Text;
        style.Colors[(int)ImGuiCol.TextDisabled] = Colors.TextSecondary;
        style.Colors[(int)ImGuiCol.WindowBg] = Colors.Background;
        style.Colors[(int)ImGuiCol.ChildBg] = Colors.Surface with { W = 0.5f };
        style.Colors[(int)ImGuiCol.PopupBg] = Colors.Surface;
        style.Colors[(int)ImGuiCol.Border] = Colors.Border;
        style.Colors[(int)ImGuiCol.BorderShadow] = Colors.Shadow;
        style.Colors[(int)ImGuiCol.FrameBg] = Colors.Surface;
        style.Colors[(int)ImGuiCol.FrameBgHovered] = LightenColor(Colors.Surface, 0.1f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = LightenColor(Colors.Surface, 0.2f);
        style.Colors[(int)ImGuiCol.TitleBg] = Colors.Surface;
        style.Colors[(int)ImGuiCol.TitleBgActive] = Colors.Primary with { W = 0.9f };
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Colors.Surface with { W = 0.5f };
        style.Colors[(int)ImGuiCol.MenuBarBg] = Colors.Surface;
        style.Colors[(int)ImGuiCol.ScrollbarBg] = Colors.Background;
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = Colors.Secondary with { W = 0.5f };
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = Colors.Secondary with { W = 0.7f };
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = Colors.Secondary;
        style.Colors[(int)ImGuiCol.CheckMark] = Colors.Accent;
        style.Colors[(int)ImGuiCol.SliderGrab] = Colors.Primary;
        style.Colors[(int)ImGuiCol.SliderGrabActive] = Colors.Accent;
        style.Colors[(int)ImGuiCol.Button] = Colors.Primary with { W = 0.8f };
        style.Colors[(int)ImGuiCol.ButtonHovered] = Colors.Primary;
        style.Colors[(int)ImGuiCol.ButtonActive] = Colors.Accent;
        style.Colors[(int)ImGuiCol.Header] = Colors.Primary with { W = 0.3f };
        style.Colors[(int)ImGuiCol.HeaderHovered] = Colors.Primary with { W = 0.5f };
        style.Colors[(int)ImGuiCol.HeaderActive] = Colors.Primary with { W = 0.7f };
        style.Colors[(int)ImGuiCol.Separator] = Colors.Border;
        style.Colors[(int)ImGuiCol.SeparatorHovered] = Colors.Primary with { W = 0.5f };
        style.Colors[(int)ImGuiCol.SeparatorActive] = Colors.Primary;
        style.Colors[(int)ImGuiCol.ResizeGrip] = Colors.Secondary with { W = 0.3f };
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = Colors.Secondary with { W = 0.5f };
        style.Colors[(int)ImGuiCol.ResizeGripActive] = Colors.Secondary with { W = 0.7f };
        style.Colors[(int)ImGuiCol.Tab] = Colors.Surface;
        style.Colors[(int)ImGuiCol.TabHovered] = Colors.Primary with { W = 0.5f };
        style.Colors[(int)ImGuiCol.TabActive] = Colors.Primary;
        style.Colors[(int)ImGuiCol.TabUnfocused] = Colors.Surface with { W = 0.7f };
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] = Colors.Primary with { W = 0.7f };
        style.Colors[(int)ImGuiCol.PlotLines] = Colors.Accent;
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = Colors.Accent with { W = 1f };
        style.Colors[(int)ImGuiCol.PlotHistogram] = Colors.Secondary;
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = Colors.Secondary with { W = 1f };
        style.Colors[(int)ImGuiCol.TableHeaderBg] = Colors.Surface;
        style.Colors[(int)ImGuiCol.TableBorderStrong] = Colors.Border;
        style.Colors[(int)ImGuiCol.TableBorderLight] = Colors.Border with { W = 0.3f };
        style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0, 0, 0, 0);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = Colors.Surface with { W = 0.1f };
        style.Colors[(int)ImGuiCol.TextSelectedBg] = Colors.Primary with { W = 0.3f };
        style.Colors[(int)ImGuiCol.DragDropTarget] = Colors.Accent;
        style.Colors[(int)ImGuiCol.NavHighlight] = Colors.Primary;
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = Colors.Primary with { W = 0.7f };
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = Colors.Background with { W = 0.8f };
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = Colors.Shadow with { W = 0.8f };
    }
    
    /// <summary>
    /// 保存原始ImGui样式
    /// </summary>
    private void SaveOriginalStyle(ImGuiStylePtr style)
    {
        // 保存颜色
        savedColors.Clear();
        for (int i = 0; i < (int)ImGuiCol.COUNT; i++)
        {
            savedColors[(ImGuiCol)i] = style.Colors[i];
        }
    }
    
    /// <summary>
    /// 恢复原始ImGui样式
    /// </summary>
    public void Restore()
    {
        if (!isApplied)
            return;
            
        var style = ImGui.GetStyle();
        
        // 恢复颜色
        foreach (var kvp in savedColors)
        {
            style.Colors[(int)kvp.Key] = kvp.Value;
        }
        
        savedColors.Clear();
        isApplied = false;
    }

    /// <summary>
    /// 绘制带阴影的窗口
    /// </summary>
    public static void DrawShadow(Vector2 pos, Vector2 size, float strength = 0.5f, float spread = 10f)
    {
        var drawList = ImGui.GetWindowDrawList();
        var shadowColor = new Vector4(0, 0, 0, strength);
        
        for (int i = 0; i < 5; i++)
        {
            var offset = i * 2;
            var alpha = shadowColor.W * (1f - i / 5f);
            var color = ImGui.GetColorU32(shadowColor with { W = alpha });
            drawList.AddRectFilled(
                pos - new Vector2(offset),
                pos + size + new Vector2(offset + spread),
                color,
                8f + i
            );
        }
    }

    /// <summary>
    /// 绘制渐变背景
    /// </summary>
    public static void DrawGradient(Vector2 pos, Vector2 size, Vector4 colorTop, Vector4 colorBottom)
    {
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddRectFilledMultiColor(
            pos,
            pos + size,
            ImGui.GetColorU32(colorTop),
            ImGui.GetColorU32(colorTop),
            ImGui.GetColorU32(colorBottom),
            ImGui.GetColorU32(colorBottom)
        );
    }

    /// <summary>
    /// 绘制发光边框
    /// </summary>
    public static void DrawGlowBorder(Vector2 pos, Vector2 size, Vector4 color, float thickness = 2f, float glow = 4f)
    {
        var drawList = ImGui.GetWindowDrawList();
        
        // 外发光
        for (int i = 0; i < 3; i++)
        {
            var alpha = color.W * (0.3f - i * 0.1f);
            var glowColor = color with { W = alpha };
            drawList.AddRect(
                pos - new Vector2(i * glow),
                pos + size + new Vector2(i * glow),
                ImGui.GetColorU32(glowColor),
                8f,
                ImDrawFlags.None,
                thickness + i * 2
            );
        }
        
        // 主边框
        drawList.AddRect(pos, pos + size, ImGui.GetColorU32(color), 8f, ImDrawFlags.None, thickness);
    }

    /// <summary>
    /// 颜色变亮
    /// </summary>
    private static Vector4 LightenColor(Vector4 color, float amount)
    {
        return new Vector4(
            Math.Min(1f, color.X + amount),
            Math.Min(1f, color.Y + amount),
            Math.Min(1f, color.Z + amount),
            color.W
        );
    }

    /// <summary>
    /// 颜色变暗
    /// </summary>
    private static Vector4 DarkenColor(Vector4 color, float amount)
    {
        return new Vector4(
            Math.Max(0f, color.X - amount),
            Math.Max(0f, color.Y - amount),
            Math.Max(0f, color.Z - amount),
            color.W
        );
    }

    /// <summary>
    /// 混合颜色
    /// </summary>
    public static Vector4 BlendColor(Vector4 color1, Vector4 color2, float ratio)
    {
        return new Vector4(
            color1.X * (1f - ratio) + color2.X * ratio,
            color1.Y * (1f - ratio) + color2.Y * ratio,
            color1.Z * (1f - ratio) + color2.Z * ratio,
            color1.W * (1f - ratio) + color2.W * ratio
        );
    }
}