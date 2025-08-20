using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using icen.common;
using icen.数据;
using icen.白魔.Utilities.设置;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.GCD;

public class 医济 : ISlotResolver
{
    public int Check()
    {
        if (!白魔Qt.GetQt("读条奶")) return -1;
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("群奶")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("群奶")) return -1;
        if (!技能id.群奶.GetSpell().IsReadyWithCanCast()) return -2;
        if (技能id.群奶.技能是否刚使用过(5000)) return -5;
        if (Helper.自身存在Buff大于时间([状态.医济], 3000) || Helper.自身存在Buff大于时间([状态.医养], 3000)) return -5; 
        if (Core.Resolve<MemApiMove>().IsMoving())
            return -3;
        //判断团血
        if (PartyHelper.CastableAlliesWithin20
                .Concat([Core.Me])
                .Count(ally =>
                    ally.CurrentHpPercent() <= 默认值.实例.医济血量 / 100f) >= 默认值.实例.团血检测人数)
            return 0;
        return -3;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.群奶.GetSpell(Helper.自身));
    }
}