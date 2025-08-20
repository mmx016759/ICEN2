using AEAssist.CombatRoutine.Module;
using icen.白魔.循环.GCD;
using icen.白魔.循环.能力技;

namespace icen.白魔.循环;

public static class 白魔技能策略
{
    public static readonly List<SlotResolverData> SlotResolvers =
    [
        new(new 再生(), SlotMode.Gcd),
        
        new(new 醒梦(), SlotMode.OffGcd),
        new(new 神爱抚(), SlotMode.OffGcd),
        new(new 全大赦(), SlotMode.OffGcd),
        new(new 法令(), SlotMode.OffGcd),
        new(new 铃铛(), SlotMode.OffGcd),
        new(new 庇护所(), SlotMode.OffGcd),
        new(new 翅膀(), SlotMode.OffGcd),
        new(new 狂喜之心(), SlotMode.Gcd),
        new(new 无中生有(), SlotMode.OffGcd),
        new(new 愈疗(), SlotMode.Gcd),
        new(new 群体治疗(), SlotMode.Gcd),
        new(new 医济(), SlotMode.Gcd),
        new(new 天赐(), SlotMode.OffGcd),
        new(new 水流幕(), SlotMode.OffGcd),
        new(new 安慰之心(), SlotMode.Gcd),
        new(new 神明(), SlotMode.OffGcd),
        new(new 神祝祷(), SlotMode.OffGcd),
        new(new 单体治疗(), SlotMode.Gcd),
        new(new 低级本单体治疗(), SlotMode.Gcd),
        new(new 康复(), SlotMode.Gcd),
        new(new DOt(), SlotMode.Gcd),
        new(new DOt2(), SlotMode.Gcd),
        new(new 神速咏唱(), SlotMode.OffGcd),
        new(new 红花(), SlotMode.Gcd),
        new(new 闪飒(), SlotMode.Gcd),
        new(new AOE(), SlotMode.Gcd),
        new(new 单体输出(), SlotMode.Gcd),
        
        new (new 止损(), SlotMode.Gcd),
        //白魔自动跟随
    ];
}