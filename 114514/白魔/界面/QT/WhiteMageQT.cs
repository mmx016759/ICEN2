using ICEN2.白魔.设置;
using ICEN2.白魔.设置.设置;

// ReSharper disable UnusedMember.Global

namespace ICEN2.白魔.界面.QT;

public static class 白魔Qt
    {
        public static bool GetQt(string qtName)
        {
            return baseUI.QT.GetQt(qtName);
        }

        public static bool ReverseQt(string qtName)
        {
            return baseUI.QT.ReverseQt(qtName);
        }

        public static void SetQt(string qtName, bool qtValue)
        {
            baseUI.QT.SetQt(qtName, qtValue);
        }

        public static void Reset()
        {
            defaultQtSetting(GetQt("高难模式") ? 1 : 0);
        }

        public static void NewDefault(string qtName, bool newDefault)
        {
            baseUI.QT.NewDefault(qtName, newDefault);
        }

        public static void SetDefaultFromNow()
        {
            baseUI.QT.SetDefaultFromNow();
        }

        public static string[] GetQtArray()
        {
            return baseUI.QT.GetQtArray();
        }

        public static void CreateQt()
        {
            baseUI.QT.AddQt("高难模式", false, "未实现");
            baseUI.QT.AddQt("停手", false,"仅控制输出项");
            baseUI.QT.AddQt("AOE", true, "管控所有aoe伤害");
            baseUI.QT.AddQt("Dot", true, "默认给boss上dot，小怪不上dot");
            baseUI.QT.AddQt("读条奶", true, "管理是否读条奶人");
            baseUI.QT.AddQt("单奶", true, "管控所有单奶");
            baseUI.QT.AddQt("群奶", true, "管控所有群奶");
            baseUI.QT.AddQt("减伤", true, "管控所有庇护所和节制和水流幕");
            baseUI.QT.AddQt("再生", true);
            baseUI.QT.AddQt("天赐", true);
            baseUI.QT.AddQt("法令", true);
            baseUI.QT.AddQt("神名", true);
            baseUI.QT.AddQt("红花", true);
            baseUI.QT.AddQt("铃铛", true);
            baseUI.QT.AddQt("醒梦", true);
            baseUI.QT.AddQt("停手卸蓝花", true, "停手时是否卸完蓝花");
            baseUI.QT.AddQt("狂喜之心", true);
            baseUI.QT.AddQt("安慰之心", true);
            baseUI.QT.AddQt("神祝祷", true);
            baseUI.QT.AddQt("水流幕", true);
            baseUI.QT.AddQt("全大赦", true);
            baseUI.QT.AddQt("康复", true);
            baseUI.QT.AddQt("拉人", true);
            baseUI.QT.AddQt("神速咏唱", true);
            默认值.实例.JobViewSave.QtUnVisibleList.Clear();
            // WhiteMageSettings.Instance.JobViewSave.QtUnVisibleList.Add("4人本再生");
            
            
            //baseUI.QT.SetUpdateAction(AOnUpdate);
            
            if (默认值.实例.JobViewSave.QtUnVisibleList.Count == 0)
            {
                默认值.实例.JobViewSave.QtUnVisibleList.Clear();
                默认值.实例.JobViewSave.QtUnVisibleList.Add("高难模式");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("铃铛");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("狂喜之心");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("安慰之心");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("神名");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("醒梦");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("神祝祷");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("全大赦");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("天赐");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("再生");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("水流幕");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("读条奶");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("减伤");
                默认值.实例.JobViewSave.QtUnVisibleList.Add("停手卸蓝花");
                默认值.实例.Save();
                
            }
        }
        
        private static void defaultQtSetting(int value)
        {
            switch (value)
            {
                case 1:

                    break;
                case 0:

                    break;
                default:

                    break;
            }
        }

        private static void AOnUpdate()
        {
            if (默认值.实例.HLBU)
            {
                SettingsButtonWindow.Draw();

                设置界面.Draw();
            }

        }

    }