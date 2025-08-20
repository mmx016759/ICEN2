using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.循环.能力技;

public class 神祝祷 : ISlotResolver
{
    private static IBattleChara _target;

    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("神祝祷")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("单奶")) return -1;
        if (!白魔Qt.GetQt("神祝祷")) return -1;
        if (技能id.神祝祷.技能是否刚使用过(5000)) return -5;
        if (!技能id.神祝祷.GetSpell().IsReadyWithCanCast()) return -2;
        var list = PartyHelper.CastableAlliesWithin30
            .Concat([Core.Me])
            .Where(ally => ally.CurrentHpPercent() <= 默认值.实例.神祝祷血量 / 100f)
            .ToList();
        var target = list
            .OrderBy(ally => ally.CurrentHpPercent())
            .FirstOrDefault();
        if (target == null) return -4;
        
        //活死人不给
        if (target.HasAura(状态.死而不僵)) return -5;
        if (target.HasAura(状态.出生入死)) return -5;
        if (target.HasAura(状态.死斗)) return -5;
        if (target.HasAura(状态.行尸走肉)) return -5;
        if (Helper.是否拥有其中的BUFF(target,[状态.神祝祷])) return -1;
        _target = target;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(技能id.神祝祷, _target));
    }
}