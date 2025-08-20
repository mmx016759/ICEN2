using System.Collections.Concurrent;

namespace ICEN2.白魔.设置;

public class WhiteMageBattleData
{
    public static ConcurrentDictionary<string,long> 技能内置cd = new ConcurrentDictionary<string,long>();
    
    public static WhiteMageBattleData Instance = new();
    public int LastHealedTargetId;
    public int LilyStacks;
}