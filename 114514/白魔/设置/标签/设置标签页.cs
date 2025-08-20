using AEAssist.Helper;
using Dalamud.Game.Text.SeStringHandling;
using icen.utils;
using icen.utils.JobView;
using icen.白魔.Utilities.设置;
using ImGuiNET;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace icen.白魔.Utilities.标签
{
    public static class 设置标签页
    {
        private static WhiteMageSettingsUI 设置UI实例 = WhiteMageSettingsUI.实例;
        private static int _currentTab = 0; // 当前选中的标签页索引
        public static void DrawGeneral(JobViewWindow jobViewWindow)
        {

            Draw();
        }
        public static void Draw()
        {
            // 标签页选择
            string[] tabs = { "常用设置", "治疗阈值", "宏管理", "高级设置" };
            ImGui.BeginTabBar("##设置标签页");
            for (int i = 0; i < tabs.Length; i++)
            {
                if (ImGui.BeginTabItem(tabs[i]))
                {
                    _currentTab = i;
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
            
            // 顶部按钮区域（始终显示）
            DrawTopButtons();
            ImGui.Separator();
            
            // 根据当前标签页显示内容
            switch (_currentTab)
            {
                case 0: // 常用设置
                    设置UI实例.DrawCommonSettings();
                    break;
                case 1: // 治疗阈值
                    WhiteMageSettingsUI.DrawHealingThresholds();
                    break;
                case 2: // 宏管理
                    设置UI实例.DrawMacroSettings();
                    break;
                case 3: // 高级设置
                    WhiteMageSettingsUI.DrawAdvancedSettings();
                    break;
            }
            
            // 保存按钮（始终显示在底部）
            ImGui.Separator();
            if (ImGui.Button("保存设置", new Vector2(120, 30)))
            {
                默认值.实例.Save();
            }
        }
        
        private static void DrawTopButtons()
        {
            // 模式切换按钮
            if (ImGui.Button(设置UI实例.GetCurrentModeText(), new Vector2(120, 0)))
            {
                设置UI实例.ToggleMode();
            }
            ImGui.SameLine();
            ImGui.Text("当前显示模式");
            
            ImGui.SameLine(ImGui.GetWindowWidth() - 360);
            
            // 预设配置按钮
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(10, 0));
            float buttonWidth = 120f;
            
            if (ImGui.Button("默认设置", new Vector2(buttonWidth, 0)))
            {
                WhiteMageSettingsUI.应用默认设置();
            }
            ImGui.SameLine();
            if (ImGui.Button("本分奶设置", new Vector2(buttonWidth, 0)))
            {
                WhiteMageSettingsUI.应用本分奶设置();
            }
            ImGui.SameLine();
            if (ImGui.Button("输出奶设置", new Vector2(buttonWidth, 0)))
            {
                WhiteMageSettingsUI.应用输出奶设置();
            }
            ImGui.PopStyleVar();
        }
    }

    public class WhiteMageSettingsUI
    {
        public static WhiteMageSettingsUI 实例 = new();
        private string _newMacro = "";
        private static Random _random = new();
        private string _selectedChannel = "/p ";

        public WhiteMageSettingsUI()
        {
            if (默认值.实例.复活提醒宏列表.Count == 0)
            {
                默认值.实例.复活提醒宏列表.Add("已复活<t>。");
            }
        }

        private enum ButtonState { 日常模式, 仪表盘模式 }
        private ButtonState _currentState = ButtonState.日常模式;
        private float _rainbowTimer = 0.0f;

        private static Vector4 RainbowColor(float time, float speed = 1.0f)
        {
            const float frequency = 0.1f;
            float r = MathF.Sin(frequency * time * speed + 0) * 0.5f + 0.5f;
            float g = MathF.Sin(frequency * time * speed + 2) * 0.5f + 0.5f;
            float b = MathF.Sin(frequency * time * speed + 4) * 0.5f + 0.5f;
            return new Vector4(r, g, b, 1.0f);
        }
        
        public Vector4 GetCurrentModeColor()
        {
            return _currentState switch
            {
                ButtonState.日常模式 => new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                _ => RainbowColor(_rainbowTimer, 30)
            };
        }
        
        public string GetCurrentModeText()
        {
            return _currentState switch
            {
                ButtonState.日常模式 => "日常模式",
                _ => "仪表盘模式"
            };
        }
        
        public void ToggleMode()
        {
            _currentState = (ButtonState)(((int)_currentState + 1) % 2);
            switch (_currentState)
            {
                case ButtonState.日常模式:
                    应用日常设置();
                    break;
                case ButtonState.仪表盘模式:
                    应用仪表盘设置();
                    break;
            }
        }

        public void DrawCommonSettings()
        {
            _rainbowTimer += ImGui.GetIO().DeltaTime;
            
            ImGui.Spacing();
             ImGui.Text( "基本功能设置");
            ImGui.Separator();
            
            // 第一行设置
            ImGui.Checkbox("流血自动铃铛", ref 默认值.实例.流血自动铃铛);
            ImGui.SameLine();
            ImGui.Checkbox("庇护所和铃铛以自身为目标", ref 默认值.实例.庇护所目标);
            ImGui.SameLine();
            ImGui.Checkbox("蓝花防溢出", ref 默认值.实例.蓝花防溢出);
            
            // 第二行设置
            ImGui.Checkbox("优先闪飒", ref 默认值.实例.优先闪飒);
            ImGui.SameLine();
            ImGui.Checkbox("非战斗再生", ref 默认值.实例.非战斗再生);
            
            // 第三行设置
            ImGui.Checkbox("复活优先TN", ref 默认值.实例.优先TN);
            ImGui.SameLine();
            ImGui.Checkbox("使用复活宏", ref 默认值.实例.复活提醒);
            
            ImGui.Spacing();
             ImGui.Text( "复活设置");
            ImGui.Separator();
            
            ImGui.TextWrapped("复活延迟: 等待指定时间后才复活（秒）");
            ImGui.SliderInt("##复活延迟", ref 默认值.实例.复活延迟, 0, 5);
            
            ImGui.Spacing();
             ImGui.Text( "资源管理");
            ImGui.Separator();
            
            ImGui.Text("醒梦蓝量阈值:");
            ImGui.SliderInt("##醒梦阈值", ref 默认值.实例.醒梦, 1, 100);
            ImGui.SameLine();
            ImGui.Text("保留蓝花数量:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(80f); // 设置输入框宽度
            ImGui.InputInt("##保留蓝花数量", ref 默认值.实例.保留蓝花数量);
            
            // 限制大小范围
            if (默认值.实例.保留蓝花数量 < 0)
                默认值.实例.保留蓝花数量 = 0;
            if (默认值.实例.保留蓝花数量 > 3)
                默认值.实例.保留蓝花数量 = 3;
        }
        

        public static void DrawHealingThresholds()
        {
            ImGui.Spacing();
             ImGui.Text( "群体治疗阈值");
            ImGui.Separator();
            
            ImGui.SliderInt("团血检测人数", ref 默认值.实例.团血检测人数, 2, 5);
            ImGui.SliderInt("狂喜之心血量", ref 默认值.实例.狂喜之心血量, 1, 100);
            ImGui.SliderInt("医济血量", ref 默认值.实例.医济血量, 1, 100);
            ImGui.SliderInt("愈疗/医治血量", ref 默认值.实例.愈疗医治血量, 1, 100);
            ImGui.SliderInt("插入全大血量", ref 默认值.实例.插入全大血量, 1, 100);
            
            ImGui.Spacing();
             ImGui.Text( "单体治疗阈值");
            ImGui.Separator();
            
            ImGui.SliderInt("安慰之心血量", ref 默认值.实例.安慰之心血量, 1, 100);
            ImGui.SliderInt("救疗/治疗血量", ref 默认值.实例.救疗治疗血量, 1, 100);
            ImGui.SliderInt("低级本救疗/治疗血量", ref 默认值.实例.低级本救疗治疗血量, 1, 100);
            ImGui.SliderInt("天赐血量", ref 默认值.实例.天赐血量, 1, 100);
            ImGui.SliderInt("神名血量", ref 默认值.实例.神名血量, 1, 100);
            ImGui.SliderInt("神祝祷血量", ref 默认值.实例.神祝祷血量, 1, 100);
        }

        public void DrawMacroSettings()
        {
            ImGui.Spacing();
             ImGui.Text( "复活宏管理");
            ImGui.Separator();
            
            ImGui.TextWrapped("支持多行输入，宏内容可以包含换行符。");
            ImGui.TextWrapped("宏内容中可以使用<t>来替换为目标名称。");
            
            ImGui.Spacing();
             ImGui.Text( "频道选择");
            ImGui.Separator();
            
            ImGui.Text($"当前频道: {_selectedChannel}");
            if (ImGui.RadioButton("小队 /p", _selectedChannel == "/p "))
                _selectedChannel = "/p ";
            ImGui.SameLine();
            if (ImGui.RadioButton("团队 /团队频道", _selectedChannel == "/团队频道 "))
                _selectedChannel = "/团队频道 ";
            ImGui.SameLine();
            if (ImGui.RadioButton("默语 /e", _selectedChannel == "/e "))
                _selectedChannel = "/e ";
            
            ImGui.Spacing();
             ImGui.Text( "宏列表");
            ImGui.Separator();
            
            for (int i = 0; i < 默认值.实例.复活提醒宏列表.Count; i++)
            {
                string displayText = 默认值.实例.复活提醒宏列表[i];
                if (displayText.Length > 50)
                    displayText = displayText.Substring(0, 47) + "...";
                    
                ImGui.Text($"{i + 1}. {displayText.Replace("\n", "\\n")}");
                ImGui.SameLine();
                if (ImGui.Button($"删除##{i}"))
                {
                    默认值.实例.复活提醒宏列表.RemoveAt(i);
                    break;
                }
            }
            
            ImGui.Spacing();
             ImGui.Text( "添加新宏");
            ImGui.Separator();
            
            ImGui.InputTextMultiline("##NewMacro", ref _newMacro, 200,
                new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() * 4));
            
            if (ImGui.Button("添加宏", new Vector2(120, 30)) && !string.IsNullOrWhiteSpace(_newMacro))
            {
                默认值.实例.复活提醒宏列表.Add(_newMacro);
                _newMacro = "";
            }

            if (ImGui.Button("测试发送", new Vector2(120, 30)))
            {
                string targetName = "<t>";
                if (TryGetRandomReviveMacro(targetName, out string[] macros))
                {
                    // 逐行发送宏内容
                    foreach (string macroLine in macros)
                    {
                        ChatHelper.SendMessage(macroLine);
                    }
                }
            }
        }

        public static void DrawAdvancedSettings()
        {
            ImGui.Spacing();
             ImGui.Text( "HotKey配置窗口");
            ImGui.Separator();
            ImGui.Checkbox("HotKey配置窗口", ref GlobalSetting.Instance.HotKey配置窗口);
            
            ImGui.Spacing();
             ImGui.Text( "技能优先级");
            ImGui.Separator();
            
            ImGui.Text("治疗技能优先级设置:");
            // 这里可以添加更详细的技能优先级设置
            
            ImGui.Spacing();
             ImGui.Text( "自定义触发器");
            ImGui.Separator();
            
            ImGui.Text("自定义战斗条件:");
            // 这里可以添加自定义触发器设置
            
            ImGui.Spacing();
             ImGui.Text( "调试选项");
            ImGui.Separator();
            

            ImGui.SameLine();

        }

        public bool TryGetRandomReviveMacro(SeString targetName, out string[] result)
        {
            result = null;
            if (默认值.实例.复活提醒宏列表 == null || 默认值.实例.复活提醒宏列表.Count == 0)
                return false;

            int index = _random.Next(默认值.实例.复活提醒宏列表.Count);
            string selectedMacro = 默认值.实例.复活提醒宏列表[index];
            if (!string.IsNullOrEmpty(targetName.ToString()))
                selectedMacro = selectedMacro.Replace("<t>", targetName.ToString());
            result = selectedMacro.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => _selectedChannel + line.Trim())
                .ToArray();
            return result.Length > 0;
        }

        public static void 应用默认设置()
        {
            默认值.实例.狂喜之心血量 = 75;
            默认值.实例.医济血量 = 65;
            默认值.实例.愈疗医治血量 = 45;
            默认值.实例.插入全大血量 = 50;
            默认值.实例.安慰之心血量 = 40;
            默认值.实例.救疗治疗血量 = 40;
            默认值.实例.天赐血量 = 20;
            默认值.实例.神名血量 = 75;
            默认值.实例.神祝祷血量 = 70;
            默认值.实例.低级本救疗治疗血量 = 60;
        }

        public static void 应用本分奶设置()
        {
            默认值.实例.狂喜之心血量 = 85;
            默认值.实例.医济血量 = 73;
            默认值.实例.愈疗医治血量 = 50;
            默认值.实例.插入全大血量 = 50;
            默认值.实例.安慰之心血量 = 60;
            默认值.实例.救疗治疗血量 = 60;
            默认值.实例.天赐血量 = 30;
            默认值.实例.神名血量 = 80;
            默认值.实例.神祝祷血量 = 70;
            默认值.实例.低级本救疗治疗血量 = 70;
        }

        public static void 应用输出奶设置()
        {
            默认值.实例.狂喜之心血量 = 65;
            默认值.实例.医济血量 = 55;
            默认值.实例.愈疗医治血量 = 40;
            默认值.实例.插入全大血量 = 40;
            默认值.实例.安慰之心血量 = 55;
            默认值.实例.救疗治疗血量 = 55;
            默认值.实例.天赐血量 = 10;
            默认值.实例.神名血量 = 65;
            默认值.实例.神祝祷血量 = 60;
            默认值.实例.低级本救疗治疗血量 = 55;
        }

        private static void 应用日常设置()
        {
            默认值.实例.JobViewSave.QtUnVisibleList.Clear();
            默认值.实例.JobViewSave.QtUnVisibleList.Add("高难模式");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("铃铛");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("狂喜之心");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("安慰之心");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("神名");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("醒梦");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("神祝祷");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("全大赦");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("天赐");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("再生");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("水流幕");
            默认值.实例.JobViewSave.QtUnVisibleList.Add("减伤");
        }

        private static void 应用仪表盘设置()
        {
            默认值.实例.JobViewSave.QtUnVisibleList.Clear();
            默认值.实例.JobViewSave.QtUnVisibleList.Add("高难模式");
        }
    }
}