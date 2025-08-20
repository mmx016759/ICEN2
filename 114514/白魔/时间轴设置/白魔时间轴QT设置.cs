using System.Numerics;
using AEAssist.CombatRoutine.Trigger;
using ICEN2.白魔.界面.QT;
using ImGuiNET;

namespace ICEN2.白魔.时间轴设置;

public class 白魔时间轴QT设置 : ITriggerAction
{
    private int combo;
    private int radioCheck;

    public string DisplayName { get; } = "白魔/QT设置";

    public string Remark { get; set; }
        
    private string[] _qtArray = 白魔Qt.GetQtArray();
    public string Key = "";
    public bool Value;

    public bool Draw()
    {
        combo = Array.IndexOf(_qtArray, Key);
        if (combo == -1)
        {
            combo = 0;
        }
            
        if (ImGui.BeginTabBar("###TriggerTab"))
        {
            ImGui.BeginChild("###TriggerWhm", new Vector2(0f, 0f));
            ImGui.NewLine();
            ImGui.SetCursorPos(new Vector2(0f, 40f));
            ImGui.Combo("Qt开关", ref combo, _qtArray, _qtArray.Length);
            ImGui.RadioButton("开", ref radioCheck, 1);
            ImGui.SameLine();
            ImGui.RadioButton("关", ref radioCheck, 0);
            ImGui.EndChild();
            ImGui.EndTabBar();
        }
        Key = _qtArray[combo];
        Value = radioCheck == 1;
        return true;
    }

    public bool Handle()
    {
        白魔Qt.SetQt(Key, Value);
        return true;
    }
}