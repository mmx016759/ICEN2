using System.Numerics;
using ImGuiNET;

namespace ICEN2.白魔.设置.标签.小游戏
{
    public class 俄罗斯方块游戏 : I游戏接口
    {
        // 游戏配置
        private const int 格子大小 = 20;
        private const int 网格宽度 = 10;
        private const int 网格高度 = 20;
        
        // 方块形状定义 (7种经典形状)
        private static readonly List<int[,]> 方块形状 = new List<int[,]>
        {
            new int[,] { {1,1,1,1} }, // I
            new int[,] { {1,1}, {1,1} }, // O
            new int[,] { {0,1,0}, {1,1,1} }, // T
            new int[,] { {1,0,0}, {1,1,1} }, // L
            new int[,] { {0,0,1}, {1,1,1} }, // J
            new int[,] { {0,1,1}, {1,1,0} }, // S
            new int[,] { {1,1,0}, {0,1,1} }  // Z
        };
        
        // 方块颜色
        private static readonly Vector4[] 方块颜色 = new Vector4[]
        {
            new Vector4(0, 1, 1, 1),     // I - 青色
            new Vector4(1, 1, 0, 1),     // O - 黄色
            new Vector4(0.8f, 0, 0.8f, 1), // T - 紫色
            new Vector4(1, 0.5f, 0, 1),  // L - 橙色
            new Vector4(0, 0, 1, 1),     // J - 蓝色
            new Vector4(0, 1, 0, 1),     // S - 绿色
            new Vector4(1, 0, 0, 1)      // Z - 红色
        };
        
        // 游戏状态
        private int[,] 游戏网格 = new int[网格宽度, 网格高度];
        private int 当前方块类型;
        private int 下一个方块类型;
        private Vector2 当前方块位置 = new Vector2(4, 0);
        private int 当前旋转状态 = 0;
        private float 下落计时器 = 0f;
        private float 下落间隔 = 0.5f;
        private bool 游戏结束 = false;
        private int 分数 = 0;
        private int 已消除行数 = 0;
        private float 速度系数 = 1f; // 默认速度
        private Random 随机 = new Random();

        public 俄罗斯方块游戏()
        {
            重置游戏();
        }

        private void 重置游戏()
        {
            // 重置游戏状态
            for (int y = 0; y < 网格高度; y++)
                for (int x = 0; x < 网格宽度; x++)
                    游戏网格[x, y] = -1; // -1 表示空
            
            当前方块类型 = 随机.Next(0, 方块形状.Count);
            下一个方块类型 = 随机.Next(0, 方块形状.Count);
            当前方块位置 = new Vector2(4, 0);
            当前旋转状态 = 0;
            游戏结束 = false;
            分数 = 0;
            已消除行数 = 0;
            速度系数 = 1f;
            下落间隔 = 0.5f;
        }

        public void 绘制()
        {
            // 游戏标题和状态
            ImGui.Text("俄罗斯方块");
            if (游戏结束)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "游戏结束! 分数: " + 分数);
            }
            else
            {
                ImGui.SameLine();
                ImGui.Text("分数: " + 分数);
            }
            
            // 使用列布局确保游戏区域和预览区不重叠
            ImGui.Columns(2, "游戏布局", false);
            
            // 第一列: 游戏区域
            ImGui.SetColumnWidth(0, 网格宽度 * 格子大小 + 30); // 游戏区域宽度 + 边距
            ImGui.BeginChild("游戏区域", new Vector2(0, 0), true);
            
            // 游戏区域标题
            ImGui.Text("游戏区域");
            
            // 绘制主游戏区域
            绘制游戏区域();
            
            ImGui.EndChild();
            
            // 第二列: 控制面板和预览
            ImGui.NextColumn();
            ImGui.BeginChild("控制面板", new Vector2(0, 0), true);
            
            // 下一个方块预览
            ImGui.Text("下一个方块:");
            绘制下一个方块预览();
            
            // 游戏统计
            ImGui.Separator();
            ImGui.Text("游戏统计:");
            ImGui.Text($"已消除行数: {已消除行数}");
            ImGui.Text($"当前速度: x{速度系数:0.0}");
            
