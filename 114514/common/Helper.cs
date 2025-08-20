using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using ICEN2.数据;
using ICEN2.白魔.设置.设置;

namespace ICEN2.common;

public static class Helper
{

 public static IBattleChara? 没有复活状态的死亡玩家()
    {
        IBattleChara? result = null;
        // 按照从近到远排序玩家
        var sortedObjects = ECHelper.Objects.OfType<IBattleChara>()
            .OrderBy(到自身距离)
            .ToList();

        // 优先级映射
        var partyList = PartyHelper.Party;
        Dictionary<IBattleChara, int> priorityMap = new Dictionary<IBattleChara, int>();

        foreach (var obj in sortedObjects)
        {

            if (!obj.IsDead())
            {
                continue;
            }

            if (!obj.IsTargetable)
            {
                continue;
            }

       
            if (是否拥有BUFF(obj, 状态.限制复活))
            {
                continue;
            }

            if (到自身距离(obj) > 30)
            {
                continue;
            }

            if (obj.HasAura(状态.复活))
            {
                continue;
            }

            if (!Core.Resolve<MemApiSpell>().CheckActionInRangeOrLoS(125u, obj))
            {
                continue;
            }

            if (Core.Me.Position.Y - obj.Position.Y > 1)
            {

                    continue;
            }

            if (!new Spell(125u, obj).IsReadyWithCanCast())
            {
                continue;
            }

            int priority = 7;
            bool isInParty = partyList.Contains(obj);

            if (isInParty)
            {
                if (obj.IsHealer()) priority = 1;
                else if (obj.IsTank()) priority = 2;
                else if (obj.IsDps()) priority = 3;
            }
            else
            {
                if (obj.IsHealer()) priority = 4;
                else if (obj.IsTank()) priority = 5;
                else if (obj.IsDps()) priority = 6;
            }

            priorityMap[obj] = priority;
        }

        if (priorityMap.Count > 0)
        {
            result = priorityMap.OrderBy(pair => pair.Value).First().Key;
        }

        return result;
    }
 
    public static bool 是否拥有BUFF(IBattleChara target, uint id)
    {
        return target.HasAura(id);
    }
 
