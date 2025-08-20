using System.Numerics;
using ImGuiNET;

namespace icen.白魔.Utilities.标签.小游戏
{
    public class 数独游戏 : I游戏接口
    {
        private const int 格子大小 = 40;
        private const int 网格大小 = 9;
        private int[,] 网格 = new int[网格大小, 网格大小];
        private int[,] 答案 = new int[网格大小, 网格大小]; // 存储完整答案
        private bool[,] 固定格子 = new bool[网格大小, 网格大小];
        private bool[,] 错误格子 = new bool[网格大小, 网格大小]; // 标记错误格子
        private int 选中X = -1, 选中Y = -1;
        private bool 完成 = false;
        private Random 随机 = new Random();

        public 数独游戏()
        {
            生成新游戏();
        }

        public void 绘制()
        {
            ImGui.Text("数独");
            ImGui.Separator();
            
            if (完成)
            {
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "恭喜完成!");
            }
            
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 游戏区域大小 = new Vector2(网格大小 * 格子大小, 网格大小 * 格子大小);
            
            // 绘制背景
            uint 背景色 = ImGui.GetColorU32(new Vector4(1, 1, 1, 1));
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 游戏区域大小, 背景色);
            
            // 绘制粗网格线
            uint 粗线颜色 = ImGui.GetColorU32(new Vector4(0, 0, 0, 1));
            for (int i = 0; i <= 3; i++)
            {
                float 位置 = i * 3 * 格子大小;
                
                // 垂直线
                var 起点 = 窗口位置 + new Vector2(位置, 0);
                var 终点 = 起点 + new Vector2(0, 游戏区域大小.Y);
                绘制列表.AddLine(起点, 终点, 粗线颜色, 2f);
                
                // 水平线
                起点 = 窗口位置 + new Vector2(0, 位置);
                终点 = 起点 + new Vector2(游戏区域大小.X, 0);
                绘制列表.AddLine(起点, 终点, 粗线颜色, 2f);
            }
            
