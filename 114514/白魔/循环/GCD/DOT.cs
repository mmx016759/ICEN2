using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using icen.common;
using icen.数据;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.GCD;

public class DOt : ISlotResolver
{
    
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (白魔Qt.GetQt("停手")) return -1;
        if (!白魔Qt.GetQt("Dot")) return -8;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法造成伤害)) return -1;
        if (Helper.目标是否拥有其中的BUFF(BuffBlackList.敌人不可被攻击表)) return -3;
        if (Helper.目标到自身距离()>22+SettingMgr.GetSetting<GeneralSettings>().AttackRange) return -55;
        if (Helper.目标血量百分比<0.0001)  return -55;
        if (Helper.指定buff剩余时间是否大于(Helper.自身目标, 状态.疾风,5000) || Helper.指定buff剩余时间是否大于(Helper.自身目标, 状态.烈风, 5000) ||
            Helper.指定buff剩余时间是否大于(Helper.自身目标, 状态.天辉, 5000)) return -1;
        if (Core.Resolve<MemApiDuty>().InBossBattle) return 1;
        if (地图.四人讨伐地图.Contains(Helper.当前地图id)) return 1;
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(Helper.GetActionChange(技能id.DOT).GetSpell());
    }
}