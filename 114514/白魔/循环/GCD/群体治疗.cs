using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.循环.GCD;

public class 群体治疗 : ISlotResolver
{
    public int Check()
    {
        if (!白魔Qt.GetQt("读条奶")) return -1;
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("群奶")) return 1;
        if (!技能id.医治.GetSpell().IsReadyWithCanCast()) return -2;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("群奶")) return -1;
        if(Core.Me.Level < 10) return -999;
        if (Core.Resolve<MemApiMove>().IsMoving())
            return -3;
        //判断团血
        if (PartyHelper.CastableAlliesWithin15
                .Concat([Core.Me])
                .Count(ally =>
                    ally.CurrentHpPercent() <= 默认值.实例.愈疗医治血量 / 100f) >= 默认值.实例.团血检测人数)
            return 0;

        return -3;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.医治.GetSpell());
    }
}