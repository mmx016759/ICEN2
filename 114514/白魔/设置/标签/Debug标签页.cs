using ICEN2.common;
using ImGuiNET;

namespace ICEN2.白魔.设置.标签
{
    public static class Debug标签页
    {
        private static int _selectedTab = 0;
        
        public static void Draw()
        {
            ImGui.Text("调试信息");
            ImGui.Separator();
            
            // 创建标签栏
            if (ImGui.BeginTabBar("DebugTabs"))
            {
                // 玩家信息标签页
                if (ImGui.BeginTabItem("玩家状态"))
                {
                    DrawPlayerInfo();
                    ImGui.EndTabItem();
                }
                
                // 目标信息标签页
                if (ImGui.BeginTabItem("目标信息"))
                {
                    DrawTargetInfo();
                    ImGui.EndTabItem();
                }
                
                // 战斗信息标签页
                if (ImGui.BeginTabItem("战斗信息"))
                {
                    DrawCombatInfo();
                    ImGui.EndTabItem();
                }
                
                // 移动信息标签页
                if (ImGui.BeginTabItem("移动信息"))
                {
                    DrawMovementInfo();
                    ImGui.EndTabItem();
                }
                
                // 小队信息标签页
                if (ImGui.BeginTabItem("小队信息"))
                {
                    DrawPartyInfo();
                    ImGui.EndTabItem();
                }
                
                ImGui.EndTabBar();
            }
        }
        
        // 玩家信息标签页内容
        private static void DrawPlayerInfo()
        {
            var player = ApiHelper.当前玩家;
            if (player == null)
            {
                ImGui.Text("玩家信息不可用");
                return;
            }
            
            ImGui.Text($"玩家: {player.Name}");
            ImGui.Text($"位置: X={player.Position.X:F2}, Y={player.Position.Y:F2}, Z={player.Position.Z:F2}");
            ImGui.Text($"朝向: {player.Rotation:F2} 弧度");
            ImGui.Text($"血量: {player.CurrentHp}/{player.MaxHp} ({player.血量百分比():P0})");
            ImGui.Text($"蓝量: {player.CurrentMp}/{player.MaxMp} ({player.蓝量百分比():P0})");
            ImGui.Text($"战斗中: {player.战斗中()}");
            ImGui.Text($"动画锁定: {ApiHelper.动画锁定中}");
            
            // 显示玩家状态图标
            ImGui.Separator();
        }
        
        // 目标信息标签页内容
        private static void DrawTargetInfo()
        {
            var target = Helper.自身目标;
            if (target == null)
            {
                ImGui.Text("无目标");
                return;
            }
            
            ImGui.Text($"目标: {target.Name}");
            ImGui.Text($"ID: {target.TargetObjectId}");
            ImGui.Text($"类型: {target.ObjectKind}");
            ImGui.Text($"位置: X={target.Position.X:F2}, Y={target.Position.Y:F2}, Z={target.Position.Z:F2}");
            ImGui.Text($"距离: {target.距玩家距离():F2} 米");
            ImGui.Text($"血量: {target.CurrentHp}/{target.MaxHp} ({target.血量百分比():P0})");
            ImGui.Text($"可攻击: {ApiHelper.可攻击(target)}");
            ImGui.Text($"在仇恨列表: {target.在仇恨列表()}");
            ImGui.Text($"是Boss: {target.是Boss()}");
            ImGui.Text($"已死亡: {target.已死亡()}");
            // 显示目标的状态效果
            ImGui.Separator();

        }
        
        // 战斗信息标签页内容
        private static void DrawCombatInfo()
        {
            ImGui.Text($"GCD状态: {ApiHelper.可用GCD}");
            ImGui.Text($"GCD剩余冷却: {ApiHelper.GCD剩余冷却}ms");
            ImGui.Text($"GCD总时间: {ApiHelper.GCD总时间}ms");
            ImGui.Text($"能力技可用: {ApiHelper.可用能力技}");
            ImGui.Text($"上一个GCD技能: {ApiHelper.上一个GCD技能}");
            ImGui.Text($"上一个能力技能: {ApiHelper.上一个能力技能}");
            ImGui.Text($"上一个连击技能: {ApiHelper.上一个连击技能}");
            ImGui.Text($"技能队列: {ApiHelper.有技能队列} (ID: {ApiHelper.队列中的技能ID})");
            
            ImGui.Separator();
            ImGui.Text($"当前地图: {ApiHelper.当前地图ID}");
            ImGui.Text($"副本状态: {ApiHelper.副本名称}");
            ImGui.Text($"副本中: {ApiHelper.副本中}");
            ImGui.Text($"Boss战中: {ApiHelper.在Boss战中}");
            ImGui.Text($"副本已开始: {ApiHelper.副本已开始}");
            ImGui.Text($"副本结束: {ApiHelper.副本结束}");
        }
        
        // 移动信息标签页内容
        private static void DrawMovementInfo()
        {
            ImGui.Text($"移动状态: {ApiHelper.移动中()}");
            ImGui.Text($"路径启用: {ApiHelper.路径启用}");
            ImGui.Text($"动画锁定时间: {ApiHelper.动画锁定时间:F2}s");
            
            
            ImGui.Separator();
        }
        
        // 小队信息标签页内容
        private static void DrawPartyInfo()
        {
            var partyMembers = ApiHelper.获取小队成员();
            if (partyMembers == null || partyMembers.Count == 0)
            {
                ImGui.Text("无小队成员");
                return;
            }
            
            ImGui.Text($"小队人数: {partyMembers.Count}");
            
            foreach (var member in partyMembers)
            {
                if (member == null) continue;
                
                ImGui.Separator();
                ImGui.Text($"{member.Name} ");
                ImGui.Text($"位置: X={member.Position.X:F2}, Y={member.Position.Y:F2}");
                ImGui.Text($"距离: {member.距玩家距离():F2}米 血量: {member.血量百分比():P0}");
                ImGui.Text($"状态: {(member.已死亡() ? "死亡" : "存活")}");
            }
        }
    }
}