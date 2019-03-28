using System.Data;

public enum DataType
{
    eError = -1,
    eInt,
    eFloat,
    eString,
    eBool,
    eShort,
    eIntList,
    eFloatList,
    eStringList,
    eBoolList,
    eIntIntDic,
    eIntStringDic,
    eStringIntDic,
    eStringStringDic,
}

class ExportExcelUtility
{
    public static int GetRealColumns(int columns, DataRow nameRow)
    {
        int realColumns = 0;
        for (int i = 0; i < columns; i++)
        {
            if (!string.IsNullOrEmpty(nameRow[i].ToString()))
            {
                realColumns++;
            }
        }

        return realColumns;
    }

    public static int GetClientColumns(int columns, DataRow csRow)
    {
        int realColumns = 0;
        for (int i = 0; i < columns; i++)
        {
            if (csRow[i].ToString().Contains("c"))
            {
                realColumns++;
            }
        }

        return realColumns;
    }

    public static DataType SwitchTypeToEnumType(string type)
    {
        type = type.ToLower();
        switch (type)
        {
            case "int":
                {
                    return DataType.eInt;
                }
            case "float":
                {
                    return DataType.eFloat;
                }
            case "string":
                {
                    return DataType.eString;
                }
            case "bool":
                {
                    return DataType.eBool;
                }
            case "short":
                {
                    return DataType.eShort;
                }
            case "intarr":
                {
                    return DataType.eIntList;
                }
            case "floatarr":
                {
                    return DataType.eFloatList;
                }
            case "stringarr":
                {
                    return DataType.eStringList;
                }
            case "boolarr":
                {
                    return DataType.eBoolList;
                }
            case "{int:int}":
                {
                    return DataType.eIntIntDic;
                }
            case "{int:string}":
                {
                    return DataType.eIntStringDic;
                }
            case "{string:int}":
                {
                    return DataType.eStringIntDic;
                }
            case "{string:string}":
                {
                    return DataType.eStringStringDic;
                }
        }

        return DataType.eError;
    }
}

