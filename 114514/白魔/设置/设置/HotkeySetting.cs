// HotkeySetting.cs

using AEAssist.CombatRoutine;

namespace icen.白魔.Utilities.设置
{
    public class HotkeySetting
    {
        public string Name { get; set; } = "新热键";
        public uint SpellId { get; set; }
        public SpellTargetType TargetType { get; set; } = SpellTargetType.Self;
        public bool UseHighPrioritySlot { get; set; } = true;
        public bool WaitCoolDown { get; set; } = true;
        public HotkeyResolverType ResolverType { get; set; } = HotkeyResolverType.General;
    }

    public enum HotkeyResolverType
    {
        General,
        节制,
        铃铛,
        医养,
        狂喜,
        愈疗,
        水流幕,
        庇护所
    }
}