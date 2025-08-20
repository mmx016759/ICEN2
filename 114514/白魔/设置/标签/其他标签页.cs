using ICEN2.白魔.设置.标签.小游戏;
using ImGuiNET;

namespace ICEN2.白魔.设置.标签
{
    public static class 其他标签页
    {
        public static void Draw()
        {
            ImGui.Text("迷你游戏中心");
            ImGui.Separator();
            
            // 游戏选择器
            if (ImGui.Button("贪吃蛇")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.贪吃蛇;
            ImGui.SameLine();
            if (ImGui.Button("俄罗斯方块")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.俄罗斯方块;
            ImGui.SameLine();
            if (ImGui.Button("扫雷")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.扫雷;
            ImGui.SameLine();
            if (ImGui.Button("井字棋")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.井字棋;
                
            if (ImGui.Button("推箱子")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.推箱子;
            ImGui.SameLine();
/*if (ImGui.Button("华容道")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.华容道;
            ImGui.SameLine();
            */
            if (ImGui.Button("2048")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.数字拼图2048;
            ImGui.SameLine();
            if (ImGui.Button("数独")) 
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.数独;
                
            if (ImGui.Button("关闭游戏"))
                游戏管理器.当前游戏 = 游戏管理器.游戏类型.无;
                
            ImGui.Separator();
            
            // 绘制当前选中的游戏
            游戏管理器.绘制当前游戏();
        }
    }
}