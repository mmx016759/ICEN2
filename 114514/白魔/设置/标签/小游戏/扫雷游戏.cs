using System.Numerics;
using ImGuiNET;

namespace icen.白魔.Utilities.标签.小游戏
{
    public class 扫雷游戏 : I游戏接口
    {
        // 难度级别定义
        public enum 难度级别
        {
            初级,
            中级,
            高级,
            自定义
        }
        
        // 难度配置
        private static readonly Dictionary<难度级别, (int 宽度, int 高度, int 雷数)> 默认难度配置 = new Dictionary<难度级别, (int, int, int)>
        {
            { 难度级别.初级, (8, 8, 10) },
            { 难度级别.中级, (12, 12, 25) },
            { 难度级别.高级, (16, 16, 50) },
            { 难度级别.自定义, (8, 8, 10) } // 默认值
        };
        
        // 格子状态
        private enum 格子状态 { 未翻开, 翻开, 标记 }
        
        // 游戏状态
        private 难度级别 当前难度 = 难度级别.初级;
        private int 网格宽度;
        private int 网格高度;
        private int 总雷数;
        private int 剩余雷数;
        private 格子状态[,] 格子状态数组;
        private bool[,] 雷区;
        private bool 游戏结束 = false;
        private bool 游戏胜利 = false;
        private bool 首次点击 = true;
        private float 游戏时间 = 0f;
        private const int 格子大小 = 25;
        private Random 随机 = new Random();
        private string 状态表情 = "😊"; // 默认笑脸
        private Vector2 网格起始位置;
        private (int x, int y)? 悬停格子 = null;
        private bool 显示自定义设置 = false;
        private int 自定义宽度 = 8;
        private int 自定义高度 = 8;
        private int 自定义雷数 = 10;

        public 扫雷游戏()
        {
            设置难度(当前难度);
        }

        private void 设置难度(难度级别 难度)
        {
            当前难度 = 难度;
            (网格宽度, 网格高度, 总雷数) = 默认难度配置[难度];
            重置游戏();
        }
        
        private void 设置自定义难度(int 宽度, int 高度, int 雷数)
        {
            网格宽度 = Math.Clamp(宽度, 5, 30);
            网格高度 = Math.Clamp(高度, 5, 30);
            总雷数 = Math.Clamp(雷数, 1, 网格宽度 * 网格高度 - 9); // 至少保留9个安全格子
            重置游戏();
        }

        private void 重置游戏()
        {
            // 重置游戏状态
            格子状态数组 = new 格子状态[网格宽度, 网格高度];
            雷区 = new bool[网格宽度, 网格高度];
            游戏结束 = false;
            游戏胜利 = false;
            首次点击 = true;
            游戏时间 = 0f;
            剩余雷数 = 总雷数;
            状态表情 = "😊";
            悬停格子 = null;
            
            // 初始化所有格子
            for (int y = 0; y < 网格高度; y++)
            {
                for (int x = 0; x < 网格宽度; x++)
                {
                    格子状态数组[x, y] = 格子状态.未翻开;
                    雷区[x, y] = false;
                }
            }
        }

