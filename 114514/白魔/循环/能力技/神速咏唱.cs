using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;

namespace ICEN2.白魔.循环.能力技;

public class 神速咏唱 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!白魔Qt.GetQt("神速咏唱")) return -8;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!技能id.神速咏唱.GetSpell().IsReadyWithCanCast()) return -6;
        if (Helper.目标血量百分比<0.0001)  return -55;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.神速咏唱.GetSpell());
    }
}