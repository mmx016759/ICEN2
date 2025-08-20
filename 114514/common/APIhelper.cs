using System.Numerics;
using AEAssist;
using AEAssist.Avoid;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Define;
using AEAssist.Define.HotKey;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Client.Game.Event;

namespace icen.common
{
    /// <summary>
    /// AEAssist 完整API封装
    /// </summary>
    public static class ApiHelper
    {
        // ==================================
        // 核心类 (Core)
        // ==================================
        
        /// <summary>
        /// 获取当前玩家对象
        /// </summary>
        public static IPlayerCharacter 当前玩家 => Core.Me;
        
        /// <summary>
        /// 获取指定类型的服务实例
        /// </summary>
        public static T 获取服务<T>() where T : class => Core.Resolve<T>();

        // ==================================
        // 内存API (MemoryApi)
        // ==================================
        
        // === MemApiSpell - 技能施法管理 ===
        
        /// <summary>
        /// 打断读条
        /// </summary>
        public static void 打断读条() => Core.Resolve<MemApiSpell>().CancelCast();
        
        /// <summary>
        /// 宠物移动
        /// </summary>
        public static void 宠物移动(Vector3 目标位置, uint 动作id = 3) => 
            Core.Resolve<MemApiSpell>().PetMove(目标位置, 动作id);
        
        /// <summary>
        /// 根据技能名称获取ID
        /// </summary>
        public static uint 获取技能id(string 技能名称) => 
            Core.Resolve<MemApiSpell>().GetId(技能名称);
        
