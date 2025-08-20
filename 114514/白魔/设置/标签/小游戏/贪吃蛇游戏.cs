using System.Numerics;
using ImGuiNET;

namespace icen.白魔.Utilities.标签.小游戏
{
    public class 贪吃蛇游戏 : I游戏接口
    {
        // 游戏配置
        private enum 难度级别 { 简单, 中等, 困难 }
        private enum 游戏模式 { 经典, 穿墙, 迷宫 }
        private enum 地图类型 { 开放, 边界, 十字, 回廊 }
        
        // 游戏状态
        private 难度级别 当前难度 = 难度级别.中等;
        private 游戏模式 当前模式 = 游戏模式.经典;
        private 地图类型 当前地图 = 地图类型.开放;
        private List<Vector2> 蛇身 = new List<Vector2>();
        private List<Vector2> 食物位置 = new List<Vector2>();
        private List<Vector2> 墙壁位置 = new List<Vector2>();
        private Vector2 移动方向 = new Vector2(1, 0);
        private Vector2 下一个方向 = new Vector2(1, 0);
        private float 移动速度 = 0.1f;
        private float 移动计时器 = 0f;
        private bool 游戏结束 = false;
        private bool 游戏暂停 = false;
        private bool 游戏开始 = false; // 新增游戏开始状态
        private int 分数 = 0;
        private int 最高分 = 0;
        private int 吃食物计数 = 0;
        private float 速度系数 = 5f;
        private const int 格子大小 = 15;
        private const int 区域宽度 = 40;
        private const int 区域高度 = 30;
        private Random 随机 = new Random();
        private string 状态表情 = "😊";
        private bool 显示网格 = true;

        public 贪吃蛇游戏()
        {
            初始化游戏();
        }

        private void 初始化游戏()
        {
            游戏开始 = false;
            游戏结束 = false;
            游戏暂停 = false;
            状态表情 = "😊";
            
            // 设置速度
            设置难度(当前难度);
            
            // 生成地图
            生成地图();
        }

        private void 开始游戏()
        {
            游戏开始 = true;
            游戏结束 = false;
            游戏暂停 = false;
            蛇身.Clear();
            蛇身.Add(new Vector2(20, 15)); // 蛇头
            移动方向 = new Vector2(1, 0);
            下一个方向 = new Vector2(1, 0);
            分数 = 0;
            吃食物计数 = 0;
            状态表情 = "😊";
            
            生成食物();
        }

        private void 设置难度(难度级别 难度)
        {
            当前难度 = 难度;
            switch (难度)
            {
                case 难度级别.简单:
                    速度系数 = 3f;
                    break;
                case 难度级别.中等:
                    速度系数 = 5f;
                    break;
                case 难度级别.困难:
                    速度系数 = 8f;
                    break;
            }
            移动速度 = 1f / 速度系数;
        }

        private void 生成地图()
        {
            墙壁位置.Clear();
            
            switch (当前地图)
            {
                case 地图类型.边界:
                    // 创建边界墙
                    for (int x = 0; x < 区域宽度; x++)
                    {
                        if (x < 5 || x > 区域宽度 - 6)
                        {
                            墙壁位置.Add(new Vector2(x, 0));
                            墙壁位置.Add(new Vector2(x, 区域高度 - 1));
                        }
                    }
                    for (int y = 0; y < 区域高度; y++)
                    {
                        if (y < 5 || y > 区域高度 - 6)
                        {
                            墙壁位置.Add(new Vector2(0, y));
                            墙壁位置.Add(new Vector2(区域宽度 - 1, y));
                        }
                    }
                    break;
                
                case 地图类型.十字:
                    // 修复十字地图：确保在有效范围内生成墙
                    int 中心X = 区域宽度 / 2;
                    int 中心Y = 区域高度 / 2;
                    
                    // 水平墙 (确保在边界内)
                    int 水平起点 = Math.Max(5, 0);
                    int 水平终点 = Math.Min(区域宽度 - 6, 区域宽度 - 1);
                    for (int x = 水平起点; x <= 水平终点; x++)
                    {
                        墙壁位置.Add(new Vector2(x, 中心Y));
                    }
                    
                    // 垂直墙 (确保在边界内)
                    int 垂直起点 = Math.Max(5, 0);
                    int 垂直终点 = Math.Min(区域高度 - 6, 区域高度 - 1);
                    for (int y = 垂直起点; y <= 垂直终点; y++)
                    {
                        墙壁位置.Add(new Vector2(中心X, y));
                    }
                    break;
                
                case 地图类型.回廊:
                    // 创建回廊墙
                    for (int x = 5; x < 区域宽度 - 5; x++)
                    {
                        墙壁位置.Add(new Vector2(x, 5));
                        墙壁位置.Add(new Vector2(x, 区域高度 - 6));
                    }
                    for (int y = 5; y < 区域高度 - 5; y++)
                    {
                        墙壁位置.Add(new Vector2(5, y));
                        墙壁位置.Add(new Vector2(区域宽度 - 6, y));
                    }
                    break;
            }
        }

