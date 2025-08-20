using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Common.Math;
using ICEN2.utils.JobView;
using ImGuiNET;

namespace ICEN2.白魔.设置.设置;

public class WhiteMageSettingsUI
{
    
    public void DrawGeneral(JobViewWindow jobViewWindow)
    {

        Draw();
    }
    
    
    
    public static WhiteMageSettingsUI 实例 = new();
    private string _newMacro = ""; // 用于暂存新输入的宏
    private static Random _random = new(); // 用于随机选择宏
    private string _selectedChannel = "/p "; // 默认选中小队频道
    public WhiteMageSettingsUI()
    {
        // 初始化时添加默认宏
        if (默认值.实例.复活提醒宏列表.Count == 0)
        {
            默认值.实例.复活提醒宏列表.Add("已复活<t>。");
        }
    }
    
    ///加按钮状态和计时器
    private enum ButtonState { 日常模式, 仪表盘模式 }
    private ButtonState _currentState = ButtonState.日常模式;
    private float _rainbowTimer = 0.0f;
    
    // 彩虹颜色生成函数

    public void Draw()
    {
        
        // 更新彩虹计时器

        string buttonText = _currentState switch {
            ButtonState.日常模式 => "日常模式",
            _ => "仪表盘模式"
        };

        // 只设置字体颜色

        if (ImGui.Button(buttonText, new Vector2(120, 0)))
        {
            // 循环切换状态
            _currentState = (ButtonState)(((int)_currentState + 1) % 2);

            // 根据状态应用不同设置
            switch (_currentState)
            {
                case ButtonState.日常模式:
                    应用日常设置();
                    break;
               /* case ButtonState.高难模式:
                    应用高难设置();
                    break;
                    */
                case ButtonState.仪表盘模式:
                    应用仪表盘设置();
                    break;
            }
        }
        ImGui.SameLine();
        ImGui.Text("当前显示模式");
        ImGui.Separator();
        // 新增三个按钮，靠右对齐
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(10, 0));
        float buttonWidth = 120f;

        if (ImGui.Button("默认设置", new System.Numerics.Vector2(buttonWidth, 0)))
        {
            应用默认设置();
        }

        ImGui.SameLine();
        if (ImGui.Button("本分奶设置", new System.Numerics.Vector2(buttonWidth, 0)))
        {
            应用本分奶设置();
        }

        ImGui.SameLine();
        if (ImGui.Button("输出奶设置", new System.Numerics.Vector2(buttonWidth, 0)))
        {
            应用输出奶设置();
        }

        ImGui.PopStyleVar();
        ImGui.Separator();
        // 复活设置
        ImGui.TextWrapped("复活延迟设置:");
        ImGui.SliderInt("复活延迟", ref 默认值.实例.复活延迟, 0, 5);
        ImGui.TextWrapped("复活延迟说明: 复活延迟是指在施放复活技能后，等待的时间（秒）再进行下一步操作。");
// 群奶设置

        ImGui.BeginGroup();
        if (ImGui.TreeNode("群奶阈值设置:"))
        {
            ImGui.SliderInt("团血检测人数", ref 默认值.实例.团血检测人数, 2, 5);
            ImGui.SliderInt("狂喜之心血量", ref 默认值.实例.狂喜之心血量, 1, 100);
            ImGui.SliderInt("医济血量", ref 默认值.实例.医济血量, 1, 100);
            ImGui.SliderInt("愈疗/医治血量", ref 默认值.实例.愈疗医治血量, 1, 100);
            ImGui.SliderInt("插入全大血量", ref 默认值.实例.插入全大血量, 1, 100);

        }
        ImGui.EndGroup();
// 单奶设置
        ImGui.BeginGroup();
        if (ImGui.TreeNode("单奶阈值设置:"))
        {
            ImGui.SliderInt("安慰之心血量", ref 默认值.实例.安慰之心血量, 1, 100);
            ImGui.SliderInt("救疗/治疗血量", ref 默认值.实例.救疗治疗血量, 1, 100);
            ImGui.SliderInt("低级本救疗/治疗血量", ref 默认值.实例.低级本救疗治疗血量, 1, 100);

            ImGui.SliderInt("天赐血量", ref 默认值.实例.天赐血量, 1, 100);
            ImGui.SliderInt("神名血量", ref 默认值.实例.神名血量, 1, 100);
            ImGui.SliderInt("神祝祷血量", ref 默认值.实例.神祝祷血量, 1, 100);
            ImGui.SliderInt("保留蓝花数量", ref 默认值.实例.保留蓝花数量, 0, 3);
        }
        ImGui.EndGroup();
        ImGui.Spacing();

