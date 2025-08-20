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
using ICEN2.白魔.设置;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.循环.能力技;

public class 神明 : ISlotResolver
{
    private static IBattleChara _target;

    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("神名")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("单奶")) return -1;
        if (!白魔Qt.GetQt("神名")) return -1;
        if (!技能id.神名.GetSpell().IsReadyWithCanCast()) return -2;
        var list = PartyHelper.CastableAlliesWithin30
            .Concat([Core.Me])
            .Where(ally => ally.CurrentHpPercent() <= 默认值.实例.神名血量 / 100f)
            .ToList();
        if (list.Count > 2) return -3; //超过两个人就不单奶
        var target = list
            .OrderBy(ally => ally.CurrentHpPercent())
            .FirstOrDefault();
        if (target == null) return -4;
        
        //活死人不给
        //活死人不给
        if (target.HasAura(状态.行尸走肉)) return -5;
        if (target.HasAura(状态.出生入死)) return -5;
        if (target.HasAura(状态.死斗)) return -5;
        if (target != null)
        {
            if (WhiteMageBattleData.技能内置cd.TryGetValue("上次奶这个人的时间" + target.GameObjectId , out var lastUserTime))
            {
                if (Helper.GetTimeStamps() - lastUserTime < 1000)
                {
                    return -12;
                }
            }
            if (WhiteMageBattleData.技能内置cd.ContainsKey("上次奶这个人的时间" + target.GameObjectId))
            {
                WhiteMageBattleData.技能内置cd["上次奶这个人的时间" + target.GameObjectId] = Helper.GetTimeStamps();
            }
            else
            {
                WhiteMageBattleData.技能内置cd.TryAdd("上次奶这个人的时间" + target.GameObjectId, Helper.GetTimeStamps());
            }   
        }
        _target = target;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(技能id.神名, _target));
    }
}