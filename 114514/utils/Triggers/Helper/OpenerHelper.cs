using System.Reflection;
using AEAssist.Helper;

namespace ICEN2.utils.Triggers.Helper;

public class OpenerHelper
{
    // ACR配置映射常量
    private static readonly Dictionary<string, string> AcrSettingsMapping = new Dictionary<string, string>
    {
        { "白魔", "HSS.WhiteMage.Settings.WhiteMageSettings" },
        // 在这里添加其他职业的映射
        // { "黑魔", "HSS.BlackMage.Settings.BlackMageSettings" },
        // { "召唤", "HSS.Summoner.Settings.SummonerSettings" },
        // { "学者", "HSS.Scholar.Settings.ScholarSettings" },
    };

    // ACR BattleData映射常量
    private static readonly Dictionary<string, string> AcrBattleDataMapping = new Dictionary<string, string>
    {
        { "白魔", "HSS.WhiteMage.BattleData.WhiteMageBattleData" },
        // 在这里添加其他职业的映射
        // { "黑魔", "HSS.BlackMage.BattleData.BlackMageBattleData" },
        // { "召唤", "HSS.Summoner.BattleData.SummonerBattleData" },
        // { "学者", "HSS.Scholar.BattleData.ScholarBattleData" },
    };

    /// <summary>
    /// 修改指定ACR的配置值
    /// </summary>
    /// <param name="acrName">ACR名称（如"白魔"）</param>
    /// <param name="propertyName">属性名称</param>
    /// <param name="value">要设置的值</param>
    public void 修改ACR配置(string acrName, string propertyName, object value)
    {
        try
        {
            // 获取配置类型名称
            if (!AcrSettingsMapping.TryGetValue(acrName, out string settingsTypeName))
            {
                LogHelper.Error($"未找到ACR '{acrName}' 的配置映射");
                return;
            }

            // 获取配置类型
            Type settingsType = Type.GetType(settingsTypeName);
            if (settingsType == null)
            {
                LogHelper.Error($"无法找到类型: {settingsTypeName}");
                return;
            }

            // 获取Instance属性
            PropertyInfo instanceProperty = settingsType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty == null)
            {
                LogHelper.Error($"类型 {settingsTypeName} 没有Instance属性");
                return;
            }

            // 获取实例
            object instance = instanceProperty.GetValue(null);
            if (instance == null)
            {
                LogHelper.Error($"类型 {settingsTypeName} 的Instance为null");
                return;
            }

            // 获取要修改的属性
            PropertyInfo targetProperty = settingsType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            FieldInfo targetField = settingsType.GetField(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (targetProperty == null && targetField == null)
            {
                LogHelper.Error($"在类型 {settingsTypeName} 中找不到属性或字段: {propertyName}");
                return;
            }

            // 获取目标类型
            Type targetType = targetProperty != null ? targetProperty.PropertyType : targetField.FieldType;

            // 转换值类型
            object convertedValue = ConvertValue(value, targetType);

            // 设置值
            if (targetProperty != null)
            {
                targetProperty.SetValue(instance, convertedValue);
            }
            else
            {
                targetField.SetValue(instance, convertedValue);
            }

            // 保存配置
            MethodInfo saveMethod = settingsType.GetMethod("Save", BindingFlags.Public | BindingFlags.Instance);
            if (saveMethod != null)
            {
                saveMethod.Invoke(instance, null);
            }

            // 设置BattleData的isChange为true
            SetBattleDataIsChange(acrName);

            LogHelper.Info($"成功修改 {acrName} 的 {propertyName} 为 {value}");
        }
        catch (Exception ex)
        {
            LogHelper.Error($"修改ACR配置时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 批量修改ACR配置
    /// </summary>
    /// <param name="acrName">ACR名称</param>
    /// <param name="settings">配置字典</param>
    public void 批量修改ACR配置(string acrName, Dictionary<string, object> settings)
    {
        foreach (var kvp in settings)
        {
            修改ACR配置(acrName, kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// 获取ACR的所有可修改配置
    /// </summary>
    /// <param name="acrName">ACR名称</param>
    /// <returns>配置信息列表</returns>
    public List<string> 获取ACR可修改配置(string acrName)
    {
        List<string> configList = new List<string>();

        try
        {
            if (!AcrSettingsMapping.TryGetValue(acrName, out string settingsTypeName))
            {
                return configList;
            }

            Type settingsType = Type.GetType(settingsTypeName);
            if (settingsType == null)
            {
                return configList;
            }

            // 获取所有公共实例属性
            PropertyInfo[] properties = settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.CanWrite && property.Name != "Instance")
                {
                    configList.Add($"{property.Name} ({property.PropertyType.Name})");
                }
            }

            // 获取所有公共实例字段
            FieldInfo[] fields = settingsType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.IsInitOnly && !field.IsLiteral)
                {
                    configList.Add($"{field.Name} ({field.FieldType.Name})");
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error($"获取ACR配置列表时出错: {ex.Message}");
        }

        return configList;
    }

    /// <summary>
    /// 设置BattleData的isChange为true
    /// </summary>
    private void SetBattleDataIsChange(string acrName)
    {
        try
        {
            if (!AcrBattleDataMapping.TryGetValue(acrName, out string battleDataTypeName))
            {
                LogHelper.Error($"未找到ACR '{acrName}' 的BattleData映射");
                return;
            }

            Type battleDataType = Type.GetType(battleDataTypeName);
            if (battleDataType == null)
            {
                LogHelper.Error($"无法找到类型: {battleDataTypeName}");
                return;
            }

            // 获取isChange字段
            FieldInfo isChangeField = battleDataType.GetField("isChange", BindingFlags.Public | BindingFlags.Static);
            if (isChangeField == null)
            {
                LogHelper.Error($"类型 {battleDataTypeName} 没有isChange字段");
                return;
            }

            // 设置isChange为true
            isChangeField.SetValue(null, true);
            LogHelper.Info($"已设置 {acrName} 的BattleData.isChange为true");
        }
        catch (Exception ex)
        {
            LogHelper.Error($"设置BattleData.isChange时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 值类型转换辅助方法
    /// </summary>
    private object ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return null;

        // 如果值已经是目标类型，直接返回
        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        // 字符串转换
        if (value is string stringValue)
        {
            // 处理基本类型
            if (targetType == typeof(int))
                return int.Parse(stringValue);
            if (targetType == typeof(float))
                return float.Parse(stringValue);
            if (targetType == typeof(double))
                return double.Parse(stringValue);
            if (targetType == typeof(bool))
                return bool.Parse(stringValue);
            if (targetType == typeof(long))
                return long.Parse(stringValue);

            // 处理枚举
            if (targetType.IsEnum)
                return Enum.Parse(targetType, stringValue);
        }

        // 尝试使用Convert
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            throw new ArgumentException($"无法将 {value} 转换为类型 {targetType.Name}");
        }
    }
    
}