            // 游戏速度调节
            ImGui.Separator();
            ImGui.Text("游戏速度:");
            if (ImGui.SliderFloat("##speed", ref 速度系数, 0.5f, 5f, "x%.1f"))
            {
                下落间隔 = 0.5f / 速度系数;
            }
            
            // 控制按钮
            ImGui.Separator();
            ImGui.Text("控制按钮:");
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.8f, 1));
            if (ImGui.Button("左移", new Vector2(80, 30))) 移动方块(-1, 0);
            ImGui.SameLine();
            if (ImGui.Button("右移", new Vector2(80, 30))) 移动方块(1, 0);
            ImGui.PopStyleColor();
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.4f, 0.4f, 1));
            if (ImGui.Button("旋转", new Vector2(80, 30))) 旋转方块();
            ImGui.SameLine();
            if (ImGui.Button("下落", new Vector2(80, 30))) 移动方块(0, 1);
            ImGui.PopStyleColor();
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.8f, 0.4f, 1));
            if (ImGui.Button("直接落下", new Vector2(165, 30))) 硬降();
            ImGui.PopStyleColor();
            
            // 重新开始按钮
            ImGui.Separator();
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.2f, 0.2f, 1));
            if (ImGui.Button("重新开始", new Vector2(165, 40)))
            {
                重置游戏();
            }
            ImGui.PopStyleColor();
            
            // 游戏说明
            ImGui.Separator();
            ImGui.Text("游戏说明:");
            ImGui.TextWrapped("• 消除一行: 100分");
            ImGui.TextWrapped("• 同时消除多行有额外奖励");
            ImGui.TextWrapped("• 方块直接落下: +2分/格");
            
            ImGui.EndChild();
            
            ImGui.Columns(1); // 结束列布局
            
            // 更新游戏逻辑
            if (!游戏结束)
            {
                更新游戏逻辑();
            }
        }
        
        private void 更新游戏逻辑()
        {
            // 更新下落计时器
            下落计时器 += ImGui.GetIO().DeltaTime;
            if (下落计时器 >= 下落间隔)
            {
                下落计时器 = 0;
                移动方块(0, 1);
            }
        }
        
        private void 绘制游戏区域()
        {
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 区域大小 = new Vector2(网格宽度 * 格子大小, 网格高度 * 格子大小);
            
            // 绘制背景和边框
            uint 背景色 = ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 0.8f));
            uint 边框色 = ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 1));
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 区域大小, 背景色);
            绘制列表.AddRect(窗口位置, 窗口位置 + 区域大小, 边框色, 0, ImDrawFlags.None, 2f);
            
            // 绘制网格线
            uint 网格线颜色 = ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
            for (int x = 0; x <= 网格宽度; x++)
            {
                var 起点 = 窗口位置 + new Vector2(x * 格子大小, 0);
                var 终点 = 起点 + new Vector2(0, 区域大小.Y);
                绘制列表.AddLine(起点, 终点, 网格线颜色);
            }
            for (int y = 0; y <= 网格高度; y++)
            {
                var 起点 = 窗口位置 + new Vector2(0, y * 格子大小);
                var 终点 = 起点 + new Vector2(区域大小.X, 0);
                绘制列表.AddLine(起点, 终点, 网格线颜色);
            }
            
            // 绘制已固定的方块
            for (int y = 0; y < 网格高度; y++)
            {
                for (int x = 0; x < 网格宽度; x++)
                {
                    if (游戏网格[x, y] >= 0)
                    {
                        var 颜色 = 方块颜色[游戏网格[x, y]];
                        绘制方块(x, y, 颜色, 绘制列表, 窗口位置);
                    }
                }
            }
            
            // 绘制当前下落的方块
            var 当前颜色 = 方块颜色[当前方块类型];
            var 当前方块 = 获取旋转后的方块();
            for (int y = 0; y < 当前方块.GetLength(0); y++)
            {
                for (int x = 0; x < 当前方块.GetLength(1); x++)
                {
                    if (当前方块[y, x] == 1)
                    {
                        int 格子X = (int)当前方块位置.X + x;
                        int 格子Y = (int)当前方块位置.Y + y;
                        
                        if (格子Y >= 0) // 只绘制在网格内的部分
                        {
                            绘制方块(格子X, 格子Y, 当前颜色, 绘制列表, 窗口位置);
                        }
                    }
                }
            }
            
            // 设置光标位置以便继续绘制其他内容
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 区域大小.Y + 5));
        }
        
        private void 绘制下一个方块预览()
        {
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 预览大小 = new Vector2(100, 100); // 固定预览区域大小
            
            // 绘制预览区域背景
            uint 背景色 = ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 0.5f));
            uint 边框色 = ImGui.GetColorU32(ImGuiCol.Border);
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 预览大小, 背景色);
            绘制列表.AddRect(窗口位置, 窗口位置 + 预览大小, 边框色);
            
            // 绘制下一个方块
            var 方块 = 方块形状[下一个方块类型];
            var 颜色 = 方块颜色[下一个方块类型];
            
            // 居中显示
            int 方块宽度 = 方块.GetLength(1) * 格子大小;
            int 方块高度 = 方块.GetLength(0) * 格子大小;
            Vector2 居中偏移 = new Vector2(
                (预览大小.X - 方块宽度) / 2,
                (预览大小.Y - 方块高度) / 2
            );
            
            for (int y = 0; y < 方块.GetLength(0); y++)
            {
                for (int x = 0; x < 方块.GetLength(1); x++)
                {
                    if (方块[y, x] == 1)
                    {
                        var 位置 = 窗口位置 + 居中偏移 + new Vector2(x * 格子大小, y * 格子大小);
                        绘制列表.AddRectFilled(位置, 位置 + new Vector2(格子大小, 格子大小), 
                            ImGui.GetColorU32(颜色));
                        绘制列表.AddRect(位置, 位置 + new Vector2(格子大小, 格子大小), 
                            ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
                    }
                }
            }
            
            // 设置光标位置以便继续绘制其他内容
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 预览大小.Y + 10));
        }
        
        private void 绘制方块(int x, int y, Vector4 color, ImDrawListPtr 绘制列表, Vector2 基准位置)
        {
            var 位置 = 基准位置 + new Vector2(x * 格子大小, y * 格子大小);
            
            // 绘制方块主体
            绘制列表.AddRectFilled(位置, 位置 + new Vector2(格子大小, 格子大小), 
                ImGui.GetColorU32(color));
            
            // 绘制方块边框
            绘制列表.AddRect(位置, 位置 + new Vector2(格子大小, 格子大小), 
                ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
            
            // 绘制内部高光效果
            var 高光位置 = 位置 + new Vector2(2, 2);
            绘制列表.AddRectFilled(高光位置, 位置 + new Vector2(格子大小 - 2, 格子大小 - 2), 
                ImGui.GetColorU32(new Vector4(1, 1, 1, 0.2f)));
        }
        
        private void 移动方块(int dx, int dy)
        {
            if (游戏结束) return;
            
            // 尝试移动
            Vector2 新位置 = 当前方块位置 + new Vector2(dx, dy);
            
            if (可以放置(新位置, 获取旋转后的方块()))
            {
                当前方块位置 = 新位置;
                
                // 如果是玩家手动下落，增加分数
                if (dy > 0) 分数 += 2;
            }
            else if (dy > 0) // 如果是向下移动且不可移动，则固定方块
            {
                固定当前方块();
                生成新方块();
                检查消除行();
            }
        }
        
        private void 旋转方块()
        {
            if (游戏结束) return;
            
            int 新旋转 = (当前旋转状态 + 1) % 4;
            var 旋转后的方块 = 旋转方块(方块形状[当前方块类型], 新旋转);
            
            if (可以放置(当前方块位置, 旋转后的方块))
            {
                当前旋转状态 = 新旋转;
            }
        }
        
        private void 硬降()
        {
            if (游戏结束) return;
            
            // 快速下落直到不能移动
            while (可以放置(当前方块位置 + new Vector2(0, 1), 获取旋转后的方块()))
            {
                当前方块位置.Y += 1;
                分数 += 2; // 每下落一格加2分
            }
            
            固定当前方块();
            生成新方块();
            检查消除行();
        }
        
        private void 固定当前方块()
        {
            var 方块 = 获取旋转后的方块();
            
            for (int y = 0; y < 方块.GetLength(0); y++)
            {
                for (int x = 0; x < 方块.GetLength(1); x++)
                {
                    if (方块[y, x] == 1)
                    {
                        int 格子X = (int)当前方块位置.X + x;
                        int 格子Y = (int)当前方块位置.Y + y;
                        
                        if (格子Y >= 0) // 确保在网格内
                        {
                            游戏网格[格子X, 格子Y] = 当前方块类型;
                        }
                    }
                }
            }
        }
        
        private void 生成新方块()
        {
            当前方块类型 = 下一个方块类型;
            下一个方块类型 = 随机.Next(0, 方块形状.Count);
            当前方块位置 = new Vector2(4, 0);
            当前旋转状态 = 0;
            
            // 检查游戏是否结束
            if (!可以放置(当前方块位置, 获取旋转后的方块()))
            {
                游戏结束 = true;
            }
        }
        
        private void 检查消除行()
        {
            int 消除行数 = 0;
            
            for (int y = 网格高度 - 1; y >= 0; y--)
            {
                bool 行满 = true;
                
                for (int x = 0; x < 网格宽度; x++)
                {
                    if (游戏网格[x, y] < 0)
                    {
                        行满 = false;
                        break;
                    }
                }
                
                if (行满)
                {
                    消除行(y);
                    消除行数++;
                    y++; // 重新检查当前行（因为上面的行下移了）
                }
            }
            
            if (消除行数 > 0)
            {
                // 计算分数
                int 基础分数 = 100;
                int 奖励 = (消除行数 - 1) * 50; // 同时消除多行有额外奖励
                分数 += 基础分数 * 消除行数 + 奖励;
                已消除行数 += 消除行数;
            }
        }
        
        private void 消除行(int 行)
        {
            // 上方的行下移
            for (int y = 行; y > 0; y--)
            {
                for (int x = 0; x < 网格宽度; x++)
                {
                    游戏网格[x, y] = 游戏网格[x, y - 1];
                }
            }
            
            // 清空最顶行
            for (int x = 0; x < 网格宽度; x++)
            {
                游戏网格[x, 0] = -1;
            }
        }
        
        private bool 可以放置(Vector2 位置, int[,] 方块)
        {
            for (int y = 0; y < 方块.GetLength(0); y++)
            {
                for (int x = 0; x < 方块.GetLength(1); x++)
                {
                    if (方块[y, x] == 1)
                    {
                        int 格子X = (int)位置.X + x;
                        int 格子Y = (int)位置.Y + y;
                        
                        // 检查边界
                        if (格子X < 0 || 格子X >= 网格宽度 || 格子Y >= 网格高度)
                            return false;
                        
                        // 检查是否与已固定的方块重叠
                        if (格子Y >= 0 && 游戏网格[格子X, 格子Y] >= 0)
                            return false;
                    }
                }
            }
            return true;
        }
        
        private int[,] 获取旋转后的方块()
        {
            return 旋转方块(方块形状[当前方块类型], 当前旋转状态);
        }
        
        private int[,] 旋转方块(int[,] 原始方块, int 旋转)
        {
            int 行数 = 原始方块.GetLength(0);
            int 列数 = 原始方块.GetLength(1);
            int[,] 结果;
            
            switch (旋转 % 4)
            {
                case 0: // 0度
                    return (int[,])原始方块.Clone();
                case 1: // 90度
                    结果 = new int[列数, 行数];
                    for (int y = 0; y < 行数; y++)
                        for (int x = 0; x < 列数; x++)
                            结果[x, 行数 - 1 - y] = 原始方块[y, x];
                    return 结果;
                case 2: // 180度
                    结果 = new int[行数, 列数];
                    for (int y = 0; y < 行数; y++)
                        for (int x = 0; x < 列数; x++)
                            结果[行数 - 1 - y, 列数 - 1 - x] = 原始方块[y, x];
                    return 结果;
                case 3: // 270度
                    结果 = new int[列数, 行数];
                    for (int y = 0; y < 行数; y++)
                        for (int x = 0; x < 列数; x++)
                            结果[列数 - 1 - x, y] = 原始方块[y, x];
                    return 结果;
                default:
                    return (int[,])原始方块.Clone();
            }
        }
    }
}