    /// <summary>
    /// 获取自身buff的剩余时间
    /// </summary>
    /// <param name="buffId"></param>
    /// <returns></returns>
    public static int GetAuraTimeLeft(uint buffId) => Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me, buffId, true);

    /// <summary>显示一个文本提示，用于在游戏中显示简短的消息。</summary>
    /// <param name="msg">要显示的消息文本。</param>
    /// <param name="s">文本提示的样式。支持蓝色提示（1）和红色提示（2）两种</param>
    /// <param name="time">文本提示显示的时间（单位毫秒）。如显示3秒，填写3000即可</param>
    public static void SendTips(string msg, int s = 1, int time = 3000) => Core.Resolve<MemApiChatMessage>()
        .Toast2(msg, s, time);

    public static bool IsMove => MoveHelper.IsMoving();

    /// <summary>
    /// 全局设置
    /// </summary>
    public static GeneralSettings GlobalSettings => SettingMgr.GetSetting<GeneralSettings>();

    /// <summary>
    /// 当前地图id
    /// </summary>
    public static uint GetTerritoyId => Core.Resolve<MemApiMap>().GetCurrTerrId();

    /// <summary>
    /// 返回可变技能的当前id
    /// </summary>
    public static uint GetActionChange(uint spellId) => Core.Resolve<MemApiSpell>().CheckActionChange(spellId);

    /// <summary>
    /// 高优先级插入条件检测函数
    /// </summary>
    public static int HighPrioritySlotCheckFunc(SlotMode mode, Slot slot)
    {
        if (mode != SlotMode.OffGcd) return 1;
        //限制高优先能力技插入，只能在g窗口前半和后半打
        if (GCDHelper.GetGCDCooldown() is > 750 and < 1500) return -1;
        //连续的两个高优先能力技插入，在gcd前半窗口打，以免卡gcd
        if (slot.Actions.Count > 1 && GCDHelper.GetGCDCooldown() < 1500) return -1;
        return 1;
    }

    public static double 连击剩余时间 => Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds;

    public static bool 在近战范围内 =>
        Core.Me.Distance(Core.Me.GetCurrTarget()!) <= SettingMgr.GetSetting<GeneralSettings>().AttackRange;

    public static bool 在背身位 => Core.Resolve<MemApiTarget>().IsBehind;
    public static bool 在侧身位 => Core.Resolve<MemApiTarget>().IsFlanking;

    /// <summary>
    /// 充能技能还有多少冷却时间才可用
    /// </summary>
    /// <param name="skillId">技能id</param>
    /// <returns></returns>
    public static int 充能技能冷却时间(uint skillId)
    {
        var spell = skillId.GetSpell();
        return (int)(spell.Cooldown.TotalMilliseconds -
                     (spell.RecastTime.TotalMilliseconds / spell.MaxCharges) * (spell.MaxCharges - 1));
    }

    /// <summary>
    /// 自身有buff且时间小于
    /// </summary>
    public static bool Buff时间小于(uint buffId, int timeLeft)
    {
        if (!Core.Me.HasAura(buffId)) return false;
        return GetAuraTimeLeft(buffId) <= timeLeft;
    }

    /// <summary>
    /// 目标有buff且时间小于，有buff参数如果为false，则当目标没有玩家的buff是也返回true
    /// </summary>
    public static bool 目标Buff时间小于(uint buffId, int timeLeft, bool 有buff = true)
    {
        var target = Core.Me.GetCurrTarget();
        if (target == null) return false;

        if (有buff)
        {
            if (!target.HasLocalPlayerAura(buffId)) return false;
        }
        else
        {
            if (!target.HasLocalPlayerAura(buffId)) return true;
        }

        var time = Core.Resolve<MemApiBuff>().GetAuraTimeleft(target, buffId, true);
        return time <= timeLeft;
    }

    /// <summary>
    /// 在list中添加一个唯一的元素
    /// </summary>
    public static bool TryAdd<T>(this List<T> list, T item)
    {
        if (list.Contains(item)) return false;
        list.Add(item);
        return true;
    }

    public static bool 目标有任意我的buff(List<uint> buffs) =>
        buffs.Any(buff => Core.Me.GetCurrTarget()!.HasLocalPlayerAura(buff));


    public static IBattleChara? 最优aoe目标(this uint spellId, int count)
    {
        return TargetHelper.GetMostCanTargetObjects(spellId, count);
    }

    /// <summary>
    /// 获取非战斗状态时开了盾姿的人
    /// </summary>
    /// <returns></returns>
    public static IBattleChara? GetMt()
    {
        PartyHelper.UpdateAllies();
        return PartyHelper.CastableTanks
            .FirstOrDefault(p => p.HasAnyAura([743, 1833, 79, 91]));
    }

    public static bool In团辅()
    {
        //检测目标团辅
        List<uint> 目标团辅 = [背刺, 连环计];
        if (目标团辅.Any(buff => 目标Buff时间小于(buff, 15000))) return true;

        //检测自身团辅
        List<uint> 自身团辅 = [灼热之光, 星空, 占卜, 义结金兰, 战斗连祷, 大舞, 战斗之声, 鼓励, 神秘环];
        return 自身团辅.Any(buff => Buff时间小于(buff, 15000));
    }

    private static uint
        背刺 = 3849,
        强化药 = 49,
        灼热之光 = 2703,
        星空 = 3685,
        占卜 = 1878,
        义结金兰 = 1185,
        战斗连祷 = 786,
        大舞 = 1822,
        战斗之声 = 141,
        鼓励 = 1239,
        神秘环 = 2599,
        连环计 = 2617;

    public static IPlayerCharacter 自身 => Core.Me;

    public static uint 自身血量 => Core.Me.CurrentHp;

    public static uint 自身蓝量 => Core.Me.CurrentMp;

    public static float 自身血量百分比 => Core.Me.CurrentHpPercent();

    public static float 自身蓝量百分比 => Core.Me.CurrentMpPercent();

    public static int 队伍成员数量 => PartyHelper.Party.Count;

    public static uint 自身当前等级 => Core.Me.Level;


    public static bool 是否在副本中()
    {
        return Core.Resolve<MemApiCondition>().IsBoundByDuty();
    }

    public static uint 当前地图id => Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();

    /*public static int 副本人数()
    {
        return Core.Resolve<MemApiDuty>().DutyMembersNumber();
    }
*/
    public static bool 自身是否在移动()
    {
        return Core.Resolve<MemApiMove>().IsMoving();
    }

    public static bool 自身是否在读条()
    {
        return Core.Me.IsCasting;
    }

    public static int 自身周围单位数量(int range)
    {
        return TargetHelper.GetNearbyEnemyCount(range);
    }

    public static bool 自身存在Buff(uint id)
    {
        return Core.Me.HasAura(id);
    }

    public static bool 自身存在其中Buff(List<uint> auras, int msLeft = 0)
    {
        return Core.Me.HasAnyAura(auras, msLeft);
    }

    public static uint 自身命中其中Buff(List<uint> auras, int msLeft = 0)
    {
        return Core.Me.HitAnyAura(auras, msLeft);
    }

    public static bool 自身存在Buff大于时间(List<uint> id, int time)
    {
        foreach (var u in id) return Core.Me.HasMyAuraWithTimeleft(u, time);
        return false;
    }

    /** -----------------他人状态相关----------------- **/
    public static float 目标血量百分比 => Core.Me.GetCurrTarget().CurrentHpPercent();

    public static bool 目标战斗状态(IBattleChara target)
    {
        return target.InCombat();
    }

    public static bool 目标是否可见或在技能范围内(uint actionId)
    {
        return Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS(actionId) is not (566 or 562);
    }

    // public static bool 目标是否可见或在技能范围内(uint spellId, CharacterAgent target)
    // {
    //
    //     Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS()
    //     var localPlayer = (GameObject*)Svc.ClientState.LocalPlayer.Address;
    //     if (ActionManager.GetActionInRangeOrLoS(spellId, localPlayer, SignatureHook.Instance.从ObjectID获取GameObject(target.ObjectId)) is 562 or 566) return false;
    //     return true;
    // }
    //
    // public static bool 是否能对目标使用技能(uint spellId, CharacterAgent target)
    // {
    //     return ActionManager.CanUseActionOnTarget(spellId, SignatureHook.Instance.从ObjectID获取GameObject(target.ObjectId));
    // }


    public static float 目标到自身距离()
    {
        return Core.Me.GetCurrTarget().Distance(Core.Me);
    }

    public static float 到自身近战距离(IBattleChara target)
    {
        return Core.Me.Distance(target);
    }

    public static float 到自身距离(IBattleChara target)
    {
        return Core.Me.Distance(target);
    }

    public static bool 目标是否为假人()
    {
        return Core.Me.GetCurrTarget().IsDummy();
    }

    public static bool 目标的指定buff剩余时间是否大于(uint id, int timeLeft = 0)
    {
        return Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(id, timeLeft);
    }

    public static bool 指定buff剩余时间是否大于(IBattleChara target, uint id, int timeLeft = 0)
    {
        return target.HasMyAuraWithTimeleft(id, timeLeft);
    }

    public static int 目标的指定BUFF层数(IBattleChara target, uint buff)
    {
        return Core.Resolve<MemApiBuff>().GetStack(target, buff);
    }

    public static bool 目标可选状态(IBattleChara target)
    {
        return target.IsTargetable;
    }


    public static bool 目标是否拥有BUFF(uint id)
    {
        return Core.Me.GetCurrTarget().HasAura(id);
    }

    public static bool 目标是否准备放aoe(IBattleChara target, int timeLeft)
    {
        return TargetHelper.targetCastingIsBossAOE(target, timeLeft);
    }

    public static bool 目标是否拥有其中的BUFF(List<uint> auras, int timeLeft = 0)
    {
        return Core.Me.GetCurrTarget().HasAnyAura(auras, timeLeft);
    }

    public static bool 是否拥有其中的BUFF(IBattleChara target, List<uint> auras, int timeLeft = 0)
    {
        return target.HasAnyAura(auras, timeLeft);
    }

    public static bool 目标是否存在于DOT黑名单中(IBattleChara r)
    {
        return DotBlacklistHelper.IsBlackList(r);
    }

    public static bool 队员是否拥有可驱散状态()
    {
        return PartyHelper.CastableAlliesWithin30.Any(agent => agent.HasCanDispel() && agent.Distance(Core.Me) <= 30);
    }

    public static bool 队员是否拥有BUFF(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.Any(agent => agent.HasAura(buff));
    }

    public static int 二十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin20.Count(r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }
    
    public static int 三十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin30.Count(r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }

    public static int 十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin10.Count(r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }

    /// <summary>
    /// 获取指定目标对象的血量百分比
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <returns>血量百分比（0.0-100.0）</returns>
    public static float 获取目标血量百分比(IBattleChara target)
    {
        if (target == null || target.IsDead || !target.IsTargetable)
            return 0f;
    
        return target.CurrentHpPercent();
    }

    /** -----------------目标相关----------------- **/
    public static IBattleChara 自身目标 => Core.Me.GetCurrTarget();

    public static IBattleChara? 自身目标的目标 => Core.Me.GetCurrTargetsTarget();


    public static IBattleChara? 没有复活状态的死亡队友()
    {
        return PartyHelper.DeadAllies.FirstOrDefault(r => !r.HasAura(状态.复活) && r.IsTargetable);
    }

    public static IBattleChara 获取可驱散队员()
    {
        return PartyHelper.CastableAlliesWithin30.LastOrDefault(agent =>
            agent.HasCanDispel() && agent.Distance(Core.Me) <= 30);
    }

    public static IBattleChara 获取拥有buff队员(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.LastOrDefault(agent => agent.HasAura(buff));
    }

    public static IBattleChara 获取血量最低成员()
    {
        if (PartyHelper.CastableAlliesWithin30.Count == 0)
            return Core.Me;
        return PartyHelper.CastableAlliesWithin30
            .Where(r => r.CurrentHp > 0).MinBy(r => r.CurrentHpPercent());
    }

    public static IBattleChara 获取血量最低成员_排除buff(uint buffId)
    {
        if (PartyHelper.CastableAlliesWithin30.Count == 0)
            return Core.Me;
        return PartyHelper.CastableAlliesWithin30
            .Where(r => r.CurrentHp > 0 && !r.HasAura(buffId)).MinBy(r => r.CurrentHpPercent());
    }

    public static IBattleChara 获取最低血量T()
    {
        if (PartyHelper.CastableTanks.Count == 0)
            return Core.Me;
        if (PartyHelper.CastableTanks.Count == 2 && PartyHelper.CastableTanks[1].CurrentHpPercent() <
            PartyHelper.CastableTanks[0].CurrentHpPercent())
            return PartyHelper.CastableTanks[1];
        return PartyHelper.CastableTanks[0];
    }

    public static IBattleChara 获取最低血量T_排除buff(uint buffId)
    {
        if (PartyHelper.CastableTanks.Count == 0)
            return Core.Me;
        if (PartyHelper.CastableTanks.Count == 2 &&
            PartyHelper.CastableTanks[1].CurrentHpPercent() < PartyHelper.CastableTanks[0].CurrentHpPercent() &&
            !PartyHelper.CastableTanks[1].HasAura(buffId))
            return PartyHelper.CastableTanks[1];
        return PartyHelper.CastableTanks[0];
    }

    public static IBattleChara 获取距离最远成员()
    {
        IBattleChara RescueTarget = PartyHelper.CastableAlliesWithin30
            .Where(r => r.CurrentHp > 0).MaxBy(r => r.Distance(PartyHelper.CastableAlliesWithin30.FirstOrDefault()));

        return RescueTarget;
    }

