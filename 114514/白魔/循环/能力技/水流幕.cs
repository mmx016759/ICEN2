using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;

namespace ICEN2.白魔.循环.能力技;

public class 水流幕 : ISlotResolver
{
    public int Check()
    {
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("水流幕")) return -1;
        if (!白魔Qt.GetQt("减伤")) return -1;
        if (Helper.自身是否在移动()) return -4;
        if (!技能id.水流幕.GetSpell().IsReadyWithCanCast()) return -2;
        if (DeathSentenceHelper.IsDeathSentence(Helper.自身目标)) return 1;
        if (Helper.自身周围单位数量(20)>5&&Helper.获取目标血量百分比(Helper.获取最低血量T())< 0.8f) return 0;
        return -1;
    }

    public void Build(Slot slot)
    {
        if (Helper.自身目标的目标 != null)
        {
            slot.Add(new Spell(技能id.水流幕, Helper.获取最低血量T));
        }
    }
}