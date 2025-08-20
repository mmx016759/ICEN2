using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.Extension;
using AEAssist.Helper;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ReplaceAutoPropertyWithComputedProperty
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace icen.utils.Triggers;

public class Omega麻将状态: ITriggerAction
{
    
    public string DisplayName { get; } = "OMEGA/P1麻将";
    public string Remark { get; set; }
    
    public uint SpellId { get; set; } = new();

    public enum 麻将
    {
        一麻,
        二麻,
        三麻,
        四麻
    }
    
    public 麻将 麻将号码 { get; set; }

    public bool Draw()
    {
        return false;
    }

    public bool Handle()
    {
        var party = PartyHelper.CastableParty;
        uint aura = 麻将号码 switch
        {
            麻将.一麻 => 3004,
            麻将.二麻 => 3005,
            麻将.三麻 => 3006,
            麻将.四麻 => 3451,
            _ => 0
        };
        var target = party.OrderBy(t => t.CurrentHp).FirstOrDefault(t => t.HasAura(aura));
        AI.Instance.BattleData.NextSlot = new Slot().Add(new Spell(SpellId, target));
        return true;
    }

}