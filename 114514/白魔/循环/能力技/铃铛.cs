using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.循环.能力技;

public class 铃铛 : ISlotResolver
{
    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (Helper.自身存在Buff(1495)) return -1;
        if (白魔Qt.GetQt("高难模式")&&白魔Qt.GetQt("铃铛")) return 1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (!白魔Qt.GetQt("铃铛")) return -1;
        if (!白魔Qt.GetQt("群奶")) return -1;
        if (!技能id.礼仪之铃.GetSpell().IsReadyWithCanCast()) return -2;
        if (!默认值.实例.流血自动铃铛) return -1;
        if (Helper.自身存在Buff(状态.礼仪之铃)) return -1;
        var buffId = Helper.自身命中其中Buff(状态.流血BUFF, 5000);
        if (buffId != 0) return 1;
        return -6;
    }

    public void Build(Slot slot)
    {
        var target = 默认值.实例.庇护所目标 
            ? Helper.自身   // 配置为true时选择自身当前目标
            : Helper.自身目标;                // 配置为false时选择自身

        // 当配置要求自身目标但目标不存在时，回退到自身
        if (target == null)
        {
            target = Core.Me;
        }
        slot.Add(Helper.GetActionChange(技能id.礼仪之铃).GetSpell(target));
    }
}