using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;

namespace ICEN2.白魔.循环.能力技;

public class 神爱抚 : ISlotResolver
{
    public int Check()
    {
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!技能id.神爱抚.GetSpell().IsReadyWithCanCast()) return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.神爱抚.GetSpell());
    }
}