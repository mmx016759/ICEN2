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

namespace icen.白魔.循环.能力技;

public class 天赐 : ISlotResolver
{
    private IBattleChara? target;

    public int Check()
    {
        if (!白魔Qt.GetQt("单奶")) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (!技能id.天赐祝福.GetSpell().IsReadyWithCanCast()) return -114514;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if(Core.Me.Level < 50) return -999;

        if (!白魔Qt.GetQt("天赐")) return -1;
        if (Core.Resolve<MemApiMove>().IsMoving())
            return -2;
        //获取符合血量阈值的人
        var list = PartyHelper.CastableAlliesWithin30
            .Concat([Core.Me])
            .Where(ally => ally.CurrentHpPercent() <= 默认值.实例.天赐血量 / 100f)
            .ToList();
        var target = list
            .OrderBy(ally => ally.CurrentHpPercent())
            .FirstOrDefault();
        if (target == null) return -4;
        
        //活死人不给
        if (target.HasAura(状态.行尸走肉)) return -5;
        if (target.HasAura(状态.出生入死)) return -5;
        if (target.HasAura(状态.死斗)) return -5;

        if (target != null)
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
            slot.Add(new Spell(技能id.天赐祝福, Helper.获取血量最低成员));
    }
}