using System.Numerics;
using AEAssist.IO;
using icen.utils.JobView;
using icen.utils.JobView.HotKey;
using icen.白魔.技能数据;

namespace icen.白魔.Utilities.设置;

public class 默认值
{
    public static 默认值 实例;
    #region 标准模板代码 可以直接复制后改掉类名即可
    private static string path;
    public static void Build(string settingPath)
    {
        path = Path.Combine(settingPath,nameof(默认值), "Icen.json");
        if (!File.Exists(path))
        {
            实例 = new 默认值();
            实例.Save();
            return;
        }
        try
        {
            实例 = JsonHelper.FromJson<默认值>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            实例 = new();
            //LogHelper.Error(e.ToString());
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion
    public int SgePartnerPanelIconSize = 47;
    public bool SgePartnerPanelShow = true;
    public bool SgePartner营救 = false;
    public int 狂喜之心血量 = 75;
    public int 医济血量 = 65;
    public int 愈疗医治血量 = 45;
    public int 插入全大血量 = 50;
    
    // 单奶设置
    public int 安慰之心血量 = 40;
    public int 救疗治疗血量 = 40;
    public int 天赐血量 = 25;
    public int 神名血量 = 75;
    public int 神祝祷血量 = 70;
    public int 醒梦 = 70;
    public int 低级本救疗治疗血量 = 70;
    public int 打断读条奶 = 40;
    // 群奶人数
    public int 团血检测人数 = 3;
    public bool 流血自动铃铛 = true;
    public bool 非战斗再生 = false;
    public bool 设置 = true;
    // 各种设置默认值
    public bool 蓝花防溢出 = true;
    public bool 优先闪飒 = true;
    public bool 复活提醒 = false;
    public bool 优先TN = false;
    public bool 过路圣人模式 = false;
    public int 保留蓝花数量 = 0;
    public List<HotkeySetting> HotkeySettings = new();
    public int 起手嗑药时间 = 2500;
    public int 起手预读时间 = 1500;
    public int 起手挂再生时间 = 5000;
    public int 读条 = 0;
    public int 起手设置 = 1;
    public List<string> 复活提醒宏列表 = new List<string>();
    public int 复活延迟 = 1;
    public int ICEN = 0;
    public bool 庇护所目标 = true;
    public bool HLBU=false;
    
    public float SettingsButtonWindowX = 10f; // 默认X坐标
    public float SettingsButtonWindowY = 10f; // 默认Y坐标
    
    public JobViewSave JobViewSave { get; set; } = new()
    {
        MainColor = new Vector4(8f / 51f, 0.6784314f, 14f / 51f, 0.8f), 
        AutoReset = true, 
        ShowQT = true, 
        ShowHotkey = true,
        CurrentTheme = ModernTheme.ThemePreset.深色模式  // 确保有默认值
    };
    
    public long Version = 0;
    public bool VersionMessage = true;
    
    
    public bool 调试窗口 = false;
    public Dictionary<string, HotKetSpell> HotKey设置 = new()
    {
        {
            "白冲步", new HotKetSpell("白冲步", 技能id.以太变移, 1)
        },
        {
            "医济/医养自己", new HotKetSpell("医济/医养自己", 技能id.医济, 1)
        },
        {
            "愈疗自己", new HotKetSpell("愈疗自己", 技能id.愈疗, 1)
        },
        {
            "营救最远队友", new HotKetSpell("营救最远队友", 技能id.营救, 12)
        },
        {
            "庇护所自己",new HotKetSpell("庇护所自己",技能id.庇护所,1)
        },
        {
            "复活死亡队员", new HotKetSpell("复活死亡队员", 技能id.复活, 15)
        },
        {
            "天赐祝福血量最低成员", new HotKetSpell("天赐祝福血量最低成员", 技能id.天赐祝福, 13)
        },
  
        {
            "水流幕血量最低T", new HotKetSpell("水流幕血量最低T", 技能id.水流幕, 14)
        },
    };
    
    
}