        /// <summary>
        /// 获取技能目标类型
        /// </summary>
        public static SpellTargetType 获取技能目标类型(uint 技能id) => 
            Core.Resolve<MemApiSpell>().GetSkillTarget(技能id);
        
        
        /// <summary>
        /// 在指定位置施放技能
        /// </summary>
        public static bool 施放技能(uint 技能id, Vector3 位置) => 
            Core.Resolve<MemApiSpell>().Cast(技能id, 位置);
        
        
        /// <summary>
        /// 获取技能状态码
        /// </summary>
        public static uint 获取技能状态(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetActionState(技能ID);
        
        /// <summary>
        /// 检查技能变化（如连击）
        /// </summary>
        public static uint 检查技能变化(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().CheckActionChange(技能ID);
        
        /// <summary>
        /// 检查技能是否可用
        /// </summary>
        public static bool 技能可用(uint 技能ID, float GCD时间限制 = 0.5f, float 能力技时间限制 = 0.1f) => 
            Core.Resolve<MemApiSpell>().CheckActionCanUse(技能ID, GCD时间限制, 能力技时间限制);
        
        /// <summary>
        /// 获取技能范围
        /// </summary>
        public static float 获取技能范围(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetActionRange(技能ID);
        
        /// <summary>
        /// 获取技能总冷却时间
        /// </summary>
        public static TimeSpan 获取技能冷却总时间(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetRecastTime(技能ID);
        
        /// <summary>
        /// 获取技能剩余冷却时间
        /// </summary>
        public static TimeSpan 获取技能剩余冷却(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetCooldown(技能ID);
        
        /// <summary>
        /// 获取技能充能层数
        /// </summary>
        public static float 获取技能充能数(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetCharges(技能ID);
        
        /// <summary>
        /// 获取GCD持续时间
        /// </summary>
        public static int 获取GCD持续时间(bool 原始值 = false) => 
            Core.Resolve<MemApiSpell>().GetGCDDuration(原始值);
        
        /// <summary>
        /// 获取已过GCD时间
        /// </summary>
        public static int 获取已过GCD时间() => 
            Core.Resolve<MemApiSpell>().GetElapsedGCD();
        
        /// <summary>
        /// 是否在动画锁定中
        /// </summary>
        public static bool 动画锁定中 => 
            Core.Resolve<MemApiSpell>().IsAnimationLock;
        
        /// <summary>
        /// 获取上一个连击技能
        /// </summary>
        public static uint 上一个连击技能 => 
            Core.Resolve<MemApiSpell>().GetLastComboSpellId();
        
        /// <summary>
        /// 是否有技能队列
        /// </summary>
        public static bool 有技能队列 => 
            Core.Resolve<MemApiSpell>().HasActionQueue;
        
        /// <summary>
        /// 队列中的技能ID
        /// </summary>
        public static uint 队列中的技能ID => 
            Core.Resolve<MemApiSpell>().QueuedActionId;
        
        /// <summary>
        /// 动画锁定时间
        /// </summary>
        public static float 动画锁定时间 => 
            Core.Resolve<MemApiSpell>().AnimationLock;
        
        /// <summary>
        /// 检查技能范围和视线(对当前目标)
        /// </summary>
        public static uint 检查技能范围视线(uint 技能ID) => 
            Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS(技能ID);
        
        /// <summary>
        /// 检查技能范围和视线(对指定目标)
        /// </summary>
        public static bool 检查技能范围视线(uint 技能ID, IBattleChara 目标) => 
            Core.Resolve<MemApiSpell>().CheckActionInRangeOrLoS(技能ID, 目标);
        
        // === MemApiBuff - Buff管理 ===
      
        // === MemApiTarget - 目标管理 ===
        
        /// <summary>
        /// 设置目标
        /// </summary>
        public static void 设置目标(IGameObject 目标) => 
            Core.Resolve<MemApiTarget>().SetTarget(目标);
        
        /// <summary>
        /// 清除目标
        /// </summary>
        public static void 清除目标() => 
            Core.Resolve<MemApiTarget>().ClearTarget();
        
        /// <summary>
        /// 获取单位的当前目标
        /// </summary>
        public static IGameObject 获取当前目标(IBattleChara 单位) => 
            Core.Resolve<MemApiTarget>().CurrTarget(单位);
        
        /// <summary>
        /// 是否可以攻击目标
        /// </summary>
        public static bool 可攻击(IGameObject 目标, uint 动作ID = 142) => 
            Core.Resolve<MemApiTarget>().CanAttack(目标, 动作ID);
        
        /// <summary>
        /// 当前血量百分比
        /// </summary>
        public static float 当前血量百分比(IBattleChara 单位) => 
            Core.Resolve<MemApiTarget>().CurrentHealthPercent(单位);
        
        /// <summary>
        /// 当前魔法值百分比
        /// </summary>
        public static float 当前蓝量百分比(IBattleChara 单位) => 
            Core.Resolve<MemApiTarget>().CurrentManaPercent(单位);
        
        /// <summary>
        /// 按名称查找单位
        /// </summary>
        public static bool 查找单位(string 名称, float 距离, out IGameObject 单位) => 
            Core.Resolve<MemApiTarget>().TryFindUnit(名称, 距离, out 单位);
        
        /// <summary>
        /// 按ID查找单位
        /// </summary>
        public static bool 查找单位(uint NPCID, bool 可选中, out IGameObject 单位) => 
            Core.Resolve<MemApiTarget>().TryFindUnit(NPCID, 可选中, out 单位);
        
        // === MemApiMove - 移动控制 ===
        
        /// <summary>
        /// 是否在移动中
        /// </summary>
        public static bool 移动中() => 
            Core.Resolve<MemApiMove>().IsMoving();
        
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        public static void 移动到(Vector3 目标位置) => 
            Core.Resolve<MemApiMove>().MoveToTarget(目标位置);
        
        /// <summary>
        /// 取消移动
        /// </summary>
        public static void 取消移动() => 
            Core.Resolve<MemApiMove>().CancelMove();
        
        /// <summary>
        /// 自动移动开关
        /// </summary>
        public static void 自动移动(bool 开关) => 
            Core.Resolve<MemApiMove>().AutoMove(开关);
        
        /// <summary>
        /// 设置角色朝向
        /// </summary>
        public static void 设置朝向(float 朝向, bool 发送按键 = true) => 
            Core.Resolve<MemApiMove>().SetRot(朝向, 发送按键);
        
        /// <summary>
        /// 面向目标
        /// </summary>
        public static void 面向目标(IBattleChara 目标, bool 背面 = false) => 
            Core.Resolve<MemApiMove>().Face2Target(目标, 背面);
        
        /// <summary>
        /// 滑步传送
        /// </summary>
        public static void 滑步传送(Vector3 目标, long 时间) => 
            Core.Resolve<MemApiMove>().SlideTp(目标, 时间);
        
        /// <summary>
        /// 锁定位置
        /// </summary>
        public static void 锁定位置(Vector3 目标, long 持续时间) => 
            Core.Resolve<MemApiMove>().LockPos(目标, 持续时间);
        
        /// <summary>
        /// 路径是否启用
        /// </summary>
        public static bool 路径启用 => 
            Core.Resolve<MemApiMove>().PathEnabled;
        
        // === MemApiCondition - 状态检测 ===
        
        /// <summary>
        /// 是否在副本中
        /// </summary>
        public static bool 副本中 => 
            Core.Resolve<MemApiCondition>().IsBoundByDuty();

        /// <summary>
        /// 是否在任务事件中
        /// </summary>
        public static bool 任务事件中 => 
            Core.Resolve<MemApiCondition>().IsInQuestEvent();
        
        /// <summary>
        /// 是否在区域间传送中
        /// </summary>
        public static bool 区域传送中 => 
            Core.Resolve<MemApiCondition>().IsBetweenAreas();
        
        /// <summary>
        /// 是否在无人岛
        /// </summary>
        public static bool 无人岛中 => 
            Core.Resolve<MemApiCondition>().IsInIslandSanctuary();
        
        /// <summary>
        /// 是否在制作中
        /// </summary>
        public static bool 制作中 => 
            Core.Resolve<MemApiCondition>().IsCrafting();
        
        /// <summary>
        /// 是否在采集中
        /// </summary>
        public static bool 采集中 => 
            Core.Resolve<MemApiCondition>().IsGathering();
        
        /// <summary>
        /// 是否在演奏中
        /// </summary>
        public static bool 演奏中 => 
            Core.Resolve<MemApiCondition>().IsInBardPerformance();
        
        // === MemApiDuty - 副本管理 ===
        
        /// <summary>
        /// 副本名称
        /// </summary>
        public static string 副本名称 => 
            Core.Resolve<MemApiDuty>().DutyName();
        
        /// <summary>
        /// 副本人数
        /// </summary>
        public static int 副本人数 => 
            Core.Resolve<MemApiDuty>().DutyMembersNumber();
        
        /// <summary>
        /// 获取副本进度信息
        /// </summary>
        public static DutySchedule? 获取副本进度 => 
            Core.Resolve<MemApiDuty>().GetSchedule();
        
        /// <summary>
        /// 是否在Boss房间
        /// </summary>
        public static bool 在Boss房间 => 
            Core.Resolve<MemApiDuty>().IsInBossRoom();
        
        /// <summary>
        /// 是否正式开始
        /// </summary>
        public static bool 副本已开始 => 
            Core.Resolve<MemApiDuty>().InMission;
        
        /// <summary>
        /// 是否在Boss战斗中
        /// </summary>
        public static bool 在Boss战中 => 
            Core.Resolve<MemApiDuty>().InBossBattle;
        
        /// <summary>
        /// 副本是否结束
        /// </summary>
        public static bool 副本结束 => 
            Core.Resolve<MemApiDuty>().IsOver;
        
        // === MemApiAddon - UI窗口交互 ===
        
        /// <summary>
        /// 检测窗口是否存在
        /// </summary>
        public static bool 窗口存在(string 窗口名称) => 
            Core.Resolve<MemApiAddon>().CheckAddon(窗口名称);
        
        /// <summary>
        /// 检查窗口和节点是否就绪
        /// </summary>
        public static bool 窗口就绪(string 窗口名称) => 
            Core.Resolve<MemApiAddon>().IsAddonAndNodesReady(窗口名称);
        
        /// <summary>
        /// 等待窗口状态
        /// </summary>
        public static Task<bool> 等待窗口(string 窗口名称, bool 可见 = true, int 超时 = 5000, int 延迟 = 300) => 
            Core.Resolve<MemApiAddon>().WaitAddonUntil(窗口名称, 可见, 超时, 延迟);
        
        /// <summary>
        /// 设置控件数值
        /// </summary>
        public static void 设置窗口值(string 窗口名称, uint 节点索引, Int32 值) => 
            Core.Resolve<MemApiAddon>().SetAddonValue(窗口名称, 节点索引, 值);
        
        /// <summary>
        /// 点击窗口按钮
        /// </summary>
        public static void 点击窗口按钮(string 窗口名称, params object[] 值) => 
            Core.Resolve<MemApiAddon>().SetAddonClicked(窗口名称, 值);
        
        /// <summary>
        /// 获取节点文本
        /// </summary>
        public static string 获取窗口文本(string 窗口名称, params int[] 节点编号) => 
            Core.Resolve<MemApiAddon>().GetNodeText(窗口名称, 节点编号);
        
        /// <summary>
        /// 获取窗口数据
        /// </summary>
        public static AddonValue 获取窗口值(string 窗口名称, uint 索引) => 
            Core.Resolve<MemApiAddon>().GetAddonValue(窗口名称, 索引);
        
        /// <summary>
        /// 与游戏内单位交互
        /// </summary>
        public static bool 交互单位(uint 对象ID, bool 检查视线 = true) => 
            Core.Resolve<MemApiAddon>().InteractWithUnit(对象ID, 检查视线);
        
        /// <summary>
        /// 从任务列表获取任务名称
        /// </summary>
        public static string 获取任务名称(int 索引 = 1) => 
            Core.Resolve<MemApiAddon>().GetQuestNameFromTodoList(索引);
        
        // === MemApiChatMessage - 聊天和提示 ===
        
        /// <summary>
        /// 说话频道
        /// </summary>
        public static void 说话(string 消息) => 
            Core.Resolve<MemApiChatMessage>().Say(消息);
        
        /// <summary>
        /// 呼喊频道
        /// </summary>
        public static void 呼喊(string 消息) => 
            Core.Resolve<MemApiChatMessage>().Shout(消息);
        
        /// <summary>
        /// 小队频道
        /// </summary>
        public static void 小队(string 消息) => 
            Core.Resolve<MemApiChatMessage>().Party(消息);
        
        /// <summary>
        /// 打印插件消息
        /// </summary>
        public static void 打印消息(string 消息) => 
            Core.Resolve<MemApiChatMessage>().PrintPluginMessage(消息);
        
        /// <summary>
        /// 显示Toast提示
        /// </summary>
        public static void 显示提示(string 消息) => 
            Core.Resolve<MemApiChatMessage>().Toast(消息);
        
        /// <summary>
        /// 显示带样式的文本提示
        /// </summary>
        public static void 显示样式提示(string 消息, int 样式, int 时间) => 
            Core.Resolve<MemApiChatMessage>().Toast2(消息, 样式, 时间);
        
        /// <summary>
        /// 最后链接的物品ID
        /// </summary>
        public static uint 最后链接物品ID => 
            Core.Resolve<MemApiChatMessage>().LastLinkedItemId();
        
        // === MemApiParty - 小队管理 ===
        
        /// <summary>
        /// 获取小队成员列表
        /// </summary>
        public static List<IBattleChara> 获取小队成员() => 
            Core.Resolve<MemApiParty>().GetParty();
        
        /// <summary>
        /// 获取小队成员
        /// </summary>
        public static IEnumerable<IBattleChara> 获取所有成员() => 
            Core.Resolve<MemApiParty>().GetMembers();
        
        /// <summary>
        /// 获取鼠标悬停目标
        /// </summary>
        public static IBattleChara? 鼠标目标 => 
            Core.Resolve<MemApiParty>().Mo();
        
        /// <summary>
        /// 小队成员字典
        /// </summary>
        public static Dictionary<int, IBattleChara> 小队列表 => 
            Core.Resolve<MemApiParty>().PartyList;
        
        // === MemApiZoneInfo - 区域信息 ===
        
        /// <summary>
        /// 获取当前地图ID
        /// </summary>
        public static uint 当前地图ID => 
            Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();
        
        // === MemApiSpellCastSuccess - 技能成功施放记录 ===
        
        /// <summary>
        /// 上一个GCD技能
        /// </summary>
        public static uint 上一个GCD技能 => 
            Core.Resolve<MemApiSpellCastSuccess>().LastGcd;
        
        /// <summary>
        /// 上一个能力技能
        /// </summary>
        public static uint 上一个能力技能 => 
            Core.Resolve<MemApiSpellCastSuccess>().LastAbility;
        
        // === MemApiCountdown - 倒计时 ===
        
        /// <summary>
        /// 倒计时是否激活
        /// </summary>
        public static bool 倒计时激活 => 
            Core.Resolve<MemApiCountdown>().IsActive();
        
        /// <summary>
        /// 剩余时间
        /// </summary>
        public static float 倒计时剩余时间 => 
            Core.Resolve<MemApiCountdown>().TimeRemaining();
        
        /// <summary>
        /// 倒计时器值
        /// </summary>
        public static float 倒计时值 => 
            Core.Resolve<MemApiCountdown>().Timer();
        
        // === MemApiHotkey - 热键检测 ===
        
        /// <summary>
        /// 检查按键状态
        /// </summary>
        public static bool 检查按键(ModifierKey 修饰键, Keys 按键) => 
            Core.Resolve<MemApiHotkey>().CheckState(修饰键, 按键);
        
        /// <summary>
        /// 检查按键按下状态
        /// </summary>
        public static bool 检查按键按下(ModifierKey 修饰键, Keys 按键) => 
            Core.Resolve<MemApiHotkey>().CheckStateDown(修饰键, 按键);
        
        /// <summary>
        /// 获取任意按键输入
        /// </summary>
        public static Keys 获取按键输入 => 
            Core.Resolve<MemApiHotkey>().GetAnyKeyInput();
        
        // === MemApiIcon - 图标资源 ===
        
        /// <summary>
        /// 获取技能图标
        /// </summary>
        public static bool 获取技能图标(uint 技能ID, out IDalamudTextureWrap? 纹理, bool 调整 = true) => 
            Core.Resolve<MemApiIcon>().GetActionTexture(技能ID, out 纹理, 调整);
        
        /// <summary>
        /// 获取物品图标
        /// </summary>
        public static bool 获取物品图标(int 图标ID, out IDalamudTextureWrap 纹理, bool 高品质 = true) => 
            Core.Resolve<MemApiIcon>().GetIconTexture(图标ID, out 纹理, 高品质);
        
        /// <summary>
        /// 获取职业图标ID
        /// </summary>
        public static uint 获取职业图标ID(string 职业名称) => 
            Core.Resolve<MemApiIcon>().GetJobIconId(职业名称);
        
        /// <summary>
        /// 根据职业枚举获取图标ID
        /// </summary>
        public static uint 获取职业图标ID(Jobs 职业) => 
            Core.Resolve<MemApiIcon>().GetJobIconIdByJob(职业);
        
        // === MemApiMacro - 宏命令 ===
        
        /// <summary>
        /// 执行宏命令
        /// </summary>
        public static bool 执行宏(MacroItem 宏) => 
            Core.Resolve<MemApiMacro>().UseMacro(宏);

        // ==================================
        // 辅助类 (Helper)
        // ==================================
        
        // === PartyHelper - 小队管理辅助 ===
        
        /// <summary>
        /// 死亡单位列表
        /// </summary>
        public static List<IBattleChara> 死亡队友 => PartyHelper.DeadAllies;
        
        /// <summary>
        /// 整个队伍成员
        /// </summary>
        public static List<IBattleChara> 队伍成员 => PartyHelper.Party;
        
        /// <summary>
        /// 可施法的队友(包括自己)
        /// </summary>
        public static List<IBattleChara> 可施法队友 => PartyHelper.CastableParty;
        
        /// <summary>
        /// 坦克列表
        /// </summary>
        public static List<IBattleChara> 坦克列表 => PartyHelper.CastableTanks;
        
        /// <summary>
        /// 治疗列表
        /// </summary>
        public static List<IBattleChara> 治疗列表 => PartyHelper.CastableHealers;
        
        /// <summary>
        /// DPS列表
        /// </summary>
        public static List<IBattleChara> DPS列表 => PartyHelper.CastableDps;
        
        /// <summary>
        /// 主坦克列表
        /// </summary>
        public static List<IBattleChara> 主坦克列表 => PartyHelper.CastableMainTanks;
        
        /// <summary>
        /// 30米内可施法队友
        /// </summary>
        public static List<IBattleChara> 三十米内队友 => PartyHelper.CastableAlliesWithin30;
        
        /// <summary>
        /// 25米内可施法队友
        /// </summary>
        public static List<IBattleChara> 二十五米内队友 => PartyHelper.CastableAlliesWithin25;
        
        /// <summary>
        /// 20米内可施法队友
        /// </summary>
        public static List<IBattleChara> 二十米内队友 => PartyHelper.CastableAlliesWithin20;
        
        /// <summary>
        /// 15米内可施法队友
        /// </summary>
        public static List<IBattleChara> 十五米内队友 => PartyHelper.CastableAlliesWithin15;
        
        /// <summary>
        /// 10米内可施法队友
        /// </summary>
        public static List<IBattleChara> 十米内队友 => PartyHelper.CastableAlliesWithin10;
        
        /// <summary>
        /// 3米内可施法队友
        /// </summary>
        public static List<IBattleChara> 三米内队友 => PartyHelper.CastableAlliesWithin3;
        
        /// <summary>
        /// 近战职业列表
        /// </summary>
        public static List<IBattleChara> 近战列表 => PartyHelper.CastableMelees;
        
        /// <summary>
        /// 远程职业列表
        /// </summary>
        public static List<IBattleChara> 远程列表 => PartyHelper.CastableRangeds;
        
        // === TargetHelper - 目标辅助 ===
        
        /// <summary>
        /// 获取目标附近敌人数量
        /// </summary>
        public static int 目标附近敌人数(IBattleChara 目标, int 施法范围, int 伤害范围) => 
            TargetHelper.GetNearbyEnemyCount(目标, 施法范围, 伤害范围);
        
        /// <summary>
        /// 获取自身周围敌人数量
        /// </summary>
        public static int 自身周围敌人数(int 范围) => 
            TargetHelper.GetNearbyEnemyCount(范围);
        
        /// <summary>
        /// 扇形范围内敌人数量
        /// </summary>
        public static int 扇形范围敌人数(IBattleChara 自身, IBattleChara 目标, float 扇形半径, float 扇形角度) => 
            TargetHelper.GetEnemyCountInsideSector(自身, 目标, 扇形半径, 扇形角度);
        

        
        /// <summary>
        /// 2D距离计算
        /// </summary>
        public static float 二维距离(IBattleChara 目标, IBattleChara 原点) => 
            TargetHelper.GetTargetDistanceFromMeTest2D(目标, 原点);
        
        /// <summary>
        /// 目标是否准备放AOE
        /// </summary>
        public static bool 目标即将AOE(IBattleChara 目标, int 剩余时间) => 
            TargetHelper.targetCastingIsBossAOE(目标, 剩余时间);
        
        // === GCDHelper - GCD管理辅助 ===
        
        /// <summary>
        /// 是否为插入能力技时机
        /// </summary>
        public static bool 可插入能力技(float 时间 = 1000f) => 
            GCDHelper.Is2ndAbilityTime(时间);
        
        /// <summary>
        /// 获取GCD总时间
        /// </summary>
        public static int GCD总时间 => 
            GCDHelper.GetGCDDuration();
        
        /// <summary>
        /// 获取GCD剩余冷却
        /// </summary>
        public static int GCD剩余冷却 => 
            GCDHelper.GetGCDCooldown();
        
        /// <summary>
        /// 获取已过GCD时间
        /// </summary>
        public static int 已过GCD时间 => 
            GCDHelper.GetElapsedGCD();
        
        /// <summary>
        /// 是否可以使用GCD技能
        /// </summary>
        public static bool 可用GCD => 
            GCDHelper.CanUseGCD();
        
        /// <summary>
        /// 是否可以使用能力技
        /// </summary>
        public static bool 可用能力技 => 
            GCDHelper.CanUseOffGcd();
        
        // === SpellHelper - 技能辅助 ===
        
        /// <summary>
        /// 检查当前是否可以使用技能
        /// </summary>
        public static bool 可使用技能 => 
            SpellHelper.CanUseAction();
        
        /// <summary>
        /// 根据目标类型获取目标
        /// </summary>
        public static IBattleChara? 获取目标(SpellTargetType 目标类型) => 
            SpellHelper.GetTarget(目标类型);
        
        /// <summary>
        /// 获取技能的目标
        /// </summary>
        public static IBattleChara? 获取技能目标(Spell 技能) => 
            SpellHelper.GetTarget(技能);
        
        /// <summary>
        /// 打印所有技能信息（调试用）
        /// </summary>
        public static void 打印技能信息() => 
            SpellHelper.PrintSpell();
        
        // === DotBlacklistHelper - DOT黑名单辅助 ===
        
        /// <summary>
        /// 检查目标是否在DOT黑名单中
        /// </summary>
        public static bool 在黑名单(IBattleChara 游戏对象) => 
            DotBlacklistHelper.IsBlackList(游戏对象);
        
        // === LogHelper - 日志辅助 ===
        
        /// <summary>
        /// 调试日志
        /// </summary>
        public static void 调试日志(string 消息) => 
            LogHelper.Debug(消息);
        
        /// <summary>
        /// 信息日志
        /// </summary>
        public static void 信息日志(string 消息) => 
            LogHelper.Info(消息);
        
        /// <summary>
        /// 分类日志
        /// </summary>
        public static void 分类日志(string 分类, string 消息) => 
            LogHelper.Print(分类, 消息);
        
        /// <summary>
        /// 错误日志
        /// </summary>
        public static void 错误日志(string 消息) => 
            LogHelper.Error(消息);
        
        // === 其他常用Helper类 ===
        
        // 注：以下Helper类方法需要根据实际实现封装
        // 这里只列出方法声明，具体实现需参考AEAssist源码
        // ==================================
        // 扩展方法 (Extension)
        // ==================================
        
        // === IGameObject 扩展 ===
        
        /// <summary>
        /// 是否是自己
        /// </summary>
        public static bool 是自身(this IGameObject 游戏对象) => 
            游戏对象.IsMe();
        
        /// <summary>
        /// 是否是本地玩家
        /// </summary>
        public static bool 是本地玩家(this IGameObject 游戏对象) => 
            游戏对象.IsLocalPlayer();
        
        /// <summary>
        /// 获取与玩家的关系
        /// </summary>
        public static Relationship 与玩家关系(this IGameObject 游戏对象) => 
            游戏对象.GetRelationshipWithLocalPlayer();
        
        /// <summary>
        /// 计算两目标距离
        /// </summary>
        public static float 距离(this IGameObject 源目标, IGameObject 目标, DistanceMode 模式 = DistanceMode.IgnoreAll) => 
            源目标.Distance(目标, 模式);
        
        /// <summary>
        /// 是否在技能范围内
        /// </summary>
        public static bool 在技能范围内(this IGameObject 游戏对象, float 范围 = 30) => 
            游戏对象.InActionRange(范围);
        
        /// <summary>
        /// 获取眼睛位置
        /// </summary>
        public static Vector3 眼睛位置(this IGameObject 游戏对象) => 
            游戏对象.GetEyePostion();
        
        /// <summary>
        /// 到玩家的距离
        /// </summary>
        public static float 距玩家距离(this IGameObject 游戏对象) => 
            游戏对象.DistanceToPlayer();
        
        /// <summary>
        /// 是否有身位
        /// </summary>
        public static bool 有身位(this IGameObject 游戏对象) => 
            游戏对象.HasPositional();
        
        /// <summary>
        /// 点是否在背后
        /// </summary>
        public static bool 在背后(this IGameObject 游戏对象, Vector3 点, bool 检查身位 = true) => 
            游戏对象.InBehind(点, 检查身位);
        
        /// <summary>
        /// 是否在目标背后
        /// </summary>
        public static bool 在目标背后(this IGameObject 游戏对象, IGameObject 目标) => 
            游戏对象.IsBehindTarget(目标);
        
        /// <summary>
        /// 获取背后扇形区域
        /// </summary>
        public static SectorShape 背后扇形区域(this IGameObject 游戏对象) => 
            游戏对象.BehindShape();
        
        /// <summary>
        /// 成为玩家目标
        /// </summary>
        public static void 成为玩家目标(this IGameObject 游戏对象) => 
            游戏对象.BecomeTargetOfLocalPlayer();
        
        /// <summary>
        /// 与目标交互
        /// </summary>
        public static bool 交互(this IGameObject 游戏对象) => 
            游戏对象.TargetInteract();
        
        /// <summary>
        /// 是否是敌人
        /// </summary>
        public static bool 是敌人(this IGameObject 游戏对象) => 
            游戏对象.IsEnemy();
        
        /// <summary>
        /// 是否在仇恨列表内
        /// </summary>
        public static bool 在仇恨列表(this IGameObject 游戏对象) => 
            游戏对象.IsInEnemiesList();
        
        /// <summary>
        /// 事件是否有效
        /// </summary>
        public static bool 事件有效(this IGameObject 游戏对象) => 
            游戏对象.EventValid();
        
        /// <summary>
        /// 从ID获取对象
        /// </summary>
        public static T? 获取游戏对象<T>(this uint 对象ID) where T : class, IGameObject => 
            对象ID.ToGameObject<T>();
        
        /// <summary>
        /// 是否在PvP中
        /// </summary>
        public static bool 在PvP中(this IGameObject 游戏对象) => 
            游戏对象.IsPvP();
        
        /// <summary>
        /// 获取名牌图标
        /// </summary>
        public static uint 名牌图标(this IGameObject 游戏对象) => 
            游戏对象.GetNamePlateIcon();
        
        /// <summary>
        /// 获取事件类型
        /// </summary>
        public static EventHandlerContent 事件类型(this IGameObject 游戏对象) => 
            游戏对象.GetEventType();
        
        /// <summary>
        /// 获取FATE ID
        /// </summary>
        public static uint FATE_ID(this IGameObject 游戏对象) => 
            游戏对象.FateId();
        
        /// <summary>
        /// 获取事件ID
        /// </summary>
        public static uint 事件ID(this IGameObject 游戏对象) => 
            游戏对象.EventId();
        
        /// <summary>
        /// 转换为日志字符串
        /// </summary>
        public static string 日志字符串(this IGameObject 游戏对象) => 
            游戏对象.ToLogString();
        
        // === IBattleChara 扩展 ===
        
        /// <summary>
        /// 是否有指定Buff
        /// </summary>
        public static bool 有Buff(this IBattleChara 单位, uint BuffID, int 剩余时间 = 0) => 
            单位.HasAura(BuffID, 剩余时间);
        
        /// <summary>
        /// 是否有玩家施加的Buff
        /// </summary>
        public static bool 有玩家Buff(this IBattleChara 单位, uint BuffID) => 
            单位.HasLocalPlayerAura(BuffID);
        
        /// <summary>
        /// 获取Buff层数
        /// </summary>
        public static int Buff层数(this IBattleChara 单位, uint BuffID) => 
            单位.GetAuraStack(BuffID);
        
        /// <summary>
        /// 是否有可驱散Buff
        /// </summary>
        public static bool 有可驱散Buff(this IBattleChara 单位) => 
            单位.HasCanDispel();
        
        /// <summary>
        /// 自己的Buff剩余时间是否大于指定值
        /// </summary>
        public static bool 自身Buff剩余时间(this IBattleChara 单位, uint BuffID, int 剩余时间 = 0) => 
            单位.HasMyAuraWithTimeleft(BuffID, 剩余时间);
        
        /// <summary>
        /// 检查即将结束的Buff
        /// </summary>
        public static bool 即将结束Buff(this IBattleChara 单位, uint BuffID, int 剩余时间 = 0) => 
            单位.ContainsMyInEndAura(BuffID, 剩余时间);
        
        /// <summary>
        /// 是否有列表中的任意Buff
        /// </summary>
        public static bool 有任意Buff(this IBattleChara 单位, List<uint> Buff列表, int 剩余时间 = 0) => 
            单位.HasAnyAura(Buff列表, 剩余时间);
        
        /// <summary>
        /// 返回命中的第一个Buff ID
        /// </summary>
        public static uint 命中Buff(this IBattleChara 单位, List<uint> Buff列表, int 剩余时间 = 0) => 
            单位.HitAnyAura(Buff列表, 剩余时间);
        
        /// <summary>
        /// 获取当前目标
        /// </summary>
        public static IBattleChara? 当前目标(this IBattleChara 单位) => 
            单位.GetCurrTarget();
        
        /// <summary>
        /// 获取目标的目标
        /// </summary>
        public static IBattleChara? 目标的目标(this IBattleChara 单位) => 
            单位.GetCurrTargetsTarget();
        
        /// <summary>
        /// 是否为有效攻击单位
        /// </summary>
        public static bool 有效攻击单位(this IBattleChara 单位) => 
            单位.ValidAttackUnit();
        
        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public static bool 可攻击(this IBattleChara 单位) => 
            单位.CanAttack();
        
        /// <summary>
        /// 是否为有效单位
        /// </summary>
        public static bool 有效单位(this IBattleChara 单位) => 
            单位.ValidUnit();
        
        /// <summary>
        /// 是否不是无敌状态
        /// </summary>
        public static bool 非无敌状态(this IBattleChara 单位) => 
            单位.NotInvulnerable();
        
        /// <summary>
        /// 是否在范围内
        /// </summary>
        public static bool 在范围内(this IBattleChara 单位, IBattleChara 目标, int 范围 = 3) => 
            单位.InRange(目标, 范围);
        
        /// <summary>
        /// 是否是Boss
        /// </summary>
        public static bool 是Boss(this IBattleChara 单位) => 
            单位.IsBoss();
        
        /// <summary>
        /// 是否是木桩
        /// </summary>
        public static bool 是木桩(this IBattleChara 单位) => 
            单位.IsDummy();
        
        /// <summary>
        /// 是否是玩家阵营
        /// </summary>
        public static bool 玩家阵营(this IBattleChara 单位) => 
            单位.IsPlayerCamp();
        
        /// <summary>
        /// 是否死亡
        /// </summary>
        public static bool 已死亡(this IBattleChara 单位) => 
            单位.IsDead();
        
        // === IPlayerCharacter 扩展 ===
        
        /// <summary>
        /// 是否在过场动画状态
        /// </summary>
        public static bool 过场动画中(this IPlayerCharacter 玩家) => 
            玩家.InCutSceneState();
        
        // === ICharacter 扩展 ===
        
        /// <summary>
        /// 当前血量百分比
        /// </summary>
        public static float 血量百分比(this ICharacter 角色) => 
            角色.CurrentHpPercent();
        
        /// <summary>
        /// 当前魔法值百分比
        /// </summary>
        public static float 蓝量百分比(this ICharacter 角色) => 
            角色.CurrentMpPercent();
        
        /// <summary>
        /// 是否是坦克
        /// </summary>
        public static bool 是坦克(this ICharacter 角色) => 
            角色.IsTank();
        
        /// <summary>
        /// 是否是治疗
        /// </summary>
        public static bool 是治疗(this ICharacter 角色) => 
            角色.IsHealer();
        
        /// <summary>
        /// 是否是DPS
        /// </summary>
        public static bool 是DPS(this ICharacter 角色) => 
            角色.IsDps();
        
        /// <summary>
        /// 是否是近战
        /// </summary>
        public static bool 是近战(this ICharacter 角色) => 
            角色.IsMelee();
        
        /// <summary>
        /// 是否是远程
        /// </summary>
        public static bool 是远程(this ICharacter 角色) => 
            角色.IsRanged();
        
        /// <summary>
        /// 是否是法系
        /// </summary>
        public static bool 是法系(this ICharacter 角色) => 
            角色.IsCaster();
        
        /// <summary>
        /// 是否在战斗中
        /// </summary>
        public static bool 战斗中(this ICharacter 角色) => 
            角色.InCombat();
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public static bool 有效(this ICharacter 角色) => 
            角色.IsValid();
        
        // === Spell 扩展 ===
        
        /// <summary>
        /// 技能是否刚使用过
        /// </summary>
        public static bool 刚使用过(this Spell 技能, int 时间 = 1200) => 
            技能.RecentlyUsed(时间);
        
        /// <summary>
        /// 从ID创建Spell对象
        /// </summary>
        public static Spell 创建技能(this uint 技能ID) => 
            技能ID.GetSpell();

        // ==================================
        // 战斗相关类型定义
        // ==================================
        
        // === Spell 类 ===
        
        /// <summary>
        /// 创建指定目标类型的技能
        /// </summary>
        public static Spell 创建技能(uint 技能ID, SpellTargetType 目标类型) => 
            new Spell(技能ID, 目标类型);
        
        /// <summary>
        /// 创建指定目标的技能
        /// </summary>
        public static Spell 创建技能(uint 技能ID, IBattleChara 目标) => 
            new Spell(技能ID, 目标);
        
        /// <summary>
        /// 创建地面目标技能
        /// </summary>
        public static Spell 创建地面技能(uint 技能ID, Vector3 位置) => 
            new Spell(技能ID, 位置);
    }
}