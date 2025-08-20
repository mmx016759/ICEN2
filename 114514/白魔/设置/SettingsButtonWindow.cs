﻿using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using icen.白魔.Utilities.设置; // 添加必要的命名空间引用

namespace icen.白魔.Utilities
{
    public static class SettingsButtonWindow
    {
        private static Vector2 _windowPosition = new Vector2(10, 10);
        private static Vector2 _buttonSize = new Vector2(50, 50);
        private static bool _isDragging = false;
        private static Vector2 _dragOffset = Vector2.Zero;
        private static bool _positionLoaded = false; // 标记位置是否已加载
        
        // 更复杂的齿轮图标数据
        private static readonly List<List<Vector2>> _gearIconParts = CreateComplexGearIcon();

        // 创建更复杂的齿轮图标
        private static List<List<Vector2>> CreateComplexGearIcon()
        {
            var parts = new List<List<Vector2>>();
            Vector2 center = new Vector2(25, 25);
            
            // 1. 外齿轮（12个齿）
            var outerGear = new List<Vector2>();
            int teeth = 12;
            float outerRadius = 20f;
            float innerRadius = 17f;
            
            for (int i = 0; i < teeth; i++)
            {
                float angle = i * (2 * MathF.PI / teeth);
                float nextAngle = (i + 0.5f) * (2 * MathF.PI / teeth);
                
                // 外齿
                outerGear.Add(new Vector2(
                    center.X + outerRadius * MathF.Cos(angle),
                    center.Y + outerRadius * MathF.Sin(angle)
                ));
                
                // 内齿
                outerGear.Add(new Vector2(
                    center.X + innerRadius * MathF.Cos(nextAngle),
                    center.Y + innerRadius * MathF.Sin(nextAngle)
                ));
            }
            outerGear.Add(outerGear[0]); // 闭合图形
            parts.Add(outerGear);
            
            // 2. 内环
            var innerRing = new List<Vector2>();
            float ringRadius = 12f;
            int ringPoints = 24;
            
            for (int i = 0; i <= ringPoints; i++)
            {
                float angle = i * (2 * MathF.PI / ringPoints);
                innerRing.Add(new Vector2(
                    center.X + ringRadius * MathF.Cos(angle),
                    center.Y + ringRadius * MathF.Sin(angle)
                ));
            }
            parts.Add(innerRing);
            
            // 3. 加强筋（连接内外环）
            int spokes = 6;
            for (int i = 0; i < spokes; i++)
            {
                var spoke = new List<Vector2>();
                float angle = i * (2 * MathF.PI / spokes);
                
                // 起点在内环上，终点在外环内齿上
                spoke.Add(new Vector2(
                    center.X + (ringRadius - 1) * MathF.Cos(angle),
                    center.Y + (ringRadius - 1) * MathF.Sin(angle)
                ));
                
                spoke.Add(new Vector2(
                    center.X + (innerRadius + 1) * MathF.Cos(angle),
                    center.Y + (innerRadius + 1) * MathF.Sin(angle)
                ));
                
                parts.Add(spoke);
            }
            
            // 4. 中心六边形孔
            var hexagon = new List<Vector2>();
            float hexRadius = 5f;
            int sides = 6;
            
            for (int i = 0; i <= sides; i++)
            {
                float angle = i * (2 * MathF.PI / sides) + MathF.PI/6;
                hexagon.Add(new Vector2(
                    center.X + hexRadius * MathF.Cos(angle),
                    center.Y + hexRadius * MathF.Sin(angle)
                ));
            }
            parts.Add(hexagon);
            
            // 5. 中心十字（增加细节）
            var cross = new List<Vector2>();
            float crossSize = 2.5f;
            cross.Add(new Vector2(center.X - crossSize, center.Y));
            cross.Add(new Vector2(center.X + crossSize, center.Y));
            cross.Add(new Vector2(center.X, center.Y - crossSize));
            cross.Add(new Vector2(center.X, center.Y + crossSize));
            parts.Add(cross);
            
            return parts;
        }