        private void 生成食物()
        {
            食物位置.Clear();
            
            // 生成1-3个食物
            int 食物数量 = 1;
            if (吃食物计数 > 10) 食物数量 = 2;
            if (吃食物计数 > 20) 食物数量 = 3;
            
            for (int i = 0; i < 食物数量; i++)
            {
                Vector2 新位置;
                bool 位置有效;
                int 尝试次数 = 0;
                
                do {
                    位置有效 = true;
                    新位置 = new Vector2(随机.Next(1, 区域宽度 - 1), 随机.Next(1, 区域高度 - 1));
                    
                    // 检查是否在蛇身上
                    foreach (var 节点 in 蛇身)
                    {
                        if (新位置 == 节点)
                        {
                            位置有效 = false;
                            break;
                        }
                    }
                    
                    // 检查是否在墙上
                    foreach (var 墙 in 墙壁位置)
                    {
                        if (新位置 == 墙)
                        {
                            位置有效 = false;
                            break;
                        }
                    }
                    
                    // 检查是否与其他食物重叠
                    foreach (var 食物 in 食物位置)
                    {
                        if (新位置 == 食物)
                        {
                            位置有效 = false;
                            break;
                        }
                    }
                    
                    尝试次数++;
                    if (尝试次数 > 100) break; // 防止无限循环
                    
                } while (!位置有效);
                
                if (位置有效)
                {
                    食物位置.Add(新位置);
                }
            }
        }

        public void 绘制()
        {
            // 更新游戏状态
            if (分数 > 最高分) 最高分 = 分数;
            
            // 绘制控制面板
            绘制控制面板();
            
            // 绘制游戏区域
            ImGui.Separator();
            ImGui.BeginChild("游戏区域", new Vector2(0, 0), true);
            
            if (游戏开始)
            {
                绘制游戏区域();
            }
            else
            {
                // 显示游戏准备界面
                var 窗口大小 = ImGui.GetWindowSize();
                var 文本位置 = new Vector2(窗口大小.X / 2 - 100, 窗口大小.Y / 2 - 30);
                ImGui.SetCursorPos(文本位置);
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "请点击开始游戏按钮开始游戏");
            }
            
            ImGui.EndChild();
            
            // 更新游戏逻辑
            if (游戏开始 && !游戏结束 && !游戏暂停)
            {
                更新游戏逻辑();
            }
        }
        
