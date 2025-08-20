// ReSharper disable MemberCanBePrivate.Global

namespace ICEN2.utils;

public static class Map
{

    public const uint 领航明灯天狼星灯塔 = 160u;
    public const uint 武装圣域放浪神古神殿 = 188u;
    public const uint 乐欲之所瓯博讷修道院 = 826u;


    public const uint 欧米茄绝境验证战_时空狭缝 = 1122u;
    public const uint 幻想龙诗绝境战_诗想空间 = 968u;
    public const uint 亚历山大绝境战_差分闭合宇宙 = 887u;
    public const uint 究极神兵绝境战_禁绝幻想 = 777u;
    public const uint 巴哈姆特绝境战_巴哈姆特大迷宫 = 733u;
    public const uint 泽罗姆斯歼殛战_红月深处 = 1169u;
    public const uint 光暗未来绝境战_另一个未来 = 1238u;
    public const uint 圆桌骑士幻巧战_奇点反应堆 = 1175u;
    public const uint 零式万魔殿荒天之狱4_施恩神座 = 1154u;
    public const uint 零式万魔殿荒天之狱3_十四席大堂 = 1152u;
    public const uint 零式万魔殿荒天之狱2_万魔的产房 = 1150u;
    public const uint 零式万魔殿荒天之狱1_滞淀海域 = 1148u;
    public const  uint 阿卡狄亚1 = 1226u;
    public const  uint 阿卡狄亚2 = 1228u;
    public const  uint 阿卡狄亚3 = 1230u;
    public const  uint 阿卡狄亚4 = 1232u;
    public const uint 异闻阿罗阿罗岛 = 1179u;
    public const uint 零式阿罗阿罗岛 = 1180u;

    public const uint 伊弗利特_炎帝陵 = 1045u;
    public const uint 究极神兵破坏作战_后营门 = 1048u;

    public static readonly List<uint> 高难地图 =
    [
        欧米茄绝境验证战_时空狭缝,
        幻想龙诗绝境战_诗想空间,
        亚历山大绝境战_差分闭合宇宙,
        究极神兵绝境战_禁绝幻想,
        巴哈姆特绝境战_巴哈姆特大迷宫,
        阿卡狄亚1,
        阿卡狄亚2,
        阿卡狄亚3,
        阿卡狄亚4,
        异闻阿罗阿罗岛,
        零式阿罗阿罗岛,
        光暗未来绝境战_另一个未来
    ];
        
    public static readonly List<uint> 不拉人地图 =
    [
        异闻阿罗阿罗岛,
        零式阿罗阿罗岛
    ];

    public const uint 优雷卡丰水之地 = 827u;
    public const uint 优雷卡常风之地 = 732u;
    public const uint 优雷卡恒冰之地 = 763u;
    public const uint 优雷卡涌火之地 = 795u;
    public const uint 南方博兹雅战线 = 920u;
    public const uint 扎杜诺尔高原 = 975u;

    public static readonly List<uint> 不挂再生地图 =
    [
        优雷卡丰水之地,
        优雷卡常风之地,
        优雷卡恒冰之地,
        优雷卡涌火之地,
        南方博兹雅战线,
        扎杜诺尔高原
    ];

    public static readonly List<uint> 四人本直接BOSS地图 = [
        伊弗利特_炎帝陵,
        究极神兵破坏作战_后营门
    ];
    
    public static readonly List<uint> 双DOT地图 =
    [
        亚历山大绝境战_差分闭合宇宙,
        幻想龙诗绝境战_诗想空间,
        光暗未来绝境战_另一个未来
    ];
}