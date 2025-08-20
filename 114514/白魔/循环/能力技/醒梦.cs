using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using icen.common;
using icen.数据;
using icen.白魔.Utilities.设置;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.能力技;

public class 醒梦 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("醒梦")) return -1;
        if (Core.Me.IsCasting) return -1;
        if (!技能id.醒梦.GetSpell().IsReadyWithCanCast()) return -2;
        if (AI.Instance.BattleData.HighPrioritySlots_OffGCD.Count > 0) return -2;
        if (Core.Me.CurrentMpPercent() > 默认值.实例.醒梦 / 100f) return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(技能id.醒梦.GetSpell());
    }
}