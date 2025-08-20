using AEAssist.CombatRoutine.Module.Opener;
using icen.白魔.Utilities.设置;

namespace icen.白魔.起手
{
    public static class 起手
    {
        private static readonly IOpener 白魔正常起手 = new 白魔正常起手();
        private static readonly IOpener 白魔空起手 = new 白魔空起手();
        
        public static IOpener? GetOpener(uint level)
        {
            if (level < 60)
            {
                return null;
            }

            return 默认值.实例.起手设置 == 2 ? 白魔空起手 : 白魔正常起手;
        }
    }
}