            // 绘制细网格线
            uint 细线颜色 = ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 1));
            for (int i = 1; i < 网格大小; i++)
            {
                if (i % 3 == 0) continue;
                
                float 位置 = i * 格子大小;
                
                // 垂直线
                var 起点 = 窗口位置 + new Vector2(位置, 0);
                var 终点 = 起点 + new Vector2(0, 游戏区域大小.Y);
                绘制列表.AddLine(起点, 终点, 细线颜色);
                
                // 水平线
                起点 = 窗口位置 + new Vector2(0, 位置);
                终点 = 起点 + new Vector2(游戏区域大小.X, 0);
                绘制列表.AddLine(起点, 终点, 细线颜色);
            }
            
            // 绘制数字和选中框
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    var 格子位置 = 窗口位置 + new Vector2(x * 格子大小, y * 格子大小);
                    
                    // 绘制透明按钮用于选择格子
                    ImGui.SetCursorScreenPos(格子位置);
                    string 按钮ID = $"##格子_{x}_{y}";
                    if (ImGui.InvisibleButton(按钮ID, new Vector2(格子大小, 格子大小))) 
                    {
                        选中X = x;
                        选中Y = y;
                    }
                    
                    // 绘制选中框
                    if (x == 选中X && y == 选中Y)
                    {
                        绘制列表.AddRect(格子位置, 格子位置 + new Vector2(格子大小, 格子大小), 
                            ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), 0, 0, 2f);
                    }
                    
                    // 绘制数字
                    if (网格[y, x] > 0)
                    {
                        var 文本 = 网格[y, x].ToString();
                        var 文本大小 = ImGui.CalcTextSize(文本);
                        var 文本位置 = 格子位置 + new Vector2(
                            (格子大小 - 文本大小.X) / 2,
                            (格子大小 - 文本大小.Y) / 2
                        );
                        
                        // 根据格子类型选择颜色
                        Vector4 颜色;
                        if (固定格子[y, x])
                            颜色 = new Vector4(0, 0, 0, 1); // 固定格子：黑色
                        else if (错误格子[y, x])
                            颜色 = new Vector4(1, 0, 0, 1); // 错误格子：红色
                        else
                            颜色 = new Vector4(0, 0, 1, 1); // 玩家输入：蓝色
                        
                        绘制列表.AddText(文本位置, ImGui.GetColorU32(颜色), 文本);
                    }
                }
            }
            
            // 设置光标位置
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 游戏区域大小.Y + 10));
            
            // 数字按钮
            ImGui.Text("选择数字:");
            for (int i = 1; i <= 9; i++)
            {
                if (ImGui.Button($"{i}", new Vector2(40, 40)))
                {
                    if (选中X >= 0 && 选中Y >= 0 && !固定格子[选中Y, 选中X])
                    {
                        // 清除错误标记
                        错误格子[选中Y, 选中X] = false;
                        网格[选中Y, 选中X] = i;
                        检查完成();
                    }
                }
                if (i % 3 != 0) ImGui.SameLine();
            }
            
            // 控制按钮
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.8f, 0.4f, 1));
            if (ImGui.Button("新游戏", new Vector2(120, 40)))
            {
                生成新游戏();
            }
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.8f, 0.4f, 1));
            if (ImGui.Button("提示", new Vector2(80, 40)))
            {
                提供提示();
            }
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.4f, 0.4f, 1));
            if (ImGui.Button("清空", new Vector2(80, 40)))
            {
                if (选中X >= 0 && 选中Y >= 0 && !固定格子[选中Y, 选中X])
                {
                    // 清除错误标记
                    错误格子[选中Y, 选中X] = false;
                    网格[选中Y, 选中X] = 0;
                }
            }
            ImGui.PopStyleColor();
        }
        
        private void 提供提示()
        {
            if (选中X < 0 || 选中Y < 0) return;
            if (固定格子[选中Y, 选中X]) return; // 固定格子不提供提示
            
            // 获取正确答案
            int 正确答案 = 答案[选中Y, 选中X];
            
            // 如果玩家已填写但错误，标红
            if (网格[选中Y, 选中X] > 0 && 网格[选中Y, 选中X] != 正确答案)
            {
                错误格子[选中Y, 选中X] = true; // 标记为错误
                return;
            }
            
            // 填入正确答案
            网格[选中Y, 选中X] = 正确答案;
            错误格子[选中Y, 选中X] = false; // 清除错误标记
            检查完成();
        }
        
        // 生成随机数独谜题
        private void 生成新游戏()
        {
            // 初始化数组
            网格 = new int[网格大小, 网格大小];
            固定格子 = new bool[网格大小, 网格大小];
            错误格子 = new bool[网格大小, 网格大小];
            完成 = false;
            
            // 生成完整数独答案
            生成完整数独(答案);
            
            // 复制答案并随机移除部分数字
            int 保留数量 = 35; // 保留约40%的数字作为提示
            List<(int, int)> 位置列表 = new List<(int, int)>();
            
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    位置列表.Add((x, y));
                }
            }
            
            // 随机打乱位置
            for (int i = 0; i < 位置列表.Count; i++)
            {
                int j = 随机.Next(i, 位置列表.Count);
                var temp = 位置列表[i];
                位置列表[i] = 位置列表[j];
                位置列表[j] = temp;
            }
            
            // 保留前N个位置
            for (int i = 0; i < 保留数量; i++)
            {
                var (x, y) = 位置列表[i];
                网格[y, x] = 答案[y, x];
                固定格子[y, x] = true;
            }
            
            选中X = -1;
            选中Y = -1;
        }
        
        // 生成完整数独算法
        private void 生成完整数独(int[,] 板)
        {
            // 初始化板
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    板[y, x] = 0;
                }
            }
            
            // 填充对角线的三个3x3方块（它们相互独立）
            for (int i = 0; i < 3; i++)
            {
                填充方块(板, i * 3, i * 3);
            }
            
            // 解决剩余部分
            解决数独(板);
        }
        
        private void 填充方块(int[,] 板, int 行, int 列)
        {
            List<int> 数字 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            
            // 随机打乱数字
            for (int i = 0; i < 9; i++)
            {
                int j = 随机.Next(i, 9);
                int temp = 数字[i];
                数字[i] = 数字[j];
                数字[j] = temp;
            }
            
            int index = 0;
            for (int y = 行; y < 行 + 3; y++)
            {
                for (int x = 列; x < 列 + 3; x++)
                {
                    板[y, x] = 数字[index++];
                }
            }
        }
        
        private bool 解决数独(int[,] 板)
        {
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if (板[y, x] == 0)
                    {
                        List<int> 候选数字 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        
                        // 随机打乱候选数字
                        for (int i = 0; i < 候选数字.Count; i++)
                        {
                            int j = 随机.Next(i, 候选数字.Count);
                            int temp = 候选数字[i];
                            候选数字[i] = 候选数字[j];
                            候选数字[j] = temp;
                        }
                        
                        foreach (int 数字 in 候选数字)
                        {
                            if (验证位置(板, x, y, 数字))
                            {
                                板[y, x] = 数字;
                                
                                if (解决数独(板))
                                {
                                    return true;
                                }
                                
                                板[y, x] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }
        
        private bool 验证位置(int[,] 板, int x, int y, int 数字)
        {
            // 检查行
            for (int i = 0; i < 网格大小; i++)
            {
                if (板[y, i] == 数字) return false;
            }
            
            // 检查列
            for (int i = 0; i < 网格大小; i++)
            {
                if (板[i, x] == 数字) return false;
            }
            
            // 检查3x3方块
            int 方块行 = y / 3 * 3;
            int 方块列 = x / 3 * 3;
            
            for (int i = 方块行; i < 方块行 + 3; i++)
            {
                for (int j = 方块列; j < 方块列 + 3; j++)
                {
                    if (板[i, j] == 数字) return false;
                }
            }
            
            return true;
        }
        
        private void 检查完成()
        {
            // 检查是否有空单元格
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if (网格[y, x] == 0)
                    {
                        return;
                    }
                }
            }
            
            // 检查行
            for (int y = 0; y < 网格大小; y++)
            {
                bool[] 存在 = new bool[10];
                for (int x = 0; x < 网格大小; x++)
                {
                    int 数字 = 网格[y, x];
                    if (存在[数字])
                    {
                        return;
                    }
                    存在[数字] = true;
                }
            }
            
            // 检查列
            for (int x = 0; x < 网格大小; x++)
            {
                bool[] 存在 = new bool[10];
                for (int y = 0; y < 网格大小; y++)
                {
                    int 数字 = 网格[y, x];
                    if (存在[数字])
                    {
                        return;
                    }
                    存在[数字] = true;
                }
            }
            
            // 检查3x3方块
            for (int 块Y = 0; 块Y < 3; 块Y++)
            {
                for (int 块X = 0; 块X < 3; 块X++)
                {
                    bool[] 存在 = new bool[10];
                    for (int y = 块Y * 3; y < 块Y * 3 + 3; y++)
                    {
                        for (int x = 块X * 3; x < 块X * 3 + 3; x++)
                        {
                            int 数字 = 网格[y, x];
                            if (存在[数字])
                            {
                                return;
                            }
                            存在[数字] = true;
                        }
                    }
                }
            }
            
            完成 = true;
        }
    }
}