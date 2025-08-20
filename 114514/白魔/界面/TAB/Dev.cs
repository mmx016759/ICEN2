using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.MemoryApi;
using ICEN2.common;
using ICEN2.白魔.设置.设置;
using ImGuiNET;
using JobViewWindow = ICEN2.utils.JobView.JobViewWindow;

namespace ICEN2.白魔.界面.TAB;

public static class Dev
{
    public static void DrawDev(JobViewWindow jobViewWindow)
    {
        if (默认值.实例.调试窗口)
        {
            ImGui.Begin("调试窗口");
            Draw();
            ImGui.End();
        }

        Draw();
    }


    public static void Draw()
    {
        
        ImGui.SameLine();
        ImGui.Text($"当前地图id：{Helper.当前地图id}");
        ImGui.Text($"是否在副本中：{Helper.是否在副本中()}");
        ImGui.Text($"是否在战斗中：{Helper.自身.InCombat()}");
        ImGui.Text($"副本是否正式开始：{Core.Resolve<MemApiDuty>().InMission}");
        ImGui.Text($"4人本BOSS战状态：{Core.Resolve<MemApiDuty>().InBossBattle}");
        ImGui.Text($"是否完成副本：{Core.Resolve<MemApiDuty>().IsOver}");
        ImGui.Text($"几人本: {Helper.队伍成员数量}");
        // ImGui.Text($"isChanged:{WhiteMageBattleData.isChange}");

        // var contentDirector = EventFramework.Instance()->GetContentDirector();
        // if (contentDirector != null)
        // {
        //     contentDirector->Objectives.ForEach(d =>
        //     {
        //         ImGui.Text($"{d.Label}---{d.CountCurrent}---{d.CountNeeded}---{d.Enabled}--{d.DisplayType}--{d.TimeLeft}--{d.MapRowId}");
        //     });
        //     
        //     ImGui.Text($"---------");
        //     ImGui.Text($"{contentDirector->Objectives.Last->Label}---{contentDirector->Objectives.Last->CountCurrent}---{contentDirector->Objectives.Last->CountNeeded}---{contentDirector->Objectives.Last->Enabled}");
        // }

        //
        // var d = Core.Resolve<MemApiDuty>().GetSchedule();
        // if (d != null)
        // {
        //     ImGui.Text($"当前进度名称：{d.NowPointName}");
        //     ImGui.Text($"当前进度：{d.NowPoint}");
        //     ImGui.Text($"总共需要进度：{d.CountPoint}");
        // }
        //
        //

        if (ImGui.Button("打断当前读条"))
        {
            Core.Resolve<MemApiSpell>().CancelCast();
        }

        ImGui.Separator();
        if (ImGui.TreeNode("插入技能状态"))
        {
            if (ImGui.Button("清除队列"))
            {
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Clear();
                AI.Instance.BattleData.HighPrioritySlots_GCD.Clear();
            }

            ImGui.SameLine();
            if (ImGui.Button("清除一个"))
            {
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Dequeue();
                AI.Instance.BattleData.HighPrioritySlots_GCD.Dequeue();
            }

            ImGui.Text("-------能力技-------");
            if (AI.Instance.BattleData.HighPrioritySlots_OffGCD.Count > 0)
                foreach (var action in
                         AI.Instance.BattleData.HighPrioritySlots_OffGCD.SelectMany(spell => spell.Actions))
                {
                    ImGui.Text(action.Spell.Name);
                }

            ImGui.Text("-------GCD-------");
            if (AI.Instance.BattleData.HighPrioritySlots_GCD.Count > 0)
                foreach (var action in AI.Instance.BattleData.HighPrioritySlots_GCD.SelectMany(spell => spell.Actions))
                {
                    ImGui.Text(action.Spell.Name);
                }

            ImGui.TreePop();
        }

        ImGui.Separator();
        // if (ImGui.TreeNode("当前目标"))
        // {
        //     ImGui.Text("当前目标：" + Helper.自身目标.Name);
        //     ImGui.Text($"当前目标：{Helper.自身目标.GameObjectId}");
        //     ImGui.Text($"距离:{Helper.自身目标.Distance(Helper.自身)}");
        //     ImGui.TreePop();
        // }
        // ImGui.Separator();
        // if (ImGui.TreeNode("小队"))
        // {
        //     ImGui.Text($"周围小队成员数量：{PartyHelper.Party.Count}");
        //     ImGui.Text("小队成员:");
        //     if (PartyHelper.Party.Count > 0)
        //     {
        //         foreach (var t in PartyHelper.Party)
        //         {
        //             ImGui.Separator();
        //             ImGui.Text($"姓名:{t.Name}");
        //             ImGui.Text($"战斗状态:{Helper.目标战斗状态(t)}");
        //             ImGui.Text($"死亡状态:{t.IsDead}");
        //             ImGui.Text($"距离:{t.Distance(Helper.自身)}");
        //         }
        //     }
        //     ImGui.TreePop();
        // }
        // ImGui.Separator();
        // if (ImGui.TreeNode("技能释放"))
        // {
        //     ImGui.Text($"上个技能：{Core.Resolve<MemApiSpellCastSuccess>().LastSpell.GetSpell().Name}");
        //     ImGui.Text($"上个GCD：{Core.Resolve<MemApiSpellCastSuccess>().LastGcd.GetSpell().Name}");
        //     ImGui.Text($"上个能力技：{Core.Resolve<MemApiSpellCastSuccess>().LastAbility.GetSpell().Name}");
        //     ImGui.Text($"读条状态:{Helper.自身.IsCasting}");
        //     if (Helper.自身.IsCasting) ImGui.Text($"当前正在读条:{Core.Me.CastActionId}-{Core.Me.CastActionId.GetSpell().Name}");
        //     ImGui.TreePop();
        // }
        // ImGui.Separator();
    }
}