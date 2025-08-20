using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;
using ICEN2.白魔.设置.设置;

namespace ICEN2.白魔.循环.GCD;

public class 单体输出 : ISlotResolver
{
    public int Check()
    {            
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (!技能id.单体gcd.GetSpell().IsReadyWithCanCast()) return -114514;
        if (Helper.自身存在其中Buff(BuffBlackList.无法造成伤害)) return -1;
        if (白魔Qt.GetQt("停手")) return -1;
        if (Helper.自身是否在移动()) return -4;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -3;
        if (Helper.目标是否拥有其中的BUFF(BuffBlackList.敌人不可被攻击表)) return -3;
        if (Helper.目标血量百分比<0.00001)  return -55;
        if (Helper.目标到自身距离()>22+SettingMgr.GetSetting<GeneralSettings>().AttackRange) return -55;
        return 1;
    }

    public void Build(Slot slot)
    {
        默认值.实例.读条 =1;
        slot.Add(Helper.GetActionChange(技能id.单体gcd).GetSpell());
    }
}