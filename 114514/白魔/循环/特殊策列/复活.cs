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
using WhiteMageSettingsUI = icen.白魔.Utilities.标签.WhiteMageSettingsUI;

namespace icen.白魔.循环.特殊策列;

public class 即刻复活 : ISlotSequence
{
    public List<Action<Slot>> Sequence { get; } = [Step0, Step1, Step2];
    private static IBattleChara? target;
    private static int targetPosition = -1;
    public int StartCheck()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (地图.不拉人地图.Contains(Helper.当前地图id)) return -666;
        if (!技能id.即刻咏唱.GetSpell().IsReadyWithCanCast()) return -3;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("拉人")) return -5;
        if (!技能id.无中生有.GetSpell().IsReadyWithCanCast()&& Helper.自身蓝量 < 2400) return -7;
        if (WhiteMageBattleData.技能内置cd.TryGetValue("复活内置cd", out var lastUserTime))
        {
            if (Helper.GetTimeStamps() - lastUserTime < 1000)
                return -12;
        }
        
        target = Helper.没有复活buff的死亡队友_时间();
        if (Helper.当前地图id!=1252)
        {
            if (默认值.实例.优先TN)
            {
                target = Helper.没有复活buff的死亡队友_时间_优先坦克和治疗();
            }
            else
            {
                target = Helper.没有复活buff的死亡队友_时间();
            }
        }
        else
        {
            target = Helper.没有复活状态的死亡玩家();
        }
        if (target == null || !target.IsValid()) return -8;
        if (target.Distance(Helper.自身) > 30) return -9;
        if (Helper.目标是否拥有BUFF(状态.限制复活)) return -11;
        if (!target.IsTargetable) return -12;
        if (!技能id.复活.GetSpell(target).IsReadyWithCanCast()) return -2;

        if (WhiteMageBattleData.技能内置cd.ContainsKey("复活内置cd"))
            WhiteMageBattleData.技能内置cd["复活内置cd"] = Helper.GetTimeStamps();
        else
            WhiteMageBattleData.技能内置cd.TryAdd("复活内置cd", Helper.GetTimeStamps());

        return 1;
    }

    public int StopCheck(int index) => -1;

    private static void Step0(Slot slot)
    {
        if (target == null || !target.IsValid()) return;
        slot.Add(new Spell(技能id.即刻咏唱, SpellTargetType.Self));
    }

    private static void Step1(Slot slot)
    {
        
        if (技能id.无中生有.GetSpell().IsReadyWithCanCast())
            slot.Add(new Spell(技能id.无中生有, SpellTargetType.Self));
    }


    private static void Step2(Slot slot)
    {
        if (target == null || !target.IsValid()) return;
        LogHelper.Print("七夏白魔ACR", $"复活队友:{target.Name}{target.CurrentJob()}");
        slot.Add(new Spell(技能id.复活, target));
        var ing= Helper.自身目标;
        ApiHelper.设置目标(target);
        
        // 添加复活提醒宏功能
        if (默认值.实例.复活提醒&&Helper.当前地图id!=1252)
        {
            // 使用当前目标的名称（转换为字符串）
            string targetName = "<t>";
        
            // 尝试获取随机复活宏（现在返回字符串数组）
            if (WhiteMageSettingsUI.实例.TryGetRandomReviveMacro(targetName, out string[] macros))
            {
                // 逐行发送宏内容
                foreach (string macroLine in macros)
                {
                    ChatHelper.SendMessage(macroLine);
                }
            }
            else
            {
                // 如果没有设置复活宏，显示提示消息
                Core.Resolve<MemApiChatMessage>().Toast2("你没有设置复活宏\n请设置复活宏！", 1, 10000);
            }
        }
        ApiHelper.设置目标(ing);
    }
}