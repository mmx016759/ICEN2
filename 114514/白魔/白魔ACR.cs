using AEAssist.CombatRoutine;
using icen.白魔.Main;
using icen.白魔.Utilities;
using icen.白魔.Utilities.设置;
using icen.白魔.View;

// ReSharper disable UnusedType.Global

namespace icen.白魔;

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