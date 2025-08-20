using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using icen.common;
using icen.数据;
using icen.白魔.View.QT;
using icen.白魔.技能数据;

namespace icen.白魔.循环.GCD;

public class AOE : ISlotResolver
{
    
        public int Check()
        {
            if  (Helper.当前地图id==293) return -999;
            if (Helper.当前地图id==296) return -999;
            if (Helper.当前地图id==1046) return -999;
            if (!Helper.自身存在Buff大于时间(BuffBlackList.加速度炸弹 , 1000)&&Helper.自身存在其中Buff(BuffBlackList.加速度炸弹)) return -1;
            if (!技能id.Aoe.GetSpell().IsReadyWithCanCast()) return -2;
            if (白魔Qt.GetQt("停手")) return -1;
            if (!白魔Qt.GetQt("AOE")) return -1;
            if (Helper.自身存在其中Buff(BuffBlackList.无法发动技能)) return -1;
            if (Helper.自身存在其中Buff(BuffBlackList.无法造成伤害)) return -1;
            if(Core.Me.Level < 45) return -999;
            if (Core.Resolve<MemApiMove>().IsMoving())
                return -2;
            var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 0, 8);

            if (Core.Me.Level < 72)
            {
                if (aoeCount < 2) return -3;
            }
            else
            {
                if (aoeCount < 3) return -3;
            }

            return 0;
        }

        public void Build(Slot slot)
        {
            slot.Add(Helper.GetActionChange(技能id.Aoe).GetSpell());
        }
    
    

}