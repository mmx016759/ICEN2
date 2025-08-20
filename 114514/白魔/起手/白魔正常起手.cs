using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

#pragma warning disable CS8618
namespace ICEN2.白魔.起手
{
    public class 白魔正常起手 : IOpener
    {
        public List<Action<Slot>> Sequence { get; } = [Step0, Step1, Step2];


        public int StartCheck()
        {

            if (Helper.队伍成员数量 < 8 && !Helper.目标是否为假人())
            {
                return -100;
            }

            return 0;
        }

        public int StopCheck(int index)
        {
            return -1;
        }



        public void InitCountDown(CountDownHandler countDownHandler)
        {
            if (技能id.再生.IsReady())
            {
                countDownHandler.AddAction(默认值.实例.起手挂再生时间, 技能id.再生, SpellTargetType.Pm2);
            }
            if (白魔Qt.GetQt("爆发药"))
            {
                countDownHandler.AddPotionAction(默认值.实例.起手嗑药时间);
            }
            countDownHandler.AddAction(默认值.实例.起手预读时间, Helper.获取会变化的技能(技能id.单体gcd).Id, SpellTargetType.Target);
        }



        private static void Step0(Slot slot)
        {
            if (白魔Qt.GetQt("DOT"))
            {
                slot.Add(new Spell(Helper.获取会变化的技能(技能id.DOT).Id, SpellTargetType.Target));
            }

            if (白魔Qt.GetQt("法令") && 技能id.法令.IsReady())
            {
                slot.Add(new Spell(技能id.法令, SpellTargetType.Target));
            }
        }

        private static void Step1(Slot slot)
        {
            slot.Add(new Spell(Helper.获取会变化的技能(技能id.单体gcd).Id, SpellTargetType.Target));

            if ( 技能id.即刻咏唱.IsReady())
            {
                slot.Add(new Spell(技能id.即刻咏唱, SpellTargetType.Self));
            }
        }

        private static void Step2(Slot slot)
        {
            slot.Add(new Spell(Helper.获取会变化的技能(技能id.单体gcd).Id, SpellTargetType.Target));

            if ( 白魔Qt.GetQt("法令") && 技能id.法令.IsReady())
            {
                slot.Add(new Spell(技能id.法令, SpellTargetType.Target));
            }

            if (白魔Qt.GetQt("神速咏唱") && 技能id.神速咏唱.IsReady())
            {
                slot.Add(new Spell(技能id.神速咏唱, SpellTargetType.Self));
            }

        }


    }
}
