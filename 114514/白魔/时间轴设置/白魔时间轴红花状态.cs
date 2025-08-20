using AEAssist.CombatRoutine.Trigger;
using AEAssist.GUI;
using icen.common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace icen.白魔.triggers;

public class 白魔时间轴红花状态 : ITriggerCond
{
    [LabelName("红花数量")]
    public int Lily { get; set; }

    public string DisplayName => "WHM/检测量谱-红花";

    public string Remark { get; set; }

    public bool Draw()
    {
        return false;
    }

    public bool Handle(ITriggerCondParams triggerCondParams)
    {
        return Helper.白魔量谱_红花数量() >= Lily;
    }
}