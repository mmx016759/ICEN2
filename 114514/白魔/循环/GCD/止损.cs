using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using ICEN2.common;
using ICEN2.数据;
using ICEN2.白魔.技能数据;
using ICEN2.白魔.界面.QT;

namespace ICEN2.白魔.循环.GCD;

public class 止损 : ISlotResolver
{

    public int Check()
    {
        if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹, 1000) && Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
        if (白魔Qt.GetQt("停手")) return -1;
        if (!白魔Qt.GetQt("Dot")) return -8;
        if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
        if (Helper.自身存在其中Buff(BuffBlackList.无法造成伤害)) return -1;
        if (Helper.目标血量百分比 < 0.0001) return -55;
        if (Helper.当前地图id== 1252) return -100; // 确保在白魔地图
        return 1;
    }

    public void Build(Slot slot)
    {
        var target = Helper.获取最近未被阻挡无指定buff的敌对目标(25,BuffBlackList.敌人不可被攻击表 // 排除敌人无敌/魔法无效/魔法反击状态
            .Concat(状态.Dot) // 同时排除dot状态
            .Distinct() // 确保没有重复ID
            .ToList());
        if (target == null)
        {
            target = Helper.自身目标;
            if (!Helper.目标是否被阻挡(Helper.自身,target))
            {
                slot.Add(Helper.GetActionChange(技能id.DOT).GetSpell(target));
            }
            else
            {
                target = Helper.获取最近未被阻挡无指定buff的敌对目标(25, BuffBlackList.敌人不可被攻击表);
                if (target == null)
                {
                    var 止损 = Helper.获取最低血量T();
                    if (Helper.目标是否拥有其中的BUFF(状态.再生5))
                    {
                        slot.Add(Helper.GetActionChange(技能id.再生).GetSpell());
                        ApiHelper.清除目标();
                    }
                    else
                    {
                        slot.Add(Helper.GetActionChange(技能id.再生).GetSpell(止损));
                    }
                }
                else
                {
                    slot.Add(Helper.GetActionChange(技能id.DOT).GetSpell(target));
                }
                
            }
        }
        else
        {
            slot.Add(Helper.GetActionChange(技能id.DOT).GetSpell(target));
        }

    }
}