// 在 Helper 类中添加
    public static int 获取队友位置(IBattleChara target)
    {
        var party = PartyHelper.CastableParty;

        // 使用对象的唯一标识符比较
        for (int i = 0; i < party.Count; i++)
        {
            // 比较对象的地址作为唯一标识
            if (party[i].Address == target.Address)
                return i + 1; // 位置从1开始计数
        }

        return -1; // 未找到
    }

    public static IBattleChara 获取第几号小队列表(int index)
    {
        return PartyHelper.CastableParty[index - 1];
    }


    /** -----------------技能相关----------------- **/
    public static Spell 获取技能(uint id)
    {
        return id.GetSpell();
    }

    public static uint 技能状态码(uint id)
    {
        return Core.Resolve<MemApiSpell>().GetActionState(id);
    }

    public static Spell 获取会变化的技能(uint id)
    {
        return Core.Resolve<MemApiSpell>().CheckActionChange(id).GetSpell();
    }

    public static uint 获取会变化的技能id(uint id)
    {
        return Core.Resolve<MemApiSpell>().CheckActionChange(id);
    }

    public static float 获取技能施法距离(uint id)
    {
        return ActionManager.GetActionRange(id);
    }


    public static int 高优先级gcd队列中技能数量()
    {
        return AI.Instance.BattleData.HighPrioritySlots_GCD.Count;
    }


    public static uint 上一个连击技能()
    {
        return Core.Resolve<MemApiSpell>().GetLastComboSpellId();
    }

    public static int GCD剩余时间()
    {
        return GCDHelper.GetGCDCooldown();
    }

    public static bool GCD可用状态()
    {
        return GCDHelper.CanUseGCD();
    }

    public static uint 上一个gcd技能()
    {
        return Core.Resolve<MemApiSpellCastSuccess>().LastGcd;
    }

    public static uint 上一个能力技能()
    {
        return Core.Resolve<MemApiSpellCastSuccess>().LastAbility;
    }


    public static bool 技能是否刚使用过(this uint spellId, int time = 1200)
    {
        return spellId.GetSpell().RecentlyUsed(time);
    }

    public static float 充能层数(this uint spellId)
    {
        return Core.Resolve<MemApiSpell>().GetCharges(spellId);
    }


    /** -----------------白魔----------------- **/
    public static int 白魔量谱_蓝花数量()
    {
        return Core.Resolve<JobApi_WhiteMage>().Lily;
    }

    public static int 白魔量谱_红花数量()
    {
        return Core.Resolve<JobApi_WhiteMage>().BloodLily;
    }



    /// <summary>
    /// 获取时间戳，13位，毫秒
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamps()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }
    
   public static List<IBattleChara> 获取全部战斗目标()
    {
        // 使用 AEAssist 的 TargetHelper 获取所有战斗角色
        
        var objectTable = TargetMgr.Instance.Enemys.Values;
        return objectTable.OfType<IBattleChara>().ToList();
    }

    
    public static List<IBattleChara> 获取所有敌人()
    {
        return 获取全部战斗目标()
            .Where(t => t.IsEnemy() && t.IsTargetable && !t.IsDead)
            .ToList();
    }
    public static List<IBattleChara> 获取所有可攻击目标()
    {
        return 获取全部战斗目标()
            .Where(t => t.IsTargetable && !t.IsDead)
            .ToList();
    }
    public static List<IBattleChara> 获取周围拥有Buff的敌对目标(float 距离, List<uint> buffIds)
    {
        return 获取全部战斗目标()
            .Where(t => 
                    t.IsEnemy() &&                // 是敌对目标
                    t.IsTargetable &&             // 可被选中
                    !t.IsDead &&                  // 未死亡
                    t.Distance(Core.Me) <= 距离 &&  // 在指定距离内
                    buffIds.Any(buff => t.HasAura(buff)) // 拥有任意指定buff
            )
            .ToList();
    }

    /// <summary>
    /// 获取PVP全部敌人，排除队友和团队成员，输出为目标表
    /// </summary>
    public static List<IBattleChara> 获取PVP全部敌人()
    {
        var partyAddresses = PartyHelper.Party.Select(p => p.Address).ToHashSet();
        var teamAddresses = PartyHelper.CastableParty.Select(p => p.Address).ToHashSet();
        return 获取全部战斗目标()
            .Where(t => t.IsEnemy() && t.IsTargetable && !t.IsDead
                && !partyAddresses.Contains(t.Address)
                && !teamAddresses.Contains(t.Address))
            .ToList();
    }

    /// <summary>
    /// 获取指定范围内有指定buff的敌对目标（PVP）
    /// </summary>
    public static List<IBattleChara> 获取范围有指定buff的敌对目标(float 距离, List<uint> buffIds)
    {
        return 获取PVP全部敌人()
            .Where(t => t.Distance(Core.Me) <= 距离 && buffIds.Any(buff => t.HasAura(buff)))
            .ToList();
    }

    /// <summary>
    /// 获取指定范围内无指定buff的敌对目标（PVP）
    /// </summary>
    public static List<IBattleChara> 获取范围无指定buff的敌对目标(float 距离, List<uint> buffIds)
    {
        return 获取PVP全部敌人()
            .Where(t => t.Distance(Core.Me) <= 距离 && !buffIds.Any(buff => t.HasAura(buff)))
            .ToList();
    }

    /// <summary>
    /// 获取指定范围内有/无指定buff且血量低于指定数值的敌对目标（PVP）
    /// </summary>
    /// <param name="距离">范围</param>
    /// <param name="buffIds">指定buff列表</param>
    /// <param name="hasBuff">true为有buff，false为无buff</param>
    /// <param name="hpPercent">血量百分比阈值</param>
    public static List<IBattleChara> 获取范围血量低于且Buff条件的敌对目标(float 距离, List<uint> buffIds, bool hasBuff, float hpPercent)
    {
        return 获取PVP全部敌人()
            .Where(t => t.Distance(Core.Me) <= 距离
                && (hasBuff ? buffIds.Any(buff => t.HasAura(buff)) : !buffIds.Any(buff => t.HasAura(buff)))
                && t.CurrentHpPercent() <= hpPercent)
            .ToList();
    }
    
    private static readonly Dictionary<ulong, long> DeathTimestamps = new();
    public static IBattleChara? 没有复活buff的死亡队友_时间(int delay = -1)
    {
        if (delay < 0) delay = 默认值.实例.复活延迟 * 1000;

        var deadAllies = PartyHelper.DeadAllies;
        long currentTime = GetTimeStamps();
        IBattleChara? candidate = null;
        long minDeathTime = long.MaxValue;

        foreach (var ally in deadAllies)
        {
            ulong address = (ulong)ally.Address;
        
            // 修复点：复活时清除历史记录
            if (!ally.IsTargetable || ally.HasAura(状态.复活))
            {
                if (DeathTimestamps.ContainsKey(address))
                    DeathTimestamps.Remove(address);
                continue;
            }
        
            // 仅当无记录时写入新时间戳（兼容首次死亡和清除后再次死亡）
            if (!DeathTimestamps.TryGetValue(address, out var deathTime))
            {
                deathTime = currentTime;
                DeathTimestamps[address] = deathTime;
            }
        
            // 正常计算等待时间
            if (currentTime - deathTime >= delay)
            {
                if (deathTime < minDeathTime)
                {
                    minDeathTime = deathTime;
                    candidate = ally;
                }
            }
        }
        return candidate;
    }
    public static unsafe bool 目标是否被阻挡(IBattleChara source, IBattleChara target)
    {
        if (source == null || target == null)
            return false;
        // 获取source和target的指针
        var sourcePtr = (GameObject*)source.Address;
        var targetPtr = (GameObject*)target.Address;
        // 如果指针为零，则返回false（表示没有阻挡？或者应该视为阻挡？这里我们视为没有阻挡，因为对象无效，无法判断）
        if (sourcePtr == null || targetPtr == null)
            return false;
        // 调用一个unsafe的私有方法
        return IsBlockedInternal(sourcePtr, targetPtr);
    }
    private static unsafe bool IsBlockedInternal(GameObject* source, GameObject* target)
    {
        try
        {
            var sourcePos = *source->GetPosition();
            var targetPos = *target->GetPosition();
            sourcePos.Y += 2;
            targetPos.Y += 2;
            var offset = targetPos - sourcePos;
            var maxDist = offset.Magnitude;
            // 避免除以零
            if (maxDist < 0.001f)
                return false;
            var direction = offset / maxDist;
            // 调用底层的射线投射函数
            bool hasHit = BGCollisionModule.RaycastMaterialFilter(sourcePos, direction, out _, maxDist);
            return hasHit;
        }
        catch
        {
            // 发生异常时返回true（阻挡）？或者false？这里我们返回false，避免影响流程
            return false;
        }
    }
    public static IBattleChara? 获取最近未被阻挡无指定buff的敌对目标(float 距离, List<uint> buffIds)
    {
        // 获取所有可攻击目标
        var allTargets = 获取所有可攻击目标();
        // 过滤条件：在指定距离内、是敌人、没有指定buff
        var filteredTargets = allTargets
            .Where(t => 
                t.IsEnemy() &&
                t.Distance(Core.Me) <= 距离 &&
                !buffIds.Any(buff => t.HasAura(buff)))
            .ToList();

        // 按距离排序并查找第一个未被阻挡的目标
        foreach (var target in filteredTargets.OrderBy(t => t.Distance(Core.Me)))
        {
            if (!目标是否被阻挡(Core.Me, target))
            {
                return target;
            }
        }
    
        return null; // 未找到符合条件的敌人
    }
    public static IBattleChara? 获取最近的无阻挡敌人(float 最大距离 = 25)
    {
        // 获取所有可攻击目标
        var allTargets = 获取所有可攻击目标();
    
        // 过滤条件：在指定距离内、是敌人、未被阻挡
        var filteredTargets = allTargets
            .Where(t => 
                t.IsEnemy() &&
                t.Distance(Core.Me) <= 最大距离)
            .ToList();

        // 按距离排序并查找第一个未被阻挡的目标
        foreach (var target in filteredTargets.OrderBy(t => t.Distance(Core.Me)))
        {
            if (!目标是否被阻挡(Core.Me, target))
            {
                return target;
            }
        }
    
        return null; // 未找到符合条件的敌人
    }
    

