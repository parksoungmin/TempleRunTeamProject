using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {

#if UNITY_EDITOR
        //foreach(var id in DataTableIds.String)
        //{ 
        //    var table = new StringTable();
        //    table.Load(id);
        //    tables.Add(id, table);
        //}

        //foreach (var id in ItemTableIds.String)
        //{
        //    var table = new ItemTable();
        //    table.Load(id);
        //    tables.Add(id, table);
        //}
        foreach (var id in UpGradeDataTableIds.String)
        {
            var upgradeTable = new UpGradeDataTable();
            upgradeTable.Load(id);
            tables.Add(id, upgradeTable);
        }
#else
        //var table = new StringTable();
        //var stringTableId = DataTableIds.String[(int)Varibalbes.currentLanguage];
        //table.Load(stringTableId);
        //tables.Add(stringTableId, table);

        //foreach (var id in ItemTableIds.String)
        //{
        //    var itemTable = new ItemTable();
        //    itemTable.Load(id);
        //    tables.Add(id, itemTable);
        //}
        foreach (var id in UpGradeDataTableIds.String)
        {
            var upgradeTable = new UpGradeDataTable();
            upgradeTable.Load(id);
            tables.Add(id, upgradeTable);
        }
#endif
    }

    public static StringTable StringTable
    {
        get
        {
            return Get<StringTable>(DataTableIds.String[(int)Varibalbes.currentLanguage]);
        }
    }
    

    public static UpGradeDataTable UpGradeDataTable
    {
        get
        {
            return Get<UpGradeDataTable>(UpGradeDataTableIds.String[0]);
        }
    }

    //public static ItemTable ItemTable
    //{
    //    get
    //    {
    //        return Get<ItemTable>(ItemTableIds.String[0]);
    //    }
    //}

    public static T Get<T>(string id) where T : DataTable
    {
        if(!tables.ContainsKey(id))
        {
            Debug.LogError("테이블 없음");
            return default(T);
        }

        return tables[id] as T;
    }
}
