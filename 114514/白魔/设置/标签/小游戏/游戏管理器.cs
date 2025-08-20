namespace ICEN2.白魔.设置.标签.小游戏
{
    public static class 游戏管理器
    {
        public enum 游戏类型
        {
            贪吃蛇,
            俄罗斯方块,
            扫雷,
            井字棋,
            推箱子,
            华容道,
            数字拼图2048,
            数独,
            无
        }

        public static 游戏类型 当前游戏 = 游戏类型.无;
        private static Dictionary<游戏类型, I游戏接口> 游戏实例 = new Dictionary<游戏类型, I游戏接口>
        {
            { 游戏类型.贪吃蛇, new 贪吃蛇游戏() },
            { 游戏类型.俄罗斯方块, new 俄罗斯方块游戏() },
            { 游戏类型.扫雷, new 扫雷游戏() },
            { 游戏类型.井字棋, new 井字棋游戏() },
            { 游戏类型.推箱子, new 推箱子游戏() },
            { 游戏类型.华容道, new 华容道游戏() },
            { 游戏类型.数字拼图2048, new 数字拼图2048() },
            { 游戏类型.数独, new 数独游戏() }
        };

        public static void 绘制当前游戏()
        {
            if (当前游戏 != 游戏类型.无 && 游戏实例.ContainsKey(当前游戏))
            {
                游戏实例[当前游戏].绘制();
            }
        }
    }

    public interface I游戏接口
    {
        void 绘制();
    }
}