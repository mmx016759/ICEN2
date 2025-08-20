using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using icen.common;
using icen.数据;
using icen.白魔.Utilities.设置;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.GCD;

public class 狂喜之心 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (!技能id.狂喜之心.GetSpell().IsReadyWithCanCast()) return -2;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1; 
        if (!白魔Qt.GetQt("狂喜之心")) return -1;
        if (!白魔Qt.GetQt("群奶")) return -1;
        if (Core.Resolve<MemApiMove>().IsMoving()) return -3;
        //判断团血
        if (PartyHelper.CastableAlliesWithin15
                .Concat([Core.Me])
                .Count(ally =>
                    ally.CurrentHpPercent() <= 默认值.实例.狂喜之心血量 / 100f) >= 默认值.实例.团血检测人数)
            return 2;
        if (Helper.自身当前等级 > 73 && Helper.白魔量谱_蓝花数量() > 2&&默认值.实例.蓝花防溢出) return 1;
        return -3;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.狂喜之心.GetSpell());
}
}
