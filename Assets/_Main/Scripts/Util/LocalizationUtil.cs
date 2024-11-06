using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace SuperSmashRhodes.Util {
public static class LocalizationUtil {
    private static readonly Dictionary<string, LocalizationTable> tablesCache = new();
    
    public static LocalizationTable GetTable(string key) {
        if (tablesCache.ContainsKey(key)) return tablesCache[key];
        var table = LocalizationSettings.StringDatabase.GetTable(key);
        if (table == null) return null;
        tablesCache[key] = table;
        return table;
    }
    
    public static string GetString(string tableName, string key) {
        var table = GetTable(tableName) as StringTable;
        if (table == null) return null;

        var ret = table.GetEntry(key);
        return ret == null ? key : ret.Value;
    }
}
}
