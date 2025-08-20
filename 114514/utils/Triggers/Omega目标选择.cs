using AEAssist;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.MemoryApi;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ICEN2.utils.Triggers;

public class Omega目标选择 : ITriggerAction
{
    
    public string DisplayName { get; } = "OMEGA/选中M或F";
    public enum Boss
    {
        男人,
        女人,
        索尼后男人,
        索尼后女人,
    }
    public string Remark { get; set; }
    public Boss 选中MF { get; set; }

    public bool Draw()
    {
        return false;
    }

    public bool Handle()
    {
        uint bossId = 选中MF switch
        {
            Boss.女人 => 15713,
            Boss.男人 => 15712,
            Boss.索尼后女人 => 15712,
            Boss.索尼后男人 => 15713,
            _ => 0
        };
        var enemy = TargetMgr.Instance.Units;
        foreach (var v in enemy.Values.Where(v => v.DataId == bossId))
        {
            Core.Resolve<MemApiTarget>().SetTarget(v);
        }
        return true;
    }
}