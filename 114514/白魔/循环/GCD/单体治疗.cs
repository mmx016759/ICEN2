using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using icen.common;
using icen.数据;
using icen.白魔.Utilities;
using icen.白魔.Utilities.设置;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.GCD;

public class 单体治疗 : ISlotResolver
{
    private static IBattleChara _target;

    public int Check()
    {
        if (!白魔Qt.GetQt("读条奶")) return -1;
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("单奶")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("单奶")) return -1;
        if (Core.Resolve<MemApiMove>().IsMoving())
            return -2;
        //获取符合血量阈值的人
        var list = PartyHelper.CastableAlliesWithin30
            .Concat([Core.Me])
            .Where(ally => ally.CurrentHpPercent() <= 默认值.实例.救疗治疗血量 / 100f)
            .ToList();
        var target = list
            .OrderBy(ally => ally.CurrentHpPercent())
            .FirstOrDefault();
        if (target == null) return -4;
        
        if (Helper.上一个能力技能() == 技能id.天赐祝福 && target.GameObjectId == Core.Resolve<MemApiSpellCastSuccess>().LastTarget?.GameObjectId) return -3;
        
        //活死人不给
        if (target.HasAura(状态.行尸走肉)) return -5;
        if (target.HasAura(状态.出生入死)) return -5;
        if (target.HasAura(状态.死斗)) return -5;
        {
            if (WhiteMageBattleData.技能内置cd.TryGetValue("上次奶这个人的时间" + Helper.获取血量最低成员().GameObjectId , out var lastUserTime))
            {
                if (Helper.GetTimeStamps() - lastUserTime < 1000)
                {
                    return -12;
                }
                WhiteMageBattleData.技能内置cd["上次奶这个人的时间" + Helper.获取血量最低成员().GameObjectId] = Helper.GetTimeStamps();
            }
            else
            {
                WhiteMageBattleData.技能内置cd.TryAdd("上次奶这个人的时间" + Helper.获取血量最低成员().GameObjectId, Helper.GetTimeStamps());
            }   
        }
            
        return 1;
    }

    public void Build(Slot slot)
    {
        默认值.实例.读条 = 0;
        slot.Add(new Spell(技能id.单奶, Helper.获取血量最低成员));
    }
}