using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.AILoop;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.设置;

public class 事件处理器 : IRotationEventHandler
{
    /// <summary>非战斗情况下的回调 例如远敏可以考虑此时唱跑步歌 T可以考虑切姿态</summary>
    public async Task OnPreCombat()
    {
        var target = Helper.获取最低血量T();
        if (ApiHelper.是坦克(Helper.获取最低血量T())&&
            ApiHelper.过场动画中(target as IPlayerCharacter)&&
            Helper.是否在副本中()&&默认值.实例.非战斗再生&&白魔Qt.GetQt("再生")&&!白魔Qt.GetQt("停手")&&技能id.再生.GetSpell().IsReadyWithCanCast()&&!Helper.自身存在其中Buff(BuffBlackList.无法发动技能)&&!Helper.是否拥有其中的BUFF(Helper.获取最低血量T(),状态.再生5)&&!(Helper.队伍成员数量>4))
        {
            var slot = new Slot();
            slot.Add(new Spell(技能id.再生, Helper.获取最低血量T));
            slot.Run(AI.Instance.BattleData, false);
        }
    }
    /// <summary>在战斗重置(一般时团灭重来,脱战等)时触发</summary>
    public void OnResetBattle()
    {

        WhiteMageBattleData.Instance = new();
    }

    /// <summary>ACR默认再没目标时是不工作的 为了兼容没目标时的处理 比如舞者在转阶段可能要提前跳舞</summary>
    public async Task OnNoTarget()
    {
        var list = PartyHelper.CastableAlliesWithin30
            .Concat([Core.Me])
            .Where(ally => ally.CurrentHpPercent() <= 默认值.实例.救疗治疗血量 / 100f)
            .ToList();
        var target = list
            .OrderBy(ally => ally.CurrentHpPercent())
            .FirstOrDefault();
        if (!白魔Qt.GetQt("停手")
            && !白魔Qt.GetQt("群奶")
            &&!白魔Qt.GetQt("读条奶") &&
            !Helper.自身是否在移动()
            && !Helper.自身存在其中Buff(BuffBlackList.无法发动技能)
            && 技能id.医治.GetSpell().IsReadyWithCanCast() &&
            PartyHelper.CastableAlliesWithin15
                .Concat([Core.Me])
                .Count(ally =>
                    ally.CurrentHpPercent() <= 默认值.实例.狂喜之心血量 / 100f) >= 默认值.实例.团血检测人数)
        

    {
            var slot = new Slot();
            slot.Add(技能id.医治.GetSpell());
            await slot.Run(AI.Instance.BattleData, false);
        }
        if (!白魔Qt.GetQt("停手")&&!白魔Qt.GetQt("单奶")&&!白魔Qt.GetQt("读条奶")&&技能id.单奶.GetSpell().IsReadyWithCanCast()&&!Helper.自身是否在移动()&&!Helper.自身存在其中Buff(BuffBlackList.无法发动技能)&&target != null)
        {
            var slot = new Slot();
            slot.Add(new Spell(技能id.单奶, Helper.获取血量最低成员));
            await slot.Run(AI.Instance.BattleData, false);
        }
        if (!白魔Qt.GetQt("停手")&&白魔Qt.GetQt("再生")&&技能id.再生.GetSpell().IsReadyWithCanCast()&&!Helper.自身存在其中Buff(BuffBlackList.无法发动技能)&&!Helper.是否拥有其中的BUFF(Helper.获取最低血量T(),状态.再生5)&&!(Helper.队伍成员数量>4))
        {
            var slot = new Slot();
            slot.Add(new Spell(技能id.再生, Helper.获取最低血量T));
            slot.Run(AI.Instance.BattleData, false);
        }
        if (!白魔Qt.GetQt("停手") && 白魔Qt.GetQt("安慰之心") &&Helper.白魔量谱_红花数量()<3 && !Helper.自身存在其中Buff(BuffBlackList.无法发动技能) &&Helper.白魔量谱_蓝花数量()>默认值.实例.保留蓝花数量 && Helper.自身当前等级 > 73&&白魔Qt.GetQt("停手卸蓝花"))
        {
            var slot = new Slot();
            slot.Add(new Spell(技能id.安慰之心, Helper.自身));
            slot.Run(AI.Instance.BattleData, false);
        }
        await Task.CompletedTask;
    }
    /// <summary>读条技能读条判定成功 (读条快结束 可以滑步的时间点)</summary>
    public void OnSpellCastSuccess(Slot slot, Spell spell)
    {

    }
    /// <summary>
    ///     某个技能使用之后的处理,比如诗人在刷Dot之后记录这次是否是强化buff的Dot
    /// 如果是读条技能，则是服务器判定它释放成功的时间点，比上面的要慢一点
    /// </summary>
    /// <param name="slot">这个技能归属的Slot</param>
    /// <param name="spell">某个使用完的技能</param>
    public void AfterSpell(Slot slot, Spell spell)
    {

    }
    /// <summary>战斗中每帧都会触发的逻辑</summary>
    /// <param name="currTimeInMs">从战斗开始到现在的时间,单位毫秒(ms)</param>
    public void OnBattleUpdate(int currTimeInMs)
    {

    }
    /// <summary>切到当前ACR</summary>
    public void OnEnterRotation()
    {
        Core.Resolve<MemApiChatMessage>().Toast2("欢迎使用七夏白魔ACR\n日随高难支持\n有任何问题都可以DC反馈\n有其他想法也可以反馈\n更新时间：2025.8.9", 1, 10000);
        默认值.实例.HLBU = true;
    }
    /// <summary>从当前ACR退出</summary>
    public void OnExitRotation()
    {
        Core.Resolve<MemApiChatMessage>().Toast2("再见", 1, 1000);
        默认值.实例.HLBU = false;
        默认值.实例.Save();
    }
    /// <summary>地图切换时触发</summary>
    public void OnTerritoryChanged()
    {

    }


}