        public void 绘制()
        {
            // 更新游戏时间
            if (!游戏结束 && !游戏胜利 && !首次点击)
            {
                游戏时间 += ImGui.GetIO().DeltaTime;
            }
            
            // 难度选择器
            ImGui.Text("难度选择:");
            ImGui.SameLine();
            if (ImGui.Button("初级"))
            {
                设置难度(难度级别.初级);
                return;
            }
            ImGui.SameLine();
            if (ImGui.Button("中级"))
            {
                设置难度(难度级别.中级);
                return;
            }
            ImGui.SameLine();
            if (ImGui.Button("高级"))
            {
                设置难度(难度级别.高级);
                return;
            }
            ImGui.SameLine();
            if (ImGui.Button("自定义"))
            {
                显示自定义设置 = true;
            }
            
            // 自定义难度设置窗口
            if (显示自定义设置)
            {
                ImGui.Begin("自定义难度", ImGuiWindowFlags.AlwaysAutoResize);
                
                ImGui.Text("宽度 (5-30):");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("##宽度", ref 自定义宽度);
                
                ImGui.Text("高度 (5-30):");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("##高度", ref 自定义高度);
                
                ImGui.Text("地雷数:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("##雷数", ref 自定义雷数);
                
                ImGui.Spacing();
                
                if (ImGui.Button("确定"))
                {
                    设置自定义难度(自定义宽度, 自定义高度, 自定义雷数);
                    显示自定义设置 = false;
                }
                
                ImGui.SameLine();
                if (ImGui.Button("取消"))
                {
                    显示自定义设置 = false;
                }
                
                ImGui.End();
            }
            
            // 游戏状态面板
            ImGui.Separator();
            ImGui.BeginGroup();
            
            // 状态表情按钮 (重新开始)
            float 按钮大小 = 40;
            Vector2 按钮位置 = ImGui.GetCursorPos();
            if (ImGui.Button("重新开始", new Vector2(按钮大小, 按钮大小)))
            {
                重置游戏();
            }
            
            // 绘制表情
            var 绘制列表 = ImGui.GetWindowDrawList();
            var 表情位置 = ImGui.GetItemRectMin() + new Vector2(按钮大小 / 2, 按钮大小 / 2);
            绘制列表.AddText(表情位置 - new Vector2(8, 8), ImGui.GetColorU32(ImGuiCol.Text), 状态表情);
            
            // 游戏信息
            ImGui.SameLine();
            ImGui.SetCursorPosX(按钮位置.X + 按钮大小 + 20);
            ImGui.BeginGroup();
            
            // 剩余雷数
            ImGui.Text("剩余雷数:");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1, 0.2f, 0.2f, 1), 剩余雷数.ToString());
            
