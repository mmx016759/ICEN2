﻿﻿﻿using System.Numerics;
using System.IO;
using System.Text.Json;
using icen.白魔.Utilities.标签;
using ImGuiNET;

namespace icen.白魔.Utilities
{
    public class 设置配置
    {
        public int 当前标签 { get; set; } = 0;
    }

    public static class 设置界面
    {
        public static bool ShowWindow = false;
        private static 设置配置 _配置 = new 设置配置();
        private static bool _需要保存 = false;
        private const string 配置文件路径 = "./配置/界面设置.json";
        

        public static void Draw()
        {
            if (!ShowWindow) return;
            
            // 设置窗口初始大小和大小约束
            ImGui.SetNextWindowSize(new Vector2(700, 520), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(
                new Vector2(450, 350),
                new Vector2(float.MaxValue, float.MaxValue)
            );
            
            // 添加窗口背景渐变
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            if (ImGui.Begin("ICEN 设置中心", ref ShowWindow, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.PopStyleVar();
                
                // 主内容区域
                ImGui.BeginChild("main-content", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), false);
                
                // 左侧按钮区域
                ImGui.BeginChild("left-pane", new Vector2(120, 0), true);
                DrawTabButtons();
                ImGui.EndChild();
                
                ImGui.SameLine();
                
                // 垂直分隔线
                DrawVerticalSeparator();
                
                ImGui.SameLine();
                
                // 右侧内容区域
                ImGui.BeginChild("right-pane", new Vector2(0, 0), true);
                DrawContent();
                ImGui.EndChild();
                
                ImGui.EndChild();
                
                // 底部状态栏
                DrawStatusBar();
                
                ImGui.End();
            }
            else
            {
                ImGui.PopStyleVar();
            }
        }
        
        private static void DrawTabButtons()
        {
            // 按钮样式
            Vector2 buttonSize = new Vector2(ImGui.GetContentRegionAvail().X, 40);
    
            // 为三个按钮设置不同的不透明度
            float[] opacities = { 1.0f, 0.8f, 0.6f }; // 不同的不透明度值
    
            // 设置按钮
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, opacities[0]);
            if (ImGui.Button("设 置", buttonSize))
                _配置.当前标签 = 0;
            ImGui.PopStyleVar();
    
            // 其他按钮
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, opacities[1]);
            if (ImGui.Button("其 他", buttonSize))
                _配置.当前标签 = 1;
            ImGui.PopStyleVar();
    
            // Debug按钮
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, opacities[2]);
            if (ImGui.Button("Debug", buttonSize))
                _配置.当前标签 = 2;
            ImGui.PopStyleVar();
        }
        
        private static void DrawVerticalSeparator()
        {
            var drawList = ImGui.GetWindowDrawList();
            Vector2 start = ImGui.GetCursorScreenPos();
            Vector2 end = start + new Vector2(0, ImGui.GetContentRegionAvail().Y);
            
            // 简单的灰色分隔线
            drawList.AddLine(start, end, ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 0.3f)), 1.0f);
            
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 6);
        }
        
        private static void DrawContent()
        {
            // 添加标题
            string[] titles = { "设 置 中 心", "其 他 设 置", "调 试 信 息" };
            ImGuiUtil.CenteredText(titles[_配置.当前标签], new Vector4(0.26f, 0.59f, 0.98f, 1.0f));
            
            // 添加装饰线
            var drawList = ImGui.GetWindowDrawList();
            Vector2 start = ImGui.GetCursorScreenPos();
            Vector2 end = start + new Vector2(ImGui.GetContentRegionAvail().X, 1);
            drawList.AddLine(start, end, ImGui.GetColorU32(new Vector4(0.26f, 0.59f, 0.98f, 0.5f)));
            ImGui.Dummy(new Vector2(0, 10));
            
            // 根据当前选中的标签显示不同内容
            switch (_配置.当前标签)
            {
                case 0:
                    设置标签页.Draw();
                    break;
                case 1:
                    其他标签页.Draw();
                    break;
                case 2:
                    Debug标签页.Draw();
                    break;
            }
        }
        
        private static void DrawStatusBar()
        {
            ImGui.BeginChild("status-bar", new Vector2(0, 24), false);
            
            // 状态栏背景
            
            // 状态文本
            string[] statuses = { "配置系统参数", "管理附加功能", "诊断系统状态" };
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
            
            // 添加保存状态提示
            string statusText = $"状态: {statuses[_配置.当前标签]}";
            
            ImGui.Text(statusText);
            
            ImGui.SameLine(ImGui.GetContentRegionAvail().X - 120);
            ImGui.TextDisabled(DateTime.Now.ToString("HH:mm:ss"));
            
            ImGui.EndChild();
        }
        
        
    }
    
    // 辅助类用于自定义UI元素
    public static class ImGuiUtil
    {
        // 移除了ColoredButton方法
        
        public static void CenteredText(string text, Vector4 color)
        {
            float windowWidth = ImGui.GetWindowSize().X;
            float textWidth = ImGui.CalcTextSize(text).X;
            
            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.TextColored(color, text);
        }
    }
}   