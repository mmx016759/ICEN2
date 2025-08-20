using System.Numerics;
using ImGuiNET;

namespace icen.白魔.Utilities.标签.小游戏
{
    public class 数字拼图2048 : I游戏接口
    {
        private const int 格子大小 = 70; // 增加格子大小
        private const int 网格大小 = 4;
        private int[,] 网格 = new int[网格大小, 网格大小];
        private int 分数 = 0;
        private int 最高分 = 0;
        private bool 游戏结束 = false;
        private bool 胜利 = false;
        private Random 随机 = new Random();

        public 数字拼图2048()
        {
            重置游戏();
        }

        public void 绘制()
        {
            // 标题和分数显示
            ImGui.Text("2048");
            ImGui.Separator();
            
            // 分数显示
            ImGui.Text($"分数: {分数}");
            ImGui.Text($"最高分: {最高分}");
            
            if (游戏结束)
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "游戏结束! 最终分数: " + 分数);
            }
            else if (胜利)
            {
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "恭喜胜利! 分数: " + 分数);
            }
            
            // 游戏区域绘制
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 游戏区域大小 = new Vector2(网格大小 * 格子大小, 网格大小 * 格子大小);
            
            // 绘制背景
            uint 背景色 = ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 1));
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 游戏区域大小, 背景色);
            
            // 绘制网格
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    var 格子位置 = 窗口位置 + new Vector2(x * 格子大小, y * 格子大小);
                    var 数字 = 网格[y, x];
                    var 颜色 = 获取数字颜色(数字);
                    
                    // 绘制格子背景
                    绘制列表.AddRectFilled(格子位置 + new Vector2(2, 2), 
                        格子位置 + new Vector2(格子大小 - 2, 格子大小 - 2), 
                        ImGui.GetColorU32(颜色));
                    
                    // 绘制数字
                    if (数字 > 0)
                    {
                        var 文本 = 数字.ToString();
                        var 文本大小 = ImGui.CalcTextSize(文本);
                        var 文本位置 = 格子位置 + new Vector2(
                            (格子大小 - 文本大小.X) / 2,
                            (格子大小 - 文本大小.Y) / 2
                        );
                        绘制列表.AddText(文本位置, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)), 文本);
                    }
                    
                    // 绘制网格线
                    绘制列表.AddRect(格子位置, 格子位置 + new Vector2(格子大小, 格子大小), 
                        ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1)));
                }
            }
            
            // 设置光标位置
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 游戏区域大小.Y + 20));
            
            // 方向控制按钮（十字布局）
            float 按钮大小 = 50;
            float 总宽度 = 按钮大小 * 3;
            float 起始X = (ImGui.GetContentRegionAvail().X - 总宽度) / 2;
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.8f, 1));
            
            // 上
            ImGui.SetCursorPosX(起始X + 按钮大小);
            if (ImGui.Button("↑", new Vector2(按钮大小, 按钮大小))) 移动(0);
            
            // 左、下、右
            ImGui.SetCursorPosX(起始X);
            if (ImGui.Button("←", new Vector2(按钮大小, 按钮大小))) 移动(2);
            ImGui.SameLine();
            if (ImGui.Button("↓", new Vector2(按钮大小, 按钮大小))) 移动(1);
            ImGui.SameLine();
            if (ImGui.Button("→", new Vector2(按钮大小, 按钮大小))) 移动(3);
            
            ImGui.PopStyleColor();
            
            // 游戏控制按钮
            ImGui.Spacing();
            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - 120) / 2);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.8f, 0.4f, 1));
            if (ImGui.Button("重新开始", new Vector2(120, 40)))
            {
                重置游戏();
            }
            ImGui.PopStyleColor();
        }
        
        private bool 移动(int 方向)
        {
            if (游戏结束 || 胜利) return false;
            
            bool 已移动 = false;
            int[,] 旧网格 = (int[,])网格.Clone();
            
            // 0: 上, 1: 下, 2: 左, 3: 右
            for (int i = 0; i < 网格大小; i++)
            {
                List<int> 非零元素 = new List<int>();
                
                // 收集非零元素
                for (int j = 0; j < 网格大小; j++)
                {
                    int 值 = 0;
                    switch (方向)
                    {
                        case 0: 值 = 网格[j, i]; break; // 上
                        case 1: 值 = 网格[网格大小 - 1 - j, i]; break; // 下
                        case 2: 值 = 网格[i, j]; break; // 左
                        case 3: 值 = 网格[i, 网格大小 - 1 - j]; break; // 右
                    }
                    if (值 != 0) 非零元素.Add(值);
                }
                
                // 合并相同数字
                for (int k = 0; k < 非零元素.Count - 1; k++)
                {
                    if (非零元素[k] == 非零元素[k + 1])
                    {
                        非零元素[k] *= 2;
                        分数 += 非零元素[k];
                        if (分数 > 最高分) 最高分 = 分数;
                        非零元素.RemoveAt(k + 1);
                    }
                }
                
                // 填充空格
                for (int j = 0; j < 网格大小; j++)
                {
                    int 新值 = j < 非零元素.Count ? 非零元素[j] : 0;
                    
                    switch (方向)
                    {
                        case 0: 网格[j, i] = 新值; break;
                        case 1: 网格[网格大小 - 1 - j, i] = 新值; break;
                        case 2: 网格[i, j] = 新值; break;
                        case 3: 网格[i, 网格大小 - 1 - j] = 新值; break;
                    }
                }
            }
            
            // 检查是否有移动发生
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if (网格[y, x] != 旧网格[y, x])
                    {
                        已移动 = true;
                        break;
                    }
                }
                if (已移动) break;
            }
            
            if (已移动)
            {
                添加新数字();
                检查游戏状态();
            }
            
            return 已移动;
        }
        
        private void 添加新数字()
        {
            // 寻找空位置
            var 空位置 = new List<(int, int)>();
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if (网格[y, x] == 0)
                    {
                        空位置.Add((x, y));
                    }
                }
            }
            
            if (空位置.Count > 0)
            {
                var (x, y) = 空位置[随机.Next(空位置.Count)];
                网格[y, x] = 随机.Next(10) < 9 ? 2 : 4; // 90%概率2，10%概率4
            }
        }
        
        private void 检查游戏状态()
        {
            // 检查胜利条件
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if (网格[y, x] == 2048)
                    {
                        胜利 = true;
                        return;
                    }
                }
            }
            
            // 检查是否有空位
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
            
            // 检查是否还有可合并的相邻格子
            for (int y = 0; y < 网格大小; y++)
            {
                for (int x = 0; x < 网格大小; x++)
                {
                    if ((x < 网格大小 - 1 && 网格[y, x] == 网格[y, x + 1]) ||
                        (y < 网格大小 - 1 && 网格[y, x] == 网格[y + 1, x]))
                    {
                        return;
                    }
                }
            }
            
            游戏结束 = true;
        }
        
        private Vector4 获取数字颜色(int 数字)
        {
            switch (数字)
            {
                case 2:    return new Vector4(0.93f, 0.89f, 0.85f, 1);
                case 4:    return new Vector4(0.93f, 0.88f, 0.78f, 1);
                case 8:    return new Vector4(0.95f, 0.69f, 0.47f, 1);
                case 16:   return new Vector4(0.96f, 0.58f, 0.39f, 1);
                case 32:   return new Vector4(0.96f, 0.49f, 0.37f, 1);
                case 64:   return new Vector4(0.96f, 0.37f, 0.23f, 1);
                case 128:  return new Vector4(0.93f, 0.81f, 0.45f, 1);
                case 256:  return new Vector4(0.93f, 0.80f, 0.38f, 1);
                case 512:  return new Vector4(0.93f, 0.78f, 0.31f, 1);
                case 1024: return new Vector4(0.93f, 0.77f, 0.25f, 1);
                case 2048: return new Vector4(0.20f, 0.80f, 0.80f, 1); // 胜利颜色
                default:   return new Vector4(0.78f, 0.73f, 0.65f, 1);
            }
        }
        
        private void 重置游戏()
        {
            网格 = new int[网格大小, 网格大小];
            if (分数 > 最高分) 最高分 = 分数;
            分数 = 0;
            游戏结束 = false;
            胜利 = false;
            
            // 添加两个初始数字
            添加新数字();
            添加新数字();
        }
    }
}