private static readonly Dictionary<ulong, (long DeathTime, int Priority)> DeathRecords = new();

public static IBattleChara? 没有复活buff的死亡队友_时间_优先坦克和治疗(int delay = -1)
{
    if (delay < 0) delay = 默认值.实例.复活延迟 * 1000;
    
    var deadAllies = PartyHelper.DeadAllies;
    long currentTime = GetTimeStamps();
    
    // 清理已复活或无效的目标
    var toRemove = new List<ulong>();
    foreach (var entry in DeathRecords)
    {
        ulong address = entry.Key;
        var ally = deadAllies.FirstOrDefault(a => (ulong)a.Address == address);
        
        // 如果目标已复活或不可用，标记移除
        if (ally == null || !ally.IsTargetable || ally.HasAura(状态.复活))
        {
            toRemove.Add(address);
        }
    }
    
    // 移除无效记录
    foreach (var address in toRemove)
    {
        DeathRecords.Remove(address);
    }
    
    // 更新当前死亡队友的记录
    foreach (var ally in deadAllies)
    {
        ulong address = (ulong)ally.Address;
        
        // 跳过有复活buff或不可选中的目标
        if (!ally.IsTargetable || ally.HasAura(状态.复活))
            continue;
        
        // 确定职业优先级：坦克/治疗为高优先级(1)，DPS为低优先级(2)
        int priority = ally.IsTank() || ally.IsHealer() ? 1 : 2;
        
        // 如果是首次死亡或已被复活过，重新记录死亡时间
        if (!DeathRecords.ContainsKey(address) || toRemove.Contains(address))
        {
            DeathRecords[address] = (currentTime, priority);
        }
    }
    
    // 检查高优先级目标（坦克/治疗）
    var highPriorityTargets = DeathRecords
        .Where(entry => entry.Value.Priority == 1)
        .Select(entry => new {
            Address = entry.Key,
            DeathTime = entry.Value.DeathTime,
            Ally = deadAllies.FirstOrDefault(a => (ulong)a.Address == entry.Key)
        })
        .Where(x => x.Ally != null)
        .ToList();
    
    // 检查低优先级目标（DPS）
    var lowPriorityTargets = DeathRecords
        .Where(entry => entry.Value.Priority == 2)
        .Select(entry => new {
            Address = entry.Key,
            DeathTime = entry.Value.DeathTime,
            Ally = deadAllies.FirstOrDefault(a => (ulong)a.Address == entry.Key)
        })
        .Where(x => x.Ally != null)
        .ToList();
    
    // 检查满足延迟的高优先级目标（按死亡时间排序）
    var readyHighPriority = highPriorityTargets
        .Where(t => currentTime - t.DeathTime >= delay)
        .OrderBy(t => t.DeathTime)
        .FirstOrDefault();
    
    if (readyHighPriority != null)
    {
        return readyHighPriority.Ally;
    }
    
    // 检查满足延迟且期间无高优先级死亡的DPS
    var readyLowPriority = lowPriorityTargets
        .Where(t => currentTime - t.DeathTime >= delay)
        .OrderBy(t => t.DeathTime)
        .FirstOrDefault(t => 
            !highPriorityTargets.Any(h => 
                h.DeathTime > t.DeathTime && 
                h.DeathTime <= t.DeathTime + delay));
    
    if (readyLowPriority != null)
    {
        return readyLowPriority.Ally;
    }
    
    // 返回下一个即将满足延迟的高优先级目标
    return highPriorityTargets
        .OrderBy(t => t.DeathTime)
        .FirstOrDefault()?.Ally;
}



}