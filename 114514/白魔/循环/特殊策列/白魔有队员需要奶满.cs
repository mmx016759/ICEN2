using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;
using icen.common;
using icen.数据;
using icen.白魔.View.QT;
using icen.白魔.技能数据;
namespace icen.白魔.循环.特殊策列;

public class 白魔有队员需要奶满 : ISlotSequence
{
        
    public List<Action<Slot>> Sequence { get; } = [Step0, Step1, Step2];

    private static IBattleChara target;

    public int StartCheck()
    {
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (地图.高难地图.Contains(Helper.当前地图id))
        {
            return -1;
        }

        if (Helper.队员是否拥有BUFF(状态.塞壬的歌声) && Helper.当前地图id == 地图.领航明灯天狼星灯塔)
        {
            if (Helper.获取拥有buff队员(状态.塞壬的歌声).CurrentHpPercent() < 100)
            {
                target = Helper.获取拥有buff队员(状态.塞壬的歌声);
                return 1;
            }
        }

        if (Helper.队员是否拥有BUFF(状态.死亡宣告_210) && Helper.当前地图id == 地图.武装圣域放浪神古神殿)
        {
            if (Helper.获取拥有buff队员(状态.死亡宣告_210).CurrentHpPercent() < 100)
            {
                target = Helper.获取拥有buff队员(状态.死亡宣告_210);
                return 1;
            }
        }

        if (Helper.队员是否拥有BUFF(状态.死亡宣告_1769) && Helper.当前地图id == 地图.乐欲之所瓯博讷修道院)
        {
            if (Helper.获取拥有buff队员(状态.死亡宣告_1769).CurrentHpPercent() < 100)
            {
                target = Helper.获取拥有buff队员(状态.死亡宣告_1769);
                return 1;
            }
        }
            
        if (Helper.队员是否拥有BUFF(状态.渐渐石化))
        {
            if (Helper.获取拥有buff队员(状态.渐渐石化).CurrentHpPercent() < 100)
            {
                target = Helper.获取拥有buff队员(状态.渐渐石化);
                return 1;
            }
        }

        return -1;
    }

    public int StopCheck(int index)
    {
        return -1;
    }

    private static bool needNext;

    private static void Step0(Slot slot)
    {
        needNext = false;

        if (target.Distance(Helper.自身) > 30)
        {
            return;
        }

        if (白魔Qt.GetQt("天赐"))
        {

            if (技能id.天赐祝福.GetSpell().IsReadyWithCanCast())
            {
                slot.Add(new Spell(技能id.天赐祝福, target));
                return;
            }
        }

        if (白魔Qt.GetQt("神名"))
        {
            if (技能id.神名.GetSpell().IsReadyWithCanCast())
            {
                slot.Add(new Spell(技能id.神名, target));
                return;
            }

        }

        if (白魔Qt.GetQt("GCD单体治疗"))
        {

            if (技能id.安慰之心.GetSpell().IsReadyWithCanCast())
            {
                slot.Add(new Spell(技能id.安慰之心, target));
                return;
            }

            if (技能id.安慰之心.GetSpell().IsReadyWithCanCast())
            {
                slot.Add(new Spell(技能id.安慰之心, target));
            }

        }

    }

    private static void Step1(Slot slot)
    {
        if (!needNext) return;
        if (技能id.无中生有.GetSpell().IsReadyWithCanCast())
        {
            slot.Add(new Spell(技能id.无中生有, SpellTargetType.Self));
        }

    }

    private static void Step2(Slot slot)
    {
        if (!needNext) return;
        if (Helper.自身是否在移动() && !Helper.自身存在Buff(状态.即刻咏唱)) return;
        if (Helper.自身当前等级 >= 30)
        {
            slot.Add(new Spell(技能id.救疗, target));
            return;
        }


        slot.Add(new Spell(技能id.治疗, target));

    }


}