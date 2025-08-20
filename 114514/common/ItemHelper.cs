using FFXIVClientStructs.FFXIV.Client.Game;

namespace icen.common;

public class ItemHelper
{
    /// <summary>
    /// 获取背包内指定物品的数量
    /// </summary>
    public static unsafe int FindItem(uint itemId, ItemFlag flag = ItemFlag.NQHQ)
    {
        var count = 0;

        for (var i = 0; i < 4; ++i)
        {
            var container = InventoryManager.Instance()->GetInventoryContainer((InventoryType)i);
            for (var j = 0; j < container->Size; j++)
            {
                var item = container->GetInventorySlot(j);
                if (item == null)
                    continue;
                //匹配物品id
                if (item->ItemId != itemId)
                    continue;
                //匹配flag
                var f = item->Flags;
                switch (flag)
                {
                    case ItemFlag.NQ:
                        if (f != InventoryItem.ItemFlags.None)
                            continue;
                        break;
                    case ItemFlag.HQ:
                        if (f != InventoryItem.ItemFlags.HighQuality)
                            continue;
                        break;
                    case ItemFlag.NQHQ:
                        if (f != InventoryItem.ItemFlags.HighQuality && f != InventoryItem.ItemFlags.None)
                            continue;
                        break;
                    case ItemFlag.Collectable:
                        if (f != InventoryItem.ItemFlags.Collectable)
                            continue;
                        break;
                }

                count += (int)item->Quantity;
            }
        }

        return count;
    }
}

public enum ItemFlag
{
    NQ,
    HQ,
    NQHQ,
    Collectable,
}

public enum Potion
{
    _8级刚力之幻药= 39727,
    _8级巧力之幻药= 39728,
    _8级智力之幻药= 39730,
    _8级意力之幻药= 39731,
    刚力之宝药 = 44157,
    巧力之宝药 = 44158,
    智力之宝药 = 44160,
    意力之宝药 = 44161,
    _2级刚力之宝药 = 44162,
    _2级巧力之宝药 = 44163,
    _2级智力之宝药 = 44165,
    _2级意力之宝药 = 44166,
}