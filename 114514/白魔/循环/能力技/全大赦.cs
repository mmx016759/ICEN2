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

namespace icen.白魔.循环.能力技;

public class 全大赦 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("全大赦")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!技能id.全大赦.GetSpell().IsReadyWithCanCast()) return -2;
        if (!白魔Qt.GetQt("全大赦")) return -3;
        if (Core.Resolve<MemApiMove>().IsMoving())
            return -4;
        if (Core.Resolve<JobApi_WhiteMage>().Lily<= 0) return -5;
        //判断团血
        if (PartyHelper.CastableAlliesWithin15
                .Concat([Core.Me])
                .Count(ally =>
                    ally.CurrentHpPercent() <= 默认值.实例.插入全大血量 / 100f) >= 默认值.实例.团血检测人数)
            return 0;

        return -6;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.全大赦.GetSpell());
    }
}