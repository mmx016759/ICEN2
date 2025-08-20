using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;

namespace ICEN2.白魔.循环.能力技;

public class 无中生有 : ISlotResolver
{
    public int Check()
    {
        if(Helper.充能层数(技能id.无中生有) < 2) return -9;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (白魔Qt.GetQt("停手")) return -1;
        if (!技能id.无中生有.IsMaxChargeReady(0)) return -4;
        if (!技能id.无中生有.GetSpell().IsReadyWithCanCast()) return -3;
        if (Helper.自身蓝量 > 2400 && (Helper.GCD剩余时间() <= 300 || Helper.GCD可用状态())) return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.无中生有.GetSpell());
    }
}