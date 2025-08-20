using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置;

namespace ICEN2.白魔.循环.GCD;

public class 再生 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!白魔Qt.GetQt("单奶")) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (!技能id.再生.GetSpell().IsReadyWithCanCast()) return -2;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("再生")) return -1;
        if (Helper.自身蓝量 < 400) return -3;
        if (Helper.队伍成员数量 != 4) return -4;
        if (PartyHelper.CastableTanks.Count != 1) return -5;
        if (PartyHelper.CastableTanks[0].HasAura(状态.再生)) return -6;
        if (Helper.目标战斗状态(PartyHelper.CastableTanks[0]) || Helper.自身.InCombat()) return -7;
        if (Helper.自身.Distance(PartyHelper.CastableTanks[0]) > 30) return -8;
        if (!Core.Resolve<MemApiDuty>().InMission) return -9;
        if (Core.Resolve<MemApiDuty>().IsOver) return -10;
        if (WhiteMageBattleData.技能内置cd.TryGetValue(
                技能id.再生 + PartyHelper.CastableTanks[0].GameObjectId.ToString(), out var lastUserTime))
        {
            if (Helper.GetTimeStamps() - lastUserTime < 1000)
            {
                return -12;
            }

            WhiteMageBattleData.技能内置cd[技能id.再生 + PartyHelper.CastableTanks[0].GameObjectId.ToString()] =
                Helper.GetTimeStamps();
        }
        else
            WhiteMageBattleData.技能内置cd.TryAdd(技能id.再生 + PartyHelper.CastableTanks[0].GameObjectId.ToString(),
                Helper.GetTimeStamps());

        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(技能id.再生, PartyHelper.CastableTanks[0]));
    }
}