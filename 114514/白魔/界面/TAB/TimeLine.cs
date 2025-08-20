using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.GUI;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using icen.utils.JobView;
using ImGuiNET;

namespace icen.白魔.View.TAB;

public static class TimeLine
{
    
    public static void DrawTimeLine(JobViewWindow jobViewWindow)
    {
        Draw();
    }

    private static void Draw()
    {
        var TerritoryTypeID = Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();
            ImGui.Text("当前区域ID(TerritoryTypeID): " + TerritoryTypeID);
            ImGui.Text("当前天气ID" + " : " + WeatherHelper.GetWeatherId());
            ImGui.Text("当前时间轴");
            ImGui.Indent();
            {
                var currTriggerLine = AI.Instance.TriggerlineData.CurrTriggerLine;
                var notice = "无";
                if (currTriggerLine != null)
                {
                    notice = $"[{currTriggerLine.Author}]{currTriggerLine.Name}";
                }
                ImGui.Text(notice);
                if (currTriggerLine != null)
                {
                    ImGui.Text("导出变量:");
                    ImGui.Indent();
                    foreach (var v in currTriggerLine.ExposedVars)
                    {
                        var oldValue = AI.Instance.ExposeVarsGetValueOrDefault(v);
                        ImGuiHelper.LeftInputInt(v, ref oldValue);
                        AI.Instance.ExposeVarsSet(v,oldValue);
                    }
                    ImGui.Unindent();
                    ImGui.Text("补充说明:");
                    using (new IndentGroupWrapper())
                    {
                        ImGui.TextWrapped(""+currTriggerLine.ExposedVarDesc);
                    }

                    ImGui.Text("Debug信息:");
                    ImGui.Indent();
                    foreach (var v in AI.Instance.TriggerlineData.ActiveActionBase2TCS)
                    {
                        ImGui.Text($"等待节点: Id={v.Key.Id} Name={v.Key.Remark}");
                    }
                    ImGui.Unindent();
                }
            }
            ImGui.Unindent();
            
            ImGui.Text("当前职能:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(150);
            if (ImGui.BeginCombo("###当前职能", $"{AI.Instance.PartyRole}"))
            {
                foreach (var item in AI.Instance.PartyRoleList.Where(ImGui.Selectable))
                {
                    AI.Instance.PartyRole = item;
                }

                ImGui.EndCombo();
            }
            
            if (ImGui.Button("卸载时间轴"))
            {
                AI.Instance.TriggerlineData.Clear();
            }
            ImGui.Text($"当前场景可用时间轴 {TerritoryTypeID}:");
            ImGui.Indent();
            {
                if (!string.IsNullOrEmpty(TriggerLineHelper.LastChoosed))
                {
                    ImGui.Text("指定时间轴(不自动切换) : " + TriggerLineHelper.LastChoosed);
                    ImGui.SameLine();
                    if (ImGui.Button("清除"))
                    {
                        TriggerLineHelper.LastChoosed = string.Empty;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("清除并自动切换一次"))
                    {
                        TriggerLineHelper.LastChoosed = string.Empty;
                        TriggerLineHelper.LoadDefaultTriggerline().Wait();
                    }
                }
                if (TriggerLineHelper.Terr2Triggerlines.TryGetValue(TerritoryTypeID, out var list))
                {
                    foreach (var triggerLine in list)
                    {
                        using (new GroupWrapper())
                        {
                            ImGui.Text($"[{triggerLine.Author}]{triggerLine.Name}");
                            ImGui.SameLine();
                            if (ImGui.Button("指定加载"))
                            {
                                TriggerLineHelper.LastChoosed = triggerLine.Name;
                                TriggerLineHelper.ForceLoadTriggerline(triggerLine.Name);
                            }
                        }
                    }
                }
            }
            ImGui.Unindent();
    }
}
