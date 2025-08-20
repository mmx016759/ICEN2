using System.Numerics;
using ImGuiNET;

namespace icen.白魔.Utilities.标签.小游戏
{
    public class 华容道游戏 : I游戏接口
    {
        private const int 格子大小 = 50;
        private int[,] 地图;
        private int 玩家X, 玩家Y;
        private bool 胜利 = false;
        
        // 0: 空地, 1: 墙, 2: 方块, 3: 目标点, 4: 玩家
        private int[,] 初始地图 = 
        {
            {1,1,1,1,1,1},
            {1,2,2,2,2,1},
            {1,2,0,0,2,1},
            {1,2,0,0,2,1},
            {1,2,2,2,2,1},
            {1,0,0,0,0,1},
            {1,0,4,0,0,1},
            {1,1,1,1,1,1}
        };

        public 华容道游戏()
        {
            重置游戏();
        }

        public void 绘制()
        {
            ImGui.Text("华容道");
            ImGui.Separator();
            
            if (胜利)
            {
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "恭喜过关!");
            }
            
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            
            int 高度 = 地图.GetLength(0);
            int 宽度 = 地图.GetLength(1);
            var 游戏区域大小 = new Vector2(宽度 * 格子大小, 高度 * 格子大小);
            
            // 绘制游戏区域
            for (int y = 0; y < 高度; y++)
            {
                for (int x = 0; x < 宽度; x++)
                {
                    var 格子位置 = 窗口位置 + new Vector2(x * 格子大小, y * 格子大小);
                    var 格子类型 = 地图[y, x];
                    
                    switch (格子类型)
                    {
                        case 0: // 空地
                            绘制列表.AddRectFilled(格子位置, 格子位置 + new Vector2(格子大小, 格子大小), 
                                ImGui.GetColorU32(new Vector4(0.9f, 0.9f, 0.9f, 1)));
                            break;
                        case 1: // 墙
                            绘制列表.AddRectFilled(格子位置, 格子位置 + new Vector2(格子大小, 格子大小), 
                                ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1)));
                            break;
                        case 2: // 方块
                            绘制列表.AddRectFilled(格子位置 + new Vector2(2, 2), 
                                格子位置 + new Vector2(格子大小 - 2, 格子大小 - 2), 
                                ImGui.GetColorU32(new Vector4(0.8f, 0.5f, 0.2f, 1)));
                            break;
                        case 3: // 目标点
                            绘制列表.AddCircle(格子位置 + new Vector2(格子大小 / 2, 格子大小 / 2), 
                                格子大小 / 4, 
                                ImGui.GetColorU32(new Vector4(1, 0, 0, 1)));
                            break;
                        case 4: // 玩家
                            绘制列表.AddRectFilled(格子位置 + new Vector2(5, 5), 
                                格子位置 + new Vector2(格子大小 - 5, 格子大小 - 5), 
                                ImGui.GetColorU32(new Vector4(0, 0, 1, 1)));
                            break;
                    }
                    
                    // 绘制网格线
                    绘制列表.AddRect(格子位置, 格子位置 + new Vector2(格子大小, 格子大小), 
                        ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 1)));
                }
            }
            
            // 设置光标位置
            ImGui.SetCursorScreenPos(窗口位置 + new Vector2(0, 游戏区域大小.Y + 10));
            
            // 方向控制按钮
            ImGui.Text("控制:");
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.4f, 0.8f, 1));
            if (ImGui.Button("↑", new Vector2(50, 50))) 移动(0, -1);
            ImGui.SameLine();
            if (ImGui.Button("←", new Vector2(50, 50))) 移动(-1, 0);
            ImGui.SameLine();
            if (ImGui.Button("→", new Vector2(50, 50))) 移动(1, 0);
            ImGui.SameLine();
            if (ImGui.Button("↓", new Vector2(50, 50))) 移动(0, 1);
            ImGui.PopStyleColor();
            
            // 游戏控制按钮
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.8f, 0.4f, 1));
            if (ImGui.Button("重置游戏", new Vector2(120, 40)))
            {
                重置游戏();
            }
            ImGui.PopStyleColor();
        }
        
        private void 移动(int dx, int dy)
        {
            if (胜利) return;
            
            int 新玩家X = 玩家X + dx;
            int 新玩家Y = 玩家Y + dy;
            
            // 检查边界
            if (新玩家X < 0 || 新玩家Y < 0 || 
                新玩家Y >= 地图.GetLength(0) || 
                新玩家X >= 地图.GetLength(1))
                return;
                
            int 目标格子 = 地图[新玩家Y, 新玩家X];
            
            // 如果是空地
            if (目标格子 == 0)
            {
                // 移动玩家
                地图[玩家Y, 玩家X] = 0;
                地图[新玩家Y, 新玩家X] = 4;
                玩家X = 新玩家X;
                玩家Y = 新玩家Y;
            }
            // 如果是方块
            else if (目标格子 == 2)
            {
                int 方块后X = 新玩家X + dx;
                int 方块后Y = 新玩家Y + dy;
                
                // 检查方块后的位置
                if (方块后X < 0 || 方块后Y < 0 || 
                    方块后Y >= 地图.GetLength(0) || 
                    方块后X >= 地图.GetLength(1))
                    return;
                    
                int 方块后格子 = 地图[方块后Y, 方块后X];
                
                // 方块后必须是空地
                if (方块后格子 == 0)
                {
                    // 移动方块
                    地图[方块后Y, 方块后X] = 2;
                    
                    // 移动玩家
                    地图[玩家Y, 玩家X] = 0;
                    地图[新玩家Y, 新玩家X] = 4;
                    玩家X = 新玩家X;
                    玩家Y = 新玩家Y;
                    
                    // 检查胜利条件
                    检查胜利();
                }
            }
        }
        
        private void 检查胜利()
        {
            // 检查玩家是否到达底部
            if (玩家Y == 地图.GetLength(0) - 2)
            {
                胜利 = true;
            }
        }
        
        private void 重置游戏()
        {
            地图 = (int[,])初始地图.Clone();
            胜利 = false;
            
            // 寻找玩家位置
            for (int y = 0; y < 地图.GetLength(0); y++)
            {
                for (int x = 0; x < 地图.GetLength(1); x++)
                {
                    if (地图[y, x] == 4)
                    {
                        玩家X = x;
                        玩家Y = y;
                    }
                }
            }
        }
    }
}