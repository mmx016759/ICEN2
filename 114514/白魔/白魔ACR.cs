using AEAssist.CombatRoutine;
using ICEN2.白魔.Main;
using ICEN2.白魔.界面;
using ICEN2.白魔.设置.设置;

// ReSharper disable UnusedType.Global

namespace ICEN2.白魔;

public class 白魔ACR : IRotationEntry
{
    public string AuthorName { get; set; } = "ICEN2";
    
    public Rotation Build(string settingFolder)
    {
        WhiteMageACR.Init(settingFolder);
        return WhiteMageACR.Build();
    }
    
    public IRotationUI GetRotationUI()
    {
        return baseUI.UI;
    }
    
    public void OnDrawSetting()
    {
        WhiteMageSettingsUI.实例.Draw();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}