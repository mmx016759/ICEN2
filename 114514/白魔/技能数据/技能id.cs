using AEAssist;

namespace ICEN2.白魔.技能数据;

public class 技能id
{
    public const uint 飞石 = 119;
    public const uint 治疗 = 120;
    public const uint 疾风 = 121;
    public const uint 医治 = 124;
    public const uint 复活 = 125;
    public const uint 坚石 = 127;
    public const uint 烈风 = 132;
    public const uint 医济 = 133;
    public const uint 救疗 = 135;
    public const uint 沉静 = 16560;
    public const uint 康复 = 7568;
    public const uint 醒梦 = 7562;
    public const uint 即刻咏唱 = 7561;
    public const uint 沉稳咏唱 = 7559;
    public const uint 营救 = 7571;
    public const uint 神速咏唱 = 136;
    public const uint 再生 = 137;
    public const uint 愈疗 = 131;
    public const uint 以太变移 = 37008;
    public const uint 神圣 = 139;
    public const uint 天赐祝福 = 140;
    public const uint 庇护所 = 3569;
    public const uint 安慰之心 = 16531;
    public const uint 垒石 = 3568;
    public const uint 法令 = 3571;
    public const uint 无中生有 = 7430;
    public const uint 神名 = 3570;
    public const uint 崩石 = 7431;
    public const uint 神祝祷 = 7432;
    public const uint 全大赦 = 7433;
    public const uint 天辉 = 16532;
    public const uint 闪耀 = 16533;
    public const uint 苦难之心 = 16535;
    public const uint 狂喜之心 = 16534;
    public const uint 节制 = 16536;
    public const uint 礼仪之铃 = 25862;
    public const uint 闪飒 = 37009;
    public const uint 医养 = 37010;
    public const uint 神爱抚 = 37011;
    public const uint 闪灼 = 25859;
    public const uint 豪圣 = 25860;
    public const uint 水流幕 = 25861;
    public static uint Aoe =>
        Core.Me.Level >= 82 ? 豪圣 : 神圣;
    public static uint 单奶 =>
        Core.Me.Level >= 30 ? 救疗 : 治疗;

    public static uint 群奶 =>
        Core.Me.Level >= 96 ? 医养 : 医济;
    public static uint 单体gcd =>
        Core.Me.Level >= 82 ? 闪灼 :          // 82级使用闪灼
        Core.Me.Level >= 72 ? 闪耀 :          // 72级使用闪耀
        Core.Me.Level >= 64 ? 崩石 :          // 64级使用崩石
        Core.Me.Level >= 54 ? 垒石 :          // 54级使用垒石
        Core.Me.Level >= 18 ? 坚石 :          // 18级使用坚石
        飞石;                                   // 默认使用飞石（1级）
    public static uint DOT =>
        Core.Me.Level >= 72 ? 天辉 :          
        Core.Me.Level >= 46 ? 烈风 :          
        疾风;                                   
}