        private void 绘制控制面板()
        {
            // 游戏状态
            ImGui.Text("贪吃蛇游戏");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0, 1, 0, 1), 状态表情);
            ImGui.SameLine();
            
            if (游戏结束)
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "游戏结束! 分数: " + 分数);
            }
            else if (游戏暂停)
            {
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "游戏暂停 - 分数: " + 分数);
            }
            else if (游戏开始)
            {
                ImGui.Text("分数: " + 分数);
            }
            else
            {
                ImGui.Text("准备开始游戏...");
            }
            
            // 开始/重新开始按钮
            if (!游戏开始 || 游戏结束)
            {
                if (ImGui.Button("开始游戏", new Vector2(100, 40)))
                {
                    开始游戏();
                }
            }
            else
            {
                ImGui.SameLine(ImGui.GetWindowWidth() - 220);
                if (ImGui.Button(游戏暂停 ? "继续游戏" : "暂停游戏", new Vector2(100, 40)))
                {
                    游戏暂停 = !游戏暂停;
                    状态表情 = 游戏暂停 ? "😴" : "😊";
                }
                
                ImGui.SameLine();
                if (ImGui.Button("重新开始", new Vector2(100, 40)))
                {
                    开始游戏();
                }
            }
            
            // 难度选择
            ImGui.Separator();
            ImGui.Text("难度:");
            ImGui.SameLine();
            if (ImGui.RadioButton("简单", 当前难度 == 难度级别.简单)) 设置难度(难度级别.简单);
            ImGui.SameLine();
            if (ImGui.RadioButton("中等", 当前难度 == 难度级别.中等)) 设置难度(难度级别.中等);
            ImGui.SameLine();
            if (ImGui.RadioButton("困难", 当前难度 == 难度级别.困难)) 设置难度(难度级别.困难);
            
            // 游戏模式
            ImGui.Text("模式:");
            ImGui.SameLine();
            if (ImGui.RadioButton("经典", 当前模式 == 游戏模式.经典)) 
            {
                当前模式 = 游戏模式.经典;
                初始化游戏();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("穿墙", 当前模式 == 游戏模式.穿墙)) 
            {
                当前模式 = 游戏模式.穿墙;
                初始化游戏();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("迷宫", 当前模式 == 游戏模式.迷宫)) 
            {
                当前模式 = 游戏模式.迷宫;
                初始化游戏();
            }
            
            // 地图选择
            ImGui.Text("地图:");
            ImGui.SameLine();
            if (ImGui.RadioButton("开放", 当前地图 == 地图类型.开放)) 
            {
                当前地图 = 地图类型.开放;
                初始化游戏();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("边界", 当前地图 == 地图类型.边界)) 
            {
                当前地图 = 地图类型.边界;
                初始化游戏();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("十字", 当前地图 == 地图类型.十字)) 
            {
                当前地图 = 地图类型.十字;
                初始化游戏();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("回廊", 当前地图 == 地图类型.回廊)) 
            {
                当前地图 = 地图类型.回廊;
                初始化游戏();
            }
            
            // 游戏设置
            ImGui.Text("游戏速度:");
            ImGui.SameLine();
            if (ImGui.SliderFloat("##speed", ref 速度系数, 1f, 15f, "x%.1f"))
            {
                移动速度 = 1f / 速度系数;
            }
            
            ImGui.SameLine();
            ImGui.Checkbox("显示网格", ref 显示网格);
            
            // 游戏信息
            ImGui.Text($"最高分: {最高分}  长度: {(游戏开始 ? 蛇身.Count : 0)}  食物: {吃食物计数}");
        }
        
        private void 绘制游戏区域()
        {
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 窗口位置 = ImGui.GetCursorScreenPos();
            var 区域大小 = new Vector2(区域宽度 * 格子大小, 区域高度 * 格子大小);
            
            // 绘制背景
            绘制列表.AddRectFilled(窗口位置, 窗口位置 + 区域大小, 
                ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.1f, 1)));
            
            // 绘制网格
            if (显示网格)
            {
                uint 网格线颜色 = ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1));
                for (int x = 0; x <= 区域宽度; x++)
                {
                    var 起点 = 窗口位置 + new Vector2(x * 格子大小, 0);
                    var 终点 = 起点 + new Vector2(0, 区域大小.Y);
                    绘制列表.AddLine(起点, 终点, 网格线颜色);
                }
                for (int y = 0; y <= 区域高度; y++)
                {
                    var 起点 = 窗口位置 + new Vector2(0, y * 格子大小);
                    var 终点 = 起点 + new Vector2(区域大小.X, 0);
                    绘制列表.AddLine(起点, 终点, 网格线颜色);
                }
            }
            
            // 绘制墙壁
            foreach (var 墙 in 墙壁位置)
            {
                var 位置 = 窗口位置 + new Vector2(墙.X * 格子大小, 墙.Y * 格子大小);
                绘制列表.AddRectFilled(位置, 位置 + new Vector2(格子大小, 格子大小), 
                    ImGui.GetColorU32(new Vector4(0.4f, 0.2f, 0.6f, 1)));
            }
            
            // 绘制食物
            foreach (var 食物 in 食物位置)
            {
                var 位置 = 窗口位置 + new Vector2(食物.X * 格子大小, 食物.Y * 格子大小);
                var 中心 = 位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                
                // 绘制苹果形状
                绘制列表.AddCircleFilled(中心, 格子大小 / 2, 
                    ImGui.GetColorU32(new Vector4(1, 0.2f, 0.2f, 1)));
                
                // 苹果叶子
                var 叶子位置 = 位置 + new Vector2(格子大小 * 0.7f, 格子大小 * 0.3f);
                绘制列表.AddCircleFilled(叶子位置, 格子大小 / 4, 
                    ImGui.GetColorU32(new Vector4(0.2f, 0.8f, 0.2f, 1)));
            }
            
            // 绘制蛇
            for (int i = 0; i < 蛇身.Count; i++)
            {
                var 节点 = 蛇身[i];
                var 位置 = 窗口位置 + new Vector2(节点.X * 格子大小, 节点.Y * 格子大小);
                
                // 蛇头用不同颜色
                uint 颜色;
                if (i == 0)
                {
                    颜色 = ImGui.GetColorU32(new Vector4(0, 0.8f, 0, 1)); // 绿色头
                    
                    // 绘制眼睛
                    Vector2 左眼偏移, 右眼偏移;
                    
                    if (移动方向.X > 0)
                    {
                        左眼偏移 = new Vector2(格子大小 - 4, 格子大小 / 3);
                        右眼偏移 = new Vector2(格子大小 - 4, 2 * 格子大小 / 3);
                    }
                    else if (移动方向.X < 0)
                    {
                        左眼偏移 = new Vector2(4, 格子大小 / 3);
                        右眼偏移 = new Vector2(4, 2 * 格子大小 / 3);
                    }
                    else if (移动方向.Y > 0)
                    {
                        左眼偏移 = new Vector2(格子大小 / 3, 格子大小 - 4);
                        右眼偏移 = new Vector2(2 * 格子大小 / 3, 格子大小 - 4);
                    }
                    else
                    {
                        左眼偏移 = new Vector2(格子大小 / 3, 4);
                        右眼偏移 = new Vector2(2 * 格子大小 / 3, 4);
                    }
                    
                    绘制列表.AddCircleFilled(位置 + 左眼偏移, 2, 
                        ImGui.GetColorU32(ImGuiCol.Text));
                    绘制列表.AddCircleFilled(位置 + 右眼偏移, 2, 
                        ImGui.GetColorU32(ImGuiCol.Text));
                }
                else
                {
                    // 蛇身渐变色
                    float 渐变 = 1f - (float)i / 蛇身.Count;
                    颜色 = ImGui.GetColorU32(new Vector4(0, 渐变 * 0.6f, 0, 1));
                }
                
                绘制列表.AddRectFilled(位置 + new Vector2(1, 1), 
                   位置 + new Vector2(格子大小 - 1, 格子大小 - 1), 颜色);
                
                // 蛇身连接处
                if (i > 0)
                {
                    var 上一个位置 = 窗口位置 + new Vector2(蛇身[i-1].X * 格子大小, 蛇身[i-1].Y * 格子大小);
                    var 中心 = 位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                    var 上一个中心 = 上一个位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                    绘制列表.AddLine(中心, 上一个中心, 
                        ImGui.GetColorU32(new Vector4(0, 0.5f, 0, 1)), 3);
                }
            }
            
            // 控制按钮
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            ImGui.Text("控制方向:");
            
            if (ImGui.ArrowButton("上", ImGuiDir.Up)) 尝试改变方向(new Vector2(0, -1));
            ImGui.SameLine();
            if (ImGui.ArrowButton("左", ImGuiDir.Left)) 尝试改变方向(new Vector2(-1, 0));
            ImGui.SameLine();
            if (ImGui.ArrowButton("右", ImGuiDir.Right)) 尝试改变方向(new Vector2(1, 0));
            ImGui.SameLine();
            if (ImGui.ArrowButton("下", ImGuiDir.Down)) 尝试改变方向(new Vector2(0, 1));
        }
        
        private void 尝试改变方向(Vector2 新方向)
        {
            // 防止180度转向
            if (移动方向 + 新方向 != Vector2.Zero)
            {
                下一个方向 = 新方向;
            }
        }
        
        private void 更新游戏逻辑()
        {
            移动计时器 += ImGui.GetIO().DeltaTime;
            if (移动计时器 >= 移动速度)
            {
                移动计时器 = 0;
                移动方向 = 下一个方向;
                更新蛇位置();
            }
            
            // 处理键盘输入
            var io = ImGui.GetIO();
            if (io.KeysDown[(int)ImGuiKey.UpArrow]) 尝试改变方向(new Vector2(0, -1));
            if (io.KeysDown[(int)ImGuiKey.DownArrow]) 尝试改变方向(new Vector2(0, 1));
            if (io.KeysDown[(int)ImGuiKey.LeftArrow]) 尝试改变方向(new Vector2(-1, 0));
            if (io.KeysDown[(int)ImGuiKey.RightArrow]) 尝试改变方向(new Vector2(1, 0));
        }
        
        private void 更新蛇位置()
        {
            // 计算新头部位置
            Vector2 新头部位置 = 蛇身[0] + 移动方向;
            
            // 穿墙模式处理
            if (当前模式 == 游戏模式.穿墙)
            {
                if (新头部位置.X < 0) 新头部位置.X = 区域宽度 - 1;
                if (新头部位置.Y < 0) 新头部位置.Y = 区域高度 - 1;
                if (新头部位置.X >= 区域宽度) 新头部位置.X = 0;
                if (新头部位置.Y >= 区域高度) 新头部位置.Y = 0;
            }
            
            // 检查是否吃到食物
            bool 吃到食物 = false;
            Vector2? 吃掉的食物 = null;
            
            foreach (var 食物 in 食物位置)
            {
                if (新头部位置 == 食物)
                {
                    吃到食物 = true;
                    吃掉的食物 = 食物;
                    break;
                }
            }
            
            // 移动蛇身
            蛇身.Insert(0, 新头部位置);
            if (!吃到食物)
            {
                蛇身.RemoveAt(蛇身.Count - 1);
            }
            else
            {
                // 移除被吃掉的食物
                if (吃掉的食物.HasValue)
                {
                    食物位置.Remove(吃掉的食物.Value);
                }
                
                // 增加分数
                分数 += 10 * (int)速度系数;
                吃食物计数++;
                状态表情 = "😋";
                
                // 生成新食物
                生成食物();
            }
            
            // 检查碰撞
            检查碰撞();
        }
        
        private void 检查碰撞()
        {
            Vector2 头 = 蛇身[0];
            
            // 边界检查 (经典和迷宫模式)
            if (当前模式 != 游戏模式.穿墙)
            {
                if (头.X < 0 || 头.Y < 0 || 头.X >= 区域宽度 || 头.Y >= 区域高度)
                {
                    游戏结束 = true;
                    状态表情 = "💀";
                    return;
                }
            }
            
            // 墙壁碰撞检查
            foreach (var 墙 in 墙壁位置)
            {
                if (头 == 墙)
                {
                    游戏结束 = true;
                    状态表情 = "💥";
                    return;
                }
            }
            
            // 自身检查（从第4节开始检查，避免小蛇自杀）
            for (int i = 4; i < 蛇身.Count; i++)
            {
                if (头 == 蛇身[i])
                {
                    游戏结束 = true;
                    状态表情 = "🌀";
                    return;
                }
            }
        }
    }
}