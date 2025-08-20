using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using icen.common;
using icen.数据;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.GCD;

public class 康复 : ISlotResolver
{
    public int Check()
    {
        if (!白魔Qt.GetQt("读条奶")) return -1;
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!技能id.康复.GetSpell().IsReadyWithCanCast()) return -2;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("康复")) return -6;
        if (!Helper.队员是否拥有可驱散状态()) return -7;
        if (Helper.是否拥有其中的BUFF(Helper.获取可驱散队员(),状态.不可选中))  return -8;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(技能id.康复, Helper.获取可驱散队员));
    }
}