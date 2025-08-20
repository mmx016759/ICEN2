using System.Numerics;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using icen.白魔.View.QT;
using ImGuiNET;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace icen.白魔.triggers;

public class 白魔时间轴新QT设置: ITriggerAction
{
    public string DisplayName { get; } = "白魔/QT设置(新)";

    public string Remark { get; set; }
    
    public List<TriggerQTSetting> QTList = [];
    
    public bool Draw()
    {
        ImGui.BeginChild("###TriggerWhm", new Vector2(0f, 0f));
        ImGuiHelper.DrawSplitList("QT开关", QTList, DrawHeader, AddCallBack, DrawCallback);
        ImGui.EndChild();
        return true;
    }

    public bool Handle()
    {
        foreach (var qtSetting in QTList)
        {
            qtSetting.action();
        }
        return true;
    }

    private TriggerQTSetting DrawCallback(TriggerQTSetting arg)
    {
        arg.draw();
        return  arg;
    }

    private string DrawHeader(TriggerQTSetting arg)
    {
        var v  = arg.Value ? "开" : "关";
        return $"{v}-{arg.Key}";
    }

    private TriggerQTSetting AddCallBack()
    {
        return new TriggerQTSetting();
    }
}


public class TriggerQTSetting()
{
    public string Key = "爆发药";
    public bool Value = false;
    private int combo;
    private int radioCheck;
    
    private readonly string[] _qtArray = 白魔Qt.GetQtArray();
    //这里改成你自己的QT的列表

    public void draw()
    {
        combo = Array.IndexOf(_qtArray, Key);
        if (combo == -1)
        {
            combo = 0;
        }

        radioCheck = Value ? 1 : 0;
        ImGui.NewLine();
        ImGui.SetCursorPos(new Vector2(0f, 40f));
        ImGui.Combo("Qt开关", ref combo, _qtArray, _qtArray.Length);
        ImGui.RadioButton("开", ref radioCheck, 1);
        ImGui.SameLine();
        ImGui.RadioButton("关", ref radioCheck, 0);
        Key = _qtArray[combo];
        Value = radioCheck == 1;
    }

    public void action()
    {
        //这里改成你自己的设置QT的方法
        白魔Qt.SetQt(Key, Value);
    }
}