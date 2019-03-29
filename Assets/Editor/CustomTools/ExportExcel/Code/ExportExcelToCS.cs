using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

class ExportExcelToCS
{
    //excel的路径
    private static string mExcelPath;
    //输出的二进制路径
    private static string mBinaryPath;
    //输出的代码的路径
    private static string mCodePath;

    private static BinaryWriter mMergeBinaryWriter;


    [MenuItem("Tools/Excel Export/Excel导出为CS")]
    public static void Export()
    {
        mCodePath = "E:/Test/Unity/MyStudy/Assets/Code/BinaryConfigRead/ExportCode/";
        mExcelPath = "E:/Test/Unity/MyStudy/Assets/Editor/CustomTools/ExportExcel/Excel/";
        mBinaryPath = "E:/Test/Unity/MyStudy/Assets/Export/Config/";

        //合并的配置
        FileStream fs = File.Open(mBinaryPath + "Config.bytes", FileMode.Create, FileAccess.ReadWrite);
        mMergeBinaryWriter = new BinaryWriter(fs);

        //所有excel配置
        string[] allExcelPath = Directory.GetFiles(mExcelPath);
        for (int i = 0; i < allExcelPath.Length; i++)
        {
            string extensionName = Path.GetExtension(allExcelPath[i]);
            if (extensionName == ".xlsx")
            {
                FileStream excelFS = File.Open(allExcelPath[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(excelFS);
                DataSet book = reader.AsDataSet();
                ExportCode(book.Tables[0], allExcelPath[i]);
                byte[] bytes = ExportByte(book.Tables[0], allExcelPath[i]);
                MergeByteToOne(bytes, allExcelPath[i]);
                excelFS.Close();
            }
        }

        mMergeBinaryWriter.Close();
        AssetDatabase.Refresh();
        Debug.Log("导出成功!");
    }

    static void MergeByteToOne(byte[] bytes, string path)
    {
        string name = Path.GetFileNameWithoutExtension(path);
        mMergeBinaryWriter.Write(name);
        mMergeBinaryWriter.Write(bytes.Length);
        mMergeBinaryWriter.Write(bytes);
    }

    static byte[] ExportByte(DataTable sheet, string path)
    {
        string outputPath = mBinaryPath + Path.GetFileNameWithoutExtension(path);
        //FileStream fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite);
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);

        DataRow nameRow = sheet.Rows[0];
        DataRow commentRow = sheet.Rows[1];
        DataRow typeRow = sheet.Rows[2];
        DataRow csRow = sheet.Rows[3];
        int rowNum = sheet.Rows.Count;
        int columnNum = ExportExcelUtility.GetRealColumns(sheet.Columns.Count, nameRow);

        //写入行数   
        bw.Write(rowNum - 4);
        //写入列数
        bw.Write(ExportExcelUtility.GetClientColumns(sheet.Columns.Count, csRow));
        //写入名字
        for (int i = 0; i < columnNum; i++)
        {
            if (csRow[i].ToString().Contains("c"))
                bw.Write(nameRow[i].ToString());
        }
        //写入类型
        for (int i = 0; i < columnNum; i++)
        {
            if (csRow[i].ToString().Contains("c"))
                bw.Write((int)ExportExcelUtility.SwitchTypeToEnumType(typeRow[i].ToString()));
        }
        //写入剩余数据
        for (int i = 4; i < rowNum; i++)
        {
            DataRow dataRow = sheet.Rows[i];
            for (int j = 0; j < columnNum; j++)
            {
                if (csRow[j].ToString().Contains("c"))
                {
                    string strValue = dataRow[j].ToString();
                    DataType type = ExportExcelUtility.SwitchTypeToEnumType(typeRow[j].ToString());
                    switch (type)
                    {
                        case DataType.eInt:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    int value = 0;
                                    bw.Write(value);
                                }
                                else
                                {

                                    int value = int.Parse(strValue);
                                    bw.Write(value);
                                }
                            }
                            break;
                        case DataType.eFloat:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    float value = 0;
                                    bw.Write(value);
                                }
                                else
                                {
                                    float value = float.Parse(strValue);
                                    bw.Write(value);
                                }
                            }
                            break;
                        case DataType.eString:
                            {
                                bw.Write(strValue);
                            }
                            break;
                        case DataType.eBool:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bool value = false;
                                    bw.Write(value);
                                }
                                else
                                {
                                    bool value = false;
                                    if (strValue == "false")
                                    {
                                        value = false;
                                    }
                                    else if (strValue == "true")
                                    {
                                        value = true;
                                    }
                                    else
                                    {
                                        Debug.LogError(string.Format("{0},{1}的bool 值填写错误", i, j));
                                        continue;
                                    }

                                    bw.Write(value);
                                }
                            }
                            break;
                        case DataType.eShort:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    short value = 0;
                                    bw.Write(value);
                                }
                                else
                                {

                                    short value = short.Parse(strValue);
                                    bw.Write(value);
                                }
                            }
                            break;
                        case DataType.eIntList:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        int value = int.Parse(list[k]);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eFloatList:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        float value = float.Parse(list[k]);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eStringList:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        bw.Write(list[k]);
                                    }
                                }
                            }
                            break;
                        case DataType.eBoolList:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        bool value = false;
                                        if (list[k] == "true")
                                        {
                                            value = true;
                                        }
                                        else if (list[k] == "false")
                                        {
                                            value = false;
                                        }
                                        else
                                        {
                                            Console.WriteLine("bool list value error");
                                        }

                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eIntIntDic:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        string[] pairs = list[k].Split(':');
                                        int key = int.Parse(pairs[0]);
                                        int value = int.Parse(pairs[1]);
                                        bw.Write(key);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eIntStringDic:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        string[] pairs = list[k].Split(':');
                                        int key = int.Parse(pairs[0]);
                                        string value = pairs[1];
                                        bw.Write(key);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eStringIntDic:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        string[] pairs = list[k].Split(':');
                                        string key = pairs[0];
                                        int value = int.Parse(pairs[1]);
                                        bw.Write(key);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                        case DataType.eStringStringDic:
                            {
                                if (string.IsNullOrEmpty(strValue))
                                {
                                    bw.Write(0);
                                }
                                else
                                {
                                    string[] list = strValue.Split(',');
                                    bw.Write(list.Length);
                                    for (int k = 0; k < list.Length; k++)
                                    {
                                        string[] pairs = list[k].Split(':');
                                        string key = pairs[0];
                                        string value = pairs[1];
                                        bw.Write(key);
                                        bw.Write(value);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        bw.Flush();
        ms.Flush();

        return ms.ToArray();
    }

    static void ExportCode(DataTable sheet, string path)
    {
        string fileName = Path.GetFileNameWithoutExtension(path);
        string outputPath = mCodePath + fileName + ".cs";
        FileStream fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(fs);

        DataRow nameRow = sheet.Rows[0];
        DataRow commentRow = sheet.Rows[1];
        DataRow typeRow = sheet.Rows[2];
        DataRow csRow = sheet.Rows[3];
        int rowNum = sheet.Rows.Count;
        int columnNum = ExportExcelUtility.GetRealColumns(sheet.Columns.Count, nameRow);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("namespace BinaryConfig");
        sb.AppendLine("{");
        sb.AppendFormat("    class {0} : IBinaryData\r\n", fileName);
        sb.AppendLine("    {");

        for (int i = 0; i < columnNum; i++)
        {
            if (csRow[i].ToString().Contains("c"))
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendFormat("        /// {0}\r\n", System.Text.RegularExpressions.Regex.Replace(commentRow[i].ToString(), "[\r\n\t]", ""));
                sb.AppendLine("        /// <summary>");
                sb.AppendFormat("        private {0} m{1};\r\n", SwitchTypeToRealType(typeRow[i].ToString()), nameRow[i]);
                sb.AppendFormat("        public {0} {1}\r\n", SwitchTypeToRealType(typeRow[i].ToString()), nameRow[i]);
                sb.AppendLine("        {");
                sb.AppendLine("            get" + "{" + " return m" + nameRow[i] + ";}");
                sb.AppendLine("        }");
            }
        }

        sb.AppendLine("        public void Init(BinaryConfigRow row)");
        sb.AppendLine("        {");
        int index = 0;
        for (int i = 0; i < columnNum; i++)
        {
            if (csRow[i].ToString().Contains("c"))
            {
                sb.AppendFormat("            m{0} = row.GetFieldInfo({1}).{2}();\r\n", nameRow[i], index, SwitchTypeToFunName(typeRow[i].ToString()));
                index++;
            }
        }
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}\r\n");
        sw.Write(sb);

        sw.Close();
        fs.Close();
    }

    static string SwitchTypeToFunName(string type)
    {
        type = type.ToLower();
        switch (type)
        {
            case "int":
                {
                    return "GetInt";
                }
            case "float":
                {
                    return "GetFloat";
                }
            case "string":
                {
                    return "GetString";
                }
            case "bool":
                {
                    return "GetBool";
                }
            case "short":
                {
                    return "GetShort";
                }
            case "intarr":
                {
                    return "GetIntList";
                }
            case "floatarr":
                {
                    return "GetFloatList";
                }
            case "stringarr":
                {
                    return "GetStringList";
                }
            case "boolarr":
                {
                    return "GetBoolList";
                }
            case "{int:int}":
                {
                    return "GetIntIntDic";
                }
            case "{int:string}":
                {
                    return "GetIntStringDic";
                }
            case "{string:int}":
                {
                    return "GetStringIntDic";
                }
            case "{string:string}":
                {
                    return "GetStringStringDic";
                }
        }

        return "";
    }

    static string SwitchTypeToRealType(string type)
    {
        type = type.ToLower();
        switch (type)
        {
            case "int":
                {
                    return "int";
                }
            case "float":
                {
                    return "float";
                }
            case "string":
                {
                    return "string";
                }
            case "short":
                {
                    return "short";
                }
            case "bool":
                {
                    return "bool";
                }
            case "intarr":
                {
                    return "int[]";
                }
            case "floatarr":
                {
                    return "float[]";
                }
            case "stringarr":
                {
                    return "string[]";
                }
            case "boolarr":
                {
                    return "bool[]";
                }
            case "{int:int}":
                {
                    return "Dictionary<int,int>";
                }
            case "{int:string}":
                {
                    return "Dictionary<int,string>";
                }
            case "{string:int}":
                {
                    return "Dictionary<string,int>";
                }
            case "{string:string}":
                {
                    return "Dictionary<string,string>";
                }
        }

        return "";
    }
}