        public static void Draw()
        {
            // 1. 加载保存的位置（如果存在）
            if (!_positionLoaded && 默认值.实例 != null)
            {
                _windowPosition = new Vector2(
                    默认值.实例.SettingsButtonWindowX,
                    默认值.实例.SettingsButtonWindowY
                );
                _positionLoaded = true;
            }

            // 窗口设置
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoDecoration | 
                                    ImGuiWindowFlags.NoBackground |
                                    ImGuiWindowFlags.NoSavedSettings |
                                    ImGuiWindowFlags.NoFocusOnAppearing |
                                    ImGuiWindowFlags.AlwaysAutoResize;
            
            ImGui.SetNextWindowPos(_windowPosition, ImGuiCond.FirstUseEver);
            
            if (ImGui.Begin("白魔设置按钮##Hidden", flags))
            {
                // 拖动处理
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered())
                {
                    _isDragging = true;
                    _dragOffset = ImGui.GetMousePos() - ImGui.GetWindowPos();
                }
                
                if (_isDragging && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    _windowPosition = ImGui.GetMousePos() - _dragOffset;
                    ImGui.SetWindowPos(_windowPosition);
                }
                else
                {
                    // 2. 拖动结束后保存位置
                    if (_isDragging && 默认值.实例 != null)
                    {
                        默认值.实例.SettingsButtonWindowX = _windowPosition.X;
                        默认值.实例.SettingsButtonWindowY = _windowPosition.Y;
                        默认值.实例.Save(); // 保存到文件
                    }
                    _isDragging = false;
                }
                
                // 绘制按钮
                Vector2 buttonPos = ImGui.GetCursorScreenPos();
                bool buttonClicked = ImGui.InvisibleButton("##SettingsButton", _buttonSize);
                
                // 绘制按钮背景（使用默认颜色）
                uint bgColor = ImGui.GetColorU32(ImGuiCol.Button);
                if (ImGui.IsItemHovered())
                {
                    bgColor = ImGui.GetColorU32(ImGuiCol.ButtonHovered);
                }
                if (ImGui.IsItemActive())
                {
                    bgColor = ImGui.GetColorU32(ImGuiCol.ButtonActive);
                }
                
                ImGui.GetWindowDrawList().AddRectFilled(
                    buttonPos, 
                    buttonPos + _buttonSize, 
                    bgColor,
                    ImGui.GetStyle().FrameRounding
                );
                
                // 绘制更复杂的齿轮图标（使用默认字体颜色）
                Vector2 center = buttonPos + _buttonSize / 2;
                Vector2 offset = new Vector2(25, 25);
                
                foreach (var part in _gearIconParts)
                {
                    // 不同类型的部件使用不同的线宽
                    float lineWidth = part.Count <= 4 ? 1.5f : 2.0f; // 十字线稍细
                    
                    if (part.Count > 2) // 多边形或曲线
                    {
                        for (int i = 0; i < part.Count - 1; i++)
                        {
                            ImGui.GetWindowDrawList().AddLine(
                                center + part[i] - offset,
                                center + part[i + 1] - offset,
                                ImGui.GetColorU32(ImGuiCol.Text),
                                lineWidth
                            );
                        }
                    }
                    else if (part.Count == 2) // 直线
                    {
                        ImGui.GetWindowDrawList().AddLine(
                            center + part[0] - offset,
                            center + part[1] - offset,
                            ImGui.GetColorU32(ImGuiCol.Text),
                            lineWidth
                        );
                    }
                }
                
                if (buttonClicked && !_isDragging)
                {
                    设置界面.ShowWindow = !设置界面.ShowWindow;
                }
                
                // 悬停提示
                if (ImGui.IsItemHovered())
                {
                    string tooltip = (设置界面.ShowWindow ? "关闭白魔设置" : "打开白魔设置") 
                                  + "\n拖动: 按住按钮移动鼠标";
                    ImGui.SetTooltip(tooltip);
                }
                
                // 保存窗口位置
                _windowPosition = ImGui.GetWindowPos();
                
                ImGui.End();
            }
        }
    }
}