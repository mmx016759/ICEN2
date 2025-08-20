using System.Collections.Concurrent;

namespace icen.白魔.Utilities;

public class WhiteMageBattleData
{
    public static ConcurrentDictionary<string,long> 技能内置cd = new ConcurrentDictionary<string,long>();
    
    public static WhiteMageBattleData Instance = new();
    public int LastHealedTargetId;
    public int LilyStacks;
}