        ImGui.Separator();
        /*
        ImGui.Checkbox("复活Hotkey", ref 默认值.实例.SgePartnerPanelShow);
        if (默认值.实例.SgePartnerPanelShow)
        {
            ImGui.SameLine();
            ImGui.Checkbox("营救Hotkey", ref 默认值.实例.SgePartner营救);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(80f); // 设置输入框宽度
            ImGui.InputInt("图标大小", ref 默认值.实例.SgePartnerPanelIconSize);
            
            // 限制大小范围
            if (默认值.实例.SgePartnerPanelIconSize < 10)
                默认值.实例.SgePartnerPanelIconSize = 10;
            if (默认值.实例.SgePartnerPanelIconSize > 100)
                默认值.实例.SgePartnerPanelIconSize = 100;
        }
*/
        ImGui.Checkbox("流血自动铃铛", ref 默认值.实例.流血自动铃铛);
        ImGui.Checkbox("庇护所和铃铛是否已自身为目标", ref 默认值.实例.庇护所目标);
        ImGui.SameLine();
        ImGui.Checkbox("蓝花防溢出", ref 默认值.实例.蓝花防溢出);
        ImGui.Checkbox("优先闪飒", ref 默认值.实例.优先闪飒);
        ImGui.SameLine();
        ImGui.Checkbox("非战斗再生", ref 默认值.实例.非战斗再生);
        ImGui.Separator();
        ImGui.Checkbox("复活是否优先TN", ref 默认值.实例.优先TN);
        ImGui.Checkbox("是否使用复活宏", ref 默认值.实例.复活提醒);
        

            // ====== 修改后的复活提醒宏折叠区域 ======
            if (ImGui.TreeNode("复活宏支持多行输入"))    
            {
                // 复活宏说明
                ImGui.TextWrapped("支持多行输入，宏内容可以包含换行符。");
                ImGui.TextWrapped("宏内容中可以使用<t>来替换为目标名称。");
                ImGui.TextWrapped("频道选择按钮用于选择发送宏的频道。");
                ImGui.TextWrapped("删除按钮会删除对应的宏。");
                ImGui.TextWrapped("添加宏时请确保输入框不为空。");

                // 显示当前频道
                ImGui.Text($"当前频道: {_selectedChannel}");
                ImGui.Separator();
                // 频道选择按钮
                ImGui.Text("频道:");
                ImGui.SameLine();

                // 小队频道按钮
                if (ImGui.RadioButton("小队", _selectedChannel == "/p "))
                    _selectedChannel = "/p ";

                ImGui.SameLine();

                // 团队频道按钮
                if (ImGui.RadioButton("团队", _selectedChannel == "/团队频道 "))
                    _selectedChannel = "/团队频道 ";

                ImGui.SameLine();

                // 默语(表情)频道按钮
                if (ImGui.RadioButton("默语", _selectedChannel == "/e "))
                    _selectedChannel = "/e ";

                // 显示现有宏列表和删除按钮
                for (int i = 0; i < 默认值.实例.复活提醒宏列表.Count; i++)
                {
                    // 直接显示纯文本宏内容（显示前50个字符）
                    string displayText = 默认值.实例.复活提醒宏列表[i];
                    if (displayText.Length > 50)
                        displayText = displayText.Substring(0, 47) + "...";

                    ImGui.Text($"{i + 1}. {displayText.Replace("\n", "\\n")}");
                    ImGui.SameLine();
                    if (ImGui.Button($"删除##{i}"))
                    {
                        默认值.实例.复活提醒宏列表.RemoveAt(i);
                        // 删除后跳出循环防止索引错误
                        break;
                    }
                }

                // 添加新宏的输入框和按钮（改为多行输入）
                ImGui.InputTextMultiline("##NewMacro", ref _newMacro, 200,
                    new System.Numerics.Vector2(0, ImGui.GetTextLineHeight() * 3));

                ImGui.SameLine();
                if (ImGui.Button("添加宏") && !string.IsNullOrWhiteSpace(_newMacro))
                {
                    // 保存纯文本宏内容（包含换行符）
                    默认值.实例.复活提醒宏列表.Add(_newMacro);
                    _newMacro = ""; // 清空输入框
                }

                ImGui.TreePop(); // 结束折叠区域
        }
// 保存按钮
        ImGui.SliderInt("醒梦阈值", ref 默认值.实例.醒梦, 1, 100);
        if (ImGui.Button("保存设置"))
            默认值.实例.Save();
    }

    // 修改后的获取随机宏函数（返回字符串数组）
    public bool TryGetRandomReviveMacro(SeString targetName, out string[] result)
    {
        result = null;
        
        // 没有宏时返回false
        if (默认值.实例.复活提醒宏列表 == null || 默认值.实例.复活提醒宏列表.Count == 0)
            return false;
        
        // 随机选择一条宏
        int index = _random.Next(默认值.实例.复活提醒宏列表.Count);
        string selectedMacro = 默认值.实例.复活提醒宏列表[index];
        
        // 替换<t>为目标名称
        if (!string.IsNullOrEmpty(targetName.ToString()))
            selectedMacro = selectedMacro.Replace("<t>", targetName.ToString());
        // 分割成多行并添加频道前缀
        result = selectedMacro.Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => _selectedChannel + line.Trim())
            .ToArray();
        
        return result.Length > 0;
    }
    
    
    private void 应用默认设置()
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

    private void 应用本分奶设置()
    {
        // 更高治疗阈值，专注治疗
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

    private void 应用输出奶设置()
    {
        // 更低治疗阈值，倾向输出
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
    
    private void 应用日常设置()
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
    private void 应用高难设置()
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
        默认值.实例.JobViewSave.QtUnVisibleList.Add("神速咏唱");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("康复");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("法令");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("AOE");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("Dot");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("单奶");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("群奶");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("读条奶");
        默认值.实例.JobViewSave.QtUnVisibleList.Add("减伤");
            
    }
    private void 应用仪表盘设置()
    {
        默认值.实例.JobViewSave.QtUnVisibleList.Clear();
        默认值.实例.JobViewSave.QtUnVisibleList.Add("高难模式");
    }
}