            // 游戏时间
            ImGui.Text("游戏时间:");
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.2f, 0.6f, 1, 1), $"{游戏时间:0.00}秒");
            
            // 游戏状态
            if (游戏结束)
            {
                ImGui.Text("游戏状态:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "游戏结束!");
            }
            else if (游戏胜利)
            {
                ImGui.Text("游戏状态:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0, 0.8f, 0, 1), "胜利! 时间: " + 游戏时间.ToString("0.00") + "秒");
            }
            else
            {
                ImGui.Text("游戏状态:");
                ImGui.SameLine();
                ImGui.Text("游戏中...");
            }
            
            ImGui.EndGroup();
            ImGui.EndGroup();
            
            ImGui.Separator();
            
            // 绘制游戏网格
            ImGui.BeginChild("扫雷网格", new Vector2(0, 0), true);
            绘制扫雷网格();
            ImGui.EndChild();
        }
        
private void 绘制扫雷网格()
{
    var 绘制列表 = ImGui.GetWindowDrawList();
    网格起始位置 = ImGui.GetCursorScreenPos();
    
    // 计算网格总大小
    float 网格宽度像素 = 网格宽度 * 格子大小;
    float 网格高度像素 = 网格高度 * 格子大小;
    
    // 绘制网格背景
    绘制列表.AddRectFilled(网格起始位置, 网格起始位置 + new Vector2(网格宽度像素, 网格高度像素), 
        ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1)));
    
    // 绘制网格线
    uint 网格线颜色 = ImGui.GetColorU32(new Vector4(0.4f, 0.4f, 0.4f, 1));
    for (int x = 0; x <= 网格宽度; x++)
    {
        var 起点 = 网格起始位置 + new Vector2(x * 格子大小, 0);
        var 终点 = 起点 + new Vector2(0, 网格高度像素);
        绘制列表.AddLine(起点, 终点, 网格线颜色);
    }
    for (int y = 0; y <= 网格高度; y++)
    {
        var 起点 = 网格起始位置 + new Vector2(0, y * 格子大小);
        var 终点 = 起点 + new Vector2(网格宽度像素, 0);
        绘制列表.AddLine(起点, 终点, 网格线颜色);
    }
    
    // 获取鼠标位置用于高亮
    Vector2 鼠标位置 = ImGui.GetIO().MousePos;
    if (鼠标位置.X >= 网格起始位置.X && 鼠标位置.Y >= 网格起始位置.Y &&
        鼠标位置.X < 网格起始位置.X + 网格宽度 * 格子大小 &&
        鼠标位置.Y < 网格起始位置.Y + 网格高度 * 格子大小)
    {
        int 悬停X = (int)((鼠标位置.X - 网格起始位置.X) / 格子大小);
        int 悬停Y = (int)((鼠标位置.Y - 网格起始位置.Y) / 格子大小);
        
        if (悬停X >= 0 && 悬停X < 网格宽度 && 悬停Y >= 0 && 悬停Y < 网格高度)
        {
            悬停格子 = (悬停X, 悬停Y);
        }
    }
    else
    {
        悬停格子 = null;
    }
    
    // 先绘制所有格子的背景
    for (int y = 0; y < 网格高度; y++)
    {
        for (int x = 0; x < 网格宽度; x++)
        {
            var 格子位置 = 网格起始位置 + new Vector2(x * 格子大小, y * 格子大小);
            
            // 绘制格子背景
            uint 背景色 = 格子状态数组[x, y] == 格子状态.未翻开 ? 
                ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 1)) : 
                ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1));
            
            // 游戏结束时显示错误标记
            if (游戏结束 && !游戏胜利)
            {
                if (雷区[x, y] && 格子状态数组[x, y] != 格子状态.标记)
                {
                    背景色 = ImGui.GetColorU32(new Vector4(1, 0.2f, 0.2f, 0.5f)); // 红色背景（未标记的雷）
                }
                else if (!雷区[x, y] && 格子状态数组[x, y] == 格子状态.标记)
                {
                    背景色 = ImGui.GetColorU32(new Vector4(1, 0.5f, 0.5f, 0.7f)); // 错误标记背景
                }
            }
            
            绘制列表.AddRectFilled(格子位置 + new Vector2(1, 1), 
                格子位置 + new Vector2(格子大小 - 1, 格子大小 - 1), 背景色);
        }
    }
    
    // 高亮悬停数字格周围的未翻开格子（在背景之上绘制）
    if (悬停格子.HasValue && !游戏结束 && !游戏胜利)
    {
        int 悬停X = 悬停格子.Value.x;
        int 悬停Y = 悬停格子.Value.y;
        
        // 悬停在已翻开的数字格上
        if (格子状态数组[悬停X, 悬停Y] == 格子状态.翻开 && !雷区[悬停X, 悬停Y])
        {
            int 周围雷数 = 计算周围雷数(悬停X, 悬停Y);
            if (周围雷数 > 0)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        int nx = 悬停X + dx;
                        int ny = 悬停Y + dy;
                        
                        if (nx >= 0 && nx < 网格宽度 && ny >= 0 && ny < 网格高度)
                        {
                            // 只高亮未翻开的格子
                            if (格子状态数组[nx, ny] == 格子状态.未翻开)
                            {
                                var 高亮位置 = 网格起始位置 + new Vector2(nx * 格子大小, ny * 格子大小);
                                uint 高亮颜色 = ImGui.GetColorU32(new Vector4(1, 1, 0.5f, 0.3f)); // 半透明黄色
                                绘制列表.AddRectFilled(高亮位置 + new Vector2(1, 1), 
                                    高亮位置 + new Vector2(格子大小 - 1, 格子大小 - 1), 高亮颜色);
                            }
                        }
                    }
                }
            }
        }
    }
    
    // 然后绘制所有格子的内容
    for (int y = 0; y < 网格高度; y++)
    {
        for (int x = 0; x < 网格宽度; x++)
        {
            var 格子位置 = 网格起始位置 + new Vector2(x * 格子大小, y * 格子大小);
            
            // 绘制格子内容
            if (格子状态数组[x, y] == 格子状态.标记)
            {
                // 绘制旗子
                var 旗杆位置 = 格子位置 + new Vector2(格子大小 / 2, 格子大小 / 4);
                var 旗子位置 = 格子位置 + new Vector2(格子大小 / 2 - 5, 格子大小 / 4);
                
                绘制列表.AddLine(旗杆位置, 旗杆位置 + new Vector2(0, 格子大小 / 2), 
                    ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), 2);
                
                绘制列表.AddTriangleFilled(
                    旗子位置,
                    旗子位置 + new Vector2(10, 5),
                    旗子位置 + new Vector2(0, 10),
                    ImGui.GetColorU32(new Vector4(1, 0, 0, 1)));
                
                // 游戏结束时显示错误标记（叉叉）
                if (游戏结束 && !游戏胜利 && !雷区[x, y])
                {
                    var 中心 = 格子位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                    绘制列表.AddLine(中心 - new Vector2(6, 6), 中心 + new Vector2(6, 6), 
                        ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), 2);
                    绘制列表.AddLine(中心 - new Vector2(6, -6), 中心 + new Vector2(6, -6), 
                        ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), 2);
                }
            }
            else if (格子状态数组[x, y] == 格子状态.翻开)
            {
                if (雷区[x, y])
                {
                    // 绘制地雷
                    var 地雷中心 = 格子位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                    
                    // 地雷主体
                    绘制列表.AddCircleFilled(地雷中心, 格子大小 / 3, 
                        ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1)));
                    
                    // 地雷高光
                    绘制列表.AddCircleFilled(地雷中心 - new Vector2(格子大小 / 8, 格子大小 / 8), 
                        格子大小 / 8, ImGui.GetColorU32(new Vector4(1, 1, 1, 0.8f)));
                    
                    // 爆炸效果（如果游戏结束且踩到雷）
                    if (游戏结束 && !游戏胜利)
                    {
                        绘制列表.AddCircle(地雷中心, 格子大小 / 2, 
                            ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), 0, 3);
                    }
                }
                else
                {
                    // 计算周围地雷数
                    int 周围雷数 = 计算周围雷数(x, y);
                    if (周围雷数 > 0)
                    {
                        // 根据雷数设置不同颜色
                        Vector4 数字颜色 = new Vector4(0, 0, 1, 1); // 蓝色
                        if (周围雷数 == 1) 数字颜色 = new Vector4(0, 0, 1, 1); // 蓝色
                        else if (周围雷数 == 2) 数字颜色 = new Vector4(0, 0.6f, 0, 1); // 绿色
                        else if (周围雷数 == 3) 数字颜色 = new Vector4(1, 0, 0, 1); // 红色
                        else if (周围雷数 == 4) 数字颜色 = new Vector4(0, 0, 0.8f, 1); // 深蓝
                        else 数字颜色 = new Vector4(0.8f, 0, 0, 1); // 暗红
                        
                        // 绘制数字
                        var 文字位置 = 格子位置 + new Vector2(格子大小 / 2, 格子大小 / 2);
                        绘制列表.AddText(文字位置 - new Vector2(4, 8), 
                            ImGui.GetColorU32(数字颜色), 周围雷数.ToString());
                    }
                }
            }
        }
    }
    
    // 处理鼠标点击
    处理鼠标点击();
    
    // 设置光标位置以便继续绘制其他内容
    ImGui.SetCursorScreenPos(网格起始位置 + new Vector2(0, 网格高度 * 格子大小 + 5));
}
        
        private void 处理鼠标点击()
        {
            // 游戏已结束或胜利时不处理点击
            if (游戏结束 || 游戏胜利) return;
            
            // 获取鼠标位置
            Vector2 鼠标位置 = ImGui.GetIO().MousePos;
            
            // 检查是否在网格范围内
            if (鼠标位置.X < 网格起始位置.X || 鼠标位置.Y < 网格起始位置.Y ||
                鼠标位置.X >= 网格起始位置.X + 网格宽度 * 格子大小 ||
                鼠标位置.Y >= 网格起始位置.Y + 网格高度 * 格子大小)
            {
                return;
            }
            
            // 计算点击的格子坐标
            int x = (int)((鼠标位置.X - 网格起始位置.X) / 格子大小);
            int y = (int)((鼠标位置.Y - 网格起始位置.Y) / 格子大小);
            
            // 确保坐标在有效范围内
            if (x < 0 || x >= 网格宽度 || y < 0 || y >= 网格高度) return;
            
            // 处理鼠标点击
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                // 优先处理已翻开的数字格（快速翻开周围）
                if (格子状态数组[x, y] == 格子状态.翻开 && !雷区[x, y])
                {
                    快速翻开周围(x, y);
                }
                // 然后是未翻开的格子
                else if (格子状态数组[x, y] == 格子状态.未翻开)
                {
                    翻开格子(x, y);
                }
            }
            else if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                // 右键标记/取消标记
                if (格子状态数组[x, y] == 格子状态.未翻开)
                {
                    格子状态数组[x, y] = 格子状态.标记;
                    剩余雷数--;
                    检查胜利();
                }
                else if (格子状态数组[x, y] == 格子状态.标记)
                {
                    格子状态数组[x, y] = 格子状态.未翻开;
                    剩余雷数++;
                }
            }
        }
        
        private void 快速翻开周围(int x, int y)
        {
            if (游戏结束 || 游戏胜利) return;
            
            int 周围雷数 = 计算周围雷数(x, y);
            int 标记数量 = 0;
            
            // 统计周围标记数量
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    
                    int nx = x + dx;
                    int ny = y + dy;
                    
                    if (nx >= 0 && nx < 网格宽度 && ny >= 0 && ny < 网格高度)
                    {
                        if (格子状态数组[nx, ny] == 格子状态.标记)
                        {
                            标记数量++;
                        }
                    }
                }
            }
            
            // 如果标记数量等于周围雷数，翻开周围未翻开的格子
            if (标记数量 == 周围雷数)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx >= 0 && nx < 网格宽度 && ny >= 0 && ny < 网格高度)
                        {
                            if (格子状态数组[nx, ny] == 格子状态.未翻开)
                            {
                                翻开格子(nx, ny);
                            }
                        }
                    }
                }
            }
        }
        
        private void 翻开格子(int x, int y)
        {
            if (格子状态数组[x, y] != 格子状态.未翻开) return;
            
            // 首次点击保护
            if (首次点击)
            {
                首次点击 = false;
                状态表情 = "😮";
                生成雷区(x, y); // 确保首次点击位置安全
            }
            
            if (雷区[x, y])
            {
                // 踩到地雷
                游戏结束 = true;
                状态表情 = "😵";
                格子状态数组[x, y] = 格子状态.翻开;
                return;
            }
            
            // 翻开当前格子
            格子状态数组[x, y] = 格子状态.翻开;
            
            // 如果周围没有雷，自动翻开相邻格子
            if (计算周围雷数(x, y) == 0)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        int nx = x + dx;
                        int ny = y + dy;
                        
                        if (nx >= 0 && nx < 网格宽度 && ny >= 0 && ny < 网格高度)
                        {
                            翻开格子(nx, ny);
                        }
                    }
                }
            }
            
            // 检查是否胜利
            检查胜利();
        }
        
        private void 生成雷区(int 安全X, int 安全Y)
        {
            // 清空地雷
            for (int y = 0; y < 网格高度; y++)
                for (int x = 0; x < 网格宽度; x++)
                    雷区[x, y] = false;
            
            // 放置地雷（避开安全区域及其周围）
            int 已放置雷数 = 0;
            while (已放置雷数 < 总雷数)
            {
                int x = 随机.Next(0, 网格宽度);
                int y = 随机.Next(0, 网格高度);
                
                // 确保不在安全区域及其周围
                if (Math.Abs(x - 安全X) <= 1 && Math.Abs(y - 安全Y) <= 1) 
                    continue;
                
                if (!雷区[x, y])
                {
                    雷区[x, y] = true;
                    已放置雷数++;
                }
            }
        }
        
        private int 计算周围雷数(int x, int y)
        {
            int 计数 = 0;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    
                    int nx = x + dx;
                    int ny = y + dy;
                    
                    if (nx >= 0 && nx < 网格宽度 && ny >= 0 && ny < 网格高度 && 雷区[nx, ny])
                    {
                        计数++;
                    }
                }
            }
            return 计数;
        }
        
        private void 检查胜利()
        {
            int 未翻开格子数 = 0;
            
            for (int y = 0; y < 网格高度; y++)
            {
                for (int x = 0; x < 网格宽度; x++)
                {
                    if (格子状态数组[x, y] == 格子状态.未翻开 && !雷区[x, y])
                    {
                        未翻开格子数++;
                    }
                }
            }
            
            if (未翻开格子数 == 0)
            {
                游戏胜利 = true;
                状态表情 = "😎";
            }
        }
    }
}