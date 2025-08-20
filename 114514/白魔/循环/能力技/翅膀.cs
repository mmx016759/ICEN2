using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;

namespace ICEN2.白魔.循环.能力技;

public class 翅膀 : ISlotResolver
{
    public int Check()
    {
        int count50 = Helper.自身周围单位数量(50);
        int count25 = Helper.自身周围单位数量(50);
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!白魔Qt.GetQt("减伤")) return -8;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!技能id.节制.GetSpell().IsReadyWithCanCast()) return -6;
        if (Helper.自身存在Buff(状态.节制)) return -5;
        if (Helper.自身是否在移动()) return -4;
        if (Helper.获取目标血量百分比(Helper.获取最低血量T())> 0.8f&&Helper.自身周围单位数量(25)>5) return -3;
        if (Math.Abs(count50 - count25) <= 5 && count25 > 7) return 1;
        
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.节制.GetSpell());
    }
}