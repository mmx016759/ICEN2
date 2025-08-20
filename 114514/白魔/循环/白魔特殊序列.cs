using AEAssist.CombatRoutine.Module;
using ICEN2.白魔.循环.特殊策列;

namespace ICEN2.白魔.循环;

public static class 白魔特殊序列
{
    public static ISlotSequence[] Build()
    {
        return
        [
            new 即刻复活(),
            new 白魔有队员需要奶满()
        ];
    }
}