using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using icen.common;
using icen.数据;
using icen.白魔.View.QT;
using icen.白魔.技能数据;

namespace icen.白魔.循环.能力技;

public class 庇护所 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!白魔Qt.GetQt("群奶")) return -1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("庇护所")) return -1;
        if (!白魔Qt.GetQt("减伤")) return -8;
        if (!技能id.庇护所.GetSpell().IsReadyWithCanCast()) return -2;
        if (Helper.自身存在Buff(状态.庇护所) || Helper.自身存在Buff(状态.节制)) return -5;
        if (Helper.目标是否准备放aoe(Helper.自身目标, 5)) return 6;
        int count50 = Helper.自身周围单位数量(50);
        int count25 = Helper.自身周围单位数量(50);


        if (Helper.获取目标血量百分比(Helper.获取最低血量T())> 0.8f&&Helper.自身周围单位数量(25)<5) return -3;
        if (Helper.十米视线内血量低于设定的队员数量(0.95f) < 3) return -2;
        if (Math.Abs(count50 - count25) <= 2 && count25 > 5) return 1;

        return -1;
    }

    public void Build(Slot slot)
    {
        if (Helper.自身目标的目标 != null)
        {
            slot.Add(new Spell(技能id.庇护所, Helper.自身目标的目标));
        }
    }
}