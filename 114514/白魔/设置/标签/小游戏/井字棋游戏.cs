using System.Numerics;
using ImGuiNET;

namespace ICEN2.白魔.设置.标签.小游戏
{
    public class 井字棋游戏 : I游戏接口
    {
        private const int 格子大小 = 60;
        private const int 棋盘大小 = 3;
        private int[,] 棋盘 = new int[棋盘大小, 棋盘大小];
        private int 当前玩家 = 1; // 1: X, 2: O
        private bool 游戏结束 = false;
        private string 游戏状态 = "";
        private int 获胜者 = 0;

        public void 绘制()
        {
            ImGui.Text("井字棋");
            ImGui.Separator();
            
            if (游戏结束)
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), 游戏状态);
            }
            else
            {
                ImGui.Text($"当前玩家: {(当前玩家 == 1 ? "X" : "O")}");
            }

            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 棋盘区域大小 = new Vector2(棋盘大小 * 格子大小, 棋盘大小 * 格子大小);
            
            // 绘制棋盘背景
            uint 背景色 = ImGui.GetColorU32(new Vector4(0.9f, 0.9f, 0.9f, 1));
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 棋盘区域大小, 背景色);
            
            // 绘制网格线
            uint 网格线颜色 = ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1));
            for (int i = 1; i < 棋盘大小; i++)
            {
                // 垂直线
                var 起点 = 窗口位置 + new Vector2(i * 格子大小, 0);
                var 终点 = 起点 + new Vector2(0, 棋盘区域大小.Y);
                绘制列表.AddLine(起点, 终点, 网格线颜色, 2f);
                
                // 水平线
                起点 = 窗口位置 + new Vector2(0, i * 格子大小);
                终点 = 起点 + new Vector2(棋盘区域大小.X, 0);
                绘制列表.AddLine(起点, 终点, 网格线颜色, 2f);
            }
            
            // 处理点击和绘制棋子
            for (int y = 0; y < 棋盘大小; y++)
            {
                for (int x = 0; x < 棋盘大小; x++)
                {
                    var 格子位置 = 窗口位置 + new Vector2(x * 格子大小, y * 格子大小);
                    var 格子区域 = new Vector2(格子大小, 格子大小);
                    
                    // 检测点击
                    if (!游戏结束 && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        var 鼠标位置 = ImGui.GetMousePos();
                        if (鼠标位置.X >= 格子位置.X && 鼠标位置.X < 格子位置.X + 格子大小 &&
                           鼠标位置.Y >= 格子位置.Y && 鼠标位置.Y < 格子位置.Y + 格子大小)
                        {
                            if (棋盘[x, y] == 0)
                            {
                                棋盘[x, y] = 当前玩家;
                                检查游戏状态();
                                当前玩家 = 当前玩家 == 1 ? 2 : 1;
                            }
                        }
                    }
                    
                    // 绘制棋子
                    if (棋盘[x, y] == 1) // X
                    {
                        var 颜色 = ImGui.GetColorU32(new Vector4(1, 0, 0, 1));
                        绘制列表.AddLine(
                            格子位置 + new Vector2(10, 10),
                            格子位置 + new Vector2(格子大小 - 10, 格子大小 - 10),
                            颜色, 3f);
                        绘制列表.AddLine(
                            格子位置 + new Vector2(格子大小 - 10, 10),
                            格子位置 + new Vector2(10, 格子大小 - 10),
                            颜色, 3f);
                    }
                    else if (棋盘[x, y] == 2) // O
                    {
                        var 颜色 = ImGui.GetColorU32(new Vector4(0, 0, 1, 1));
                        绘制列表.AddCircle(
                            格子位置 + new Vector2(格子大小 / 2, 格子大小 / 2),
                            格子大小 / 3,
                            颜色, 0, 3f);
                    }
                }
            }
            
            // 设置光标位置
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 棋盘区域大小.Y + 10));
            
            // 重新开始按钮
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.8f, 0.4f, 1));
            if (ImGui.Button("重新开始", new Vector2(120, 40)))
            {
                重置游戏();
            }
            ImGui.PopStyleColor();
        }
        
        private void 检查游戏状态()
        {
            // 检查行
            for (int y = 0; y < 棋盘大小; y++)
            {
                if (棋盘[0, y] != 0 && 
                    棋盘[0, y] == 棋盘[1, y] && 
                    棋盘[1, y] == 棋盘[2, y])
                {
                    获胜者 = 棋盘[0, y];
                    游戏结束 = true;
                    游戏状态 = $"玩家 {(获胜者 == 1 ? "X" : "O")} 获胜!";
                    return;
                }
            }
            
            // 检查列
            for (int x = 0; x < 棋盘大小; x++)
            {
                if (棋盘[x, 0] != 0 && 
                    棋盘[x, 0] == 棋盘[x, 1] && 
                    棋盘[x, 1] == 棋盘[x, 2])
                {
                    获胜者 = 棋盘[x, 0];
                    游戏结束 = true;
                    游戏状态 = $"玩家 {(获胜者 == 1 ? "X" : "O")} 获胜!";
                    return;
                }
            }
            
            // 检查对角线
            if (棋盘[0, 0] != 0 && 
                棋盘[0, 0] == 棋盘[1, 1] && 
                棋盘[1, 1] == 棋盘[2, 2])
            {
                获胜者 = 棋盘[0, 0];
                游戏结束 = true;
                游戏状态 = $"玩家 {(获胜者 == 1 ? "X" : "O")} 获胜!";
                return;
            }
            
            if (棋盘[2, 0] != 0 && 
                棋盘[2, 0] == 棋盘[1, 1] && 
                棋盘[1, 1] == 棋盘[0, 2])
            {
                获胜者 = 棋盘[2, 0];
                游戏结束 = true;
                游戏状态 = $"玩家 {(获胜者 == 1 ? "X" : "O")} 获胜!";
                return;
            }
            
            // 检查平局
            bool 棋盘已满 = true;
            for (int y = 0; y < 棋盘大小; y++)
            {
                for (int x = 0; x < 棋盘大小; x++)
                {
                    if (棋盘[x, y] == 0)
                    {
                        棋盘已满 = false;
                        break;
                    }
                }
            }
            
            if (棋盘已满)
            {
                游戏结束 = true;
                游戏状态 = "平局!";
            }
        }
        
        private void 重置游戏()
        {
            for (int y = 0; y < 棋盘大小; y++)
            {
                for (int x = 0; x < 棋盘大小; x++)
                {
                    棋盘[x, y] = 0;
                }
            }
            当前玩家 = 1;
            游戏结束 = false;
            获胜者 = 0;
            游戏状态 = "";
        }
    }
}