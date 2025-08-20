using AEAssist.CombatRoutine;
using ICEN2.utils;
using ICEN2.utils.Triggers;
using ICEN2.utils.Triggers.Helper;
using ICEN2.白魔.循环;
using ICEN2.白魔.时间轴设置;
using ICEN2.白魔.界面;
using ICEN2.白魔.设置;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.Main;

public static class WhiteMageACR
{
    private const long Version = 202508180919;
    public static string settingFolderPath;
    // ReSharper disable once UnusedMember.Local
    private static OpenerHelper openerHelper = new OpenerHelper();
    public static void Init(string settingFolder)
    {
        settingFolderPath = settingFolder;
        GlobalSetting.Build(settingFolder,"ICEN2",false);
        默认值.Build(settingFolder);
        baseUI.Build();
        resetConfigOnVersionUpdate();
    }

    public static Rotation Build()
    {
        return new Rotation(白魔技能策略.SlotResolvers)
                {
                    TargetJob = Jobs.WhiteMage,
                    AcrType = AcrType.Both,
                    MinLevel = 0,
                    MaxLevel = 100,
                    Description = "七夏icen的白魔ACR\n更新时间：2025/8/9\n更新内容：\n移除了复活的hekey，新建了一个设置gui\n所有设置都可以在设置gui中控制\n更新了止损的逻辑，dot会优先给无dot状态的目标\n如果有bug请在DC上反馈\n如果有建议请在DC上反馈\n如果有想法请在DC上反馈\n如果有想法请在DC上反馈"

                }
                //.AddOpener(起手.起手.GetOpener)
                .AddSlotSequences(白魔特殊序列.Build())
                .AddTriggerAction(
                    new 白魔时间轴QT设置(),
                    new 白魔时间轴新QT设置(),
                    new Omega麻将状态(),
                    new Omega目标选择())
                .AddTriggerCondition(new 白魔时间轴蓝花状态(), new 白魔时间轴红花状态())
                .SetRotationEventHandler(new 事件处理器());;
                
    }

    private static void resetConfigOnVersionUpdate()
    {
        if (默认值.实例.Version < Version)
        {
            默认值.实例.Version = Version;
            默认值.实例.VersionMessage = true;
        }
        默认值.实例.Save();
    }
}