using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Textures.TextureWraps;
using icen.common;
using icen.白魔.技能数据;
using ImGuiNET;

// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace icen.utils.JobView.HotKey;

public class HotKeyResolver(Spell p, HotKeyTarget hotKeyTarget) : IHotkeyResolver
{
    private HotKeyTarget T = hotKeyTarget;
    private IBattleChara? characterAgent = hotKeyTarget.CharacterAgent;
    private SpellTargetType? spellTargetType = hotKeyTarget.SpellTargetType;
    private Func<IBattleChara?>? targetFunc = hotKeyTarget.Func;


    public int Check()
    {
        return 1;
    }

    public void Draw(Vector2 size)
    {
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(p.Id, out IDalamudTextureWrap textureWrap))
            return;
        if (textureWrap != null) ImGui.Image(textureWrap.ImGuiHandle, size1);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        if (characterAgent != null)
        {
            SpellHelper.DrawSpellInfo(new Spell(p.Id, characterAgent), size, isActive,
                myDrawSpellDelegate);
        }
        else if (spellTargetType != null)
        {
            SpellHelper.DrawSpellInfo(new Spell(p.Id, (SpellTargetType)spellTargetType), size, isActive,
                myDrawSpellDelegate);
        }
        else
        {
            if (targetFunc != null)
                SpellHelper.DrawSpellInfo(new Spell(p.Id, targetFunc), size, isActive, myDrawSpellDelegate);
        }
    }

    private void myDrawSpellDelegate(Spell s, Vector2 hotKeySize, bool isActive)
    {
        if (s.IsAbility())
        {
            if (s.IsReadyWithCanCast())
            {
                ImGui.SetCursorPos(new Vector2(0, 0));
                if (isActive)
                {
                    //激活状态
                    if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\activeaction.png",
                            out IDalamudTextureWrap? textureWrap_active))
                        if (textureWrap_active != null)
                            ImGui.Image(textureWrap_active.ImGuiHandle, hotKeySize);
                }
                else
                {
                    //常规状态
                    if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\iconframe.png",
                            out IDalamudTextureWrap? textureWrap_normal))
                        if (textureWrap_normal != null)
                            ImGui.Image(textureWrap_normal.ImGuiHandle, hotKeySize);
                }
            }
            else
            {
                //技能不可使用
                //变黑
                ImGui.SetCursorPos(new Vector2(0, 0));
                if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\icona_frame_disabled.png",
                        out IDalamudTextureWrap? textureWrap_black))
                    if (textureWrap_black != null)
                        ImGui.Image(textureWrap_black.ImGuiHandle, hotKeySize);
            }
        }
        else
        {
            if (s.Id == 技能id.狂喜之心 && Helper.白魔量谱_蓝花数量() < 1)
            {
                ImGui.SetCursorPos(new Vector2(0, 0));
                if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\icona_frame_disabled.png",
                        out IDalamudTextureWrap? textureWrap_black))
                    if (textureWrap_black != null)
                        ImGui.Image(textureWrap_black.ImGuiHandle, hotKeySize);
                return;
            }
            if (s.Id == 技能id.苦难之心 && Helper.白魔量谱_红花数量() != 3)
            {
                ImGui.SetCursorPos(new Vector2(0, 0));
                if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\icona_frame_disabled.png",
                        out IDalamudTextureWrap? textureWrap_black))
                    if (textureWrap_black != null)
                        ImGui.Image(textureWrap_black.ImGuiHandle, hotKeySize);
                return;
            }
                
                
            //技能可使用
            ImGui.SetCursorPos(new Vector2(0, 0));
            if (isActive)
            {
                //激活状态
                if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\activeaction.png",
                        out IDalamudTextureWrap? textureWrap_active))
                    if (textureWrap_active != null)
                        ImGui.Image(textureWrap_active.ImGuiHandle, hotKeySize);
            }
            else
            {
                //常规状态
                if (Core.Resolve<MemApiIcon>().TryGetTexture(@"Resources\Spells\Icon\iconframe.png",
                        out IDalamudTextureWrap? textureWrap_normal))
                    if (textureWrap_normal != null)
                        ImGui.Image(textureWrap_normal.ImGuiHandle, hotKeySize);
            }
        }
    }


    public void Run()
    {
        
        if (p.Id is 技能id.复活)
        {
            if (Core.Me.IsCasting)
            {
                return;
            }
            if (Helper.自身当前等级 < 12)
            {
                return;
            }
            if (!技能id.复活.IsUnlockWithCDCheck())
            {
                return;
            }
            if (Map.不拉人地图.Contains(Helper.当前地图id))
            {
                return;
            }

            if (Helper.自身蓝量 < 2400 && !技能id.无中生有.GetSpell().IsReadyWithCanCast())
            {
                return;
            }
            
            if (技能id.即刻咏唱.GetSpell().IsReadyWithCanCast())
            {
                if (AI.Instance.BattleData.NextSlot != null)
                {
                    AI.Instance.BattleData.NextSlot.Add(new Spell(技能id.即刻咏唱, SpellTargetType.Self));
                }
                else
                {
                    AI.Instance.BattleData.NextSlot =
                        new Slot().Add(new Spell(技能id.即刻咏唱, SpellTargetType.Self));
                }
            }

            if (技能id.无中生有.GetSpell().IsReadyWithCanCast())
            {
                if (AI.Instance.BattleData.NextSlot != null)
                {
                    AI.Instance.BattleData.NextSlot.Add(new Spell(技能id.无中生有, SpellTargetType.Self));
                }
                else
                {
                    AI.Instance.BattleData.NextSlot =
                        new Slot().Add(new Spell(技能id.无中生有, SpellTargetType.Self));
                }
            }
        }
        
        if (p.Id is 技能id.医养 or 技能id.医济)
        {
            var sp = 技能id.医济;
            
            if (Helper.自身当前等级 >= 96)
            {
                sp = 技能id.医养;
            }
            
            if (AI.Instance.BattleData.NextSlot != null)
            {
                AI.Instance.BattleData.NextSlot.Add(new Spell(sp, Core.Me));
            }
            else
            {
                AI.Instance.BattleData.NextSlot = new Slot().Add(new Spell(sp, Core.Me));
            }
            
            return;
        }
        
        if (spellTargetType != null)
        {
            if (AI.Instance.BattleData.NextSlot != null)
            {
                AI.Instance.BattleData.NextSlot.Add(new Spell(p.Id, (SpellTargetType)spellTargetType));
            }
            else
            {
                AI.Instance.BattleData.NextSlot =
                    new Slot().Add(new Spell(p.Id, (SpellTargetType)spellTargetType));
            }
        }
        else
        {
            IBattleChara target = Core.Me;
            if (characterAgent != null)
            {
                target = characterAgent;
            }
            else
            {
                if (targetFunc != null)
                    target = targetFunc();
            }
            
            if (target == null) return;
            
            if (AI.Instance.BattleData.NextSlot != null)
            {
                AI.Instance.BattleData.NextSlot.Add(new Spell(p.Id, target));
            }
            else
            {
                AI.Instance.BattleData.NextSlot = new Slot().Add(new Spell(p.Id, target));
            }

        }
    }
}