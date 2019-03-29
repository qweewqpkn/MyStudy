using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BinaryConfig
{
    public class FiledInfo
    {
        public int mInt;
        public float mFloat;
        public string mString;
        public bool mBool;
        public short mShort;
        public int[] mIntList;
        public float[] mFloatList;
        public string[] mStringList;
        public bool[] mBoolList;
        public Dictionary<int, int> mIntIntDic;
        public Dictionary<int, string> mIntStringDic;
        public Dictionary<string, int> mStringIntDic;
        public Dictionary<string, string> mStringStringDic;

        public int GetInt()
        {
            return mInt;
        }

        public float GetFloat()
        {
            return mFloat;
        }

        public string GetString()
        {
            return mString;
        }

        public bool GetBool()
        {
            return mBool;
        }

        public short GetShort()
        {
            return mShort;
        }

        public int[] GetIntList()
        {
            return mIntList;
        }

        public float[] GetFloatList()
        {
            return mFloatList;
        }

        public string[] GetStringList()
        {
            return mStringList;
        }

        public bool[] GetBoolList()
        {
            return mBoolList;
        }

        public Dictionary<int,int> GetIntIntDic()
        {
            return mIntIntDic;
        }

        public Dictionary<int, string> GetIntStringDic()
        {
            return mIntStringDic;
        }
        public Dictionary<string, int> GetStringIntDic()
        {
            return mStringIntDic;
        }
        public Dictionary<string, string> GetStringStringDic()
        {
            return mStringStringDic;
        }
    }

    public class BinaryConfigRow
    {
        public List<FiledInfo> mFiledInfoList = new List<FiledInfo>();

        public FiledInfo GetFieldInfo(int index)
        {
            if (index < mFiledInfoList.Count)
            {
                return mFiledInfoList[index];
            }
            else
            {
                return null;
            }
        }
    }

    class BinaryConfigParse
    {
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

        public static List<BinaryConfigRow> Parse(byte[] data)
        {
            List<BinaryConfigRow> rowList = new List<BinaryConfigRow>();
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            int rowNum = br.ReadInt32();
            int colNum = br.ReadInt32();

            List<string> nameList = new List<string>();
            for (int i = 0; i < colNum; i++)
            {
                nameList.Add(br.ReadString());
            }

            List<int> typeList = new List<int>();
            for (int i = 0; i < colNum; i++)
            {
                typeList.Add(br.ReadInt32());
            }

            for (int i = 0; i < rowNum; i++)
            {
                BinaryConfigRow row = new BinaryConfigRow();
                rowList.Add(row);
                for (int j = 0; j < colNum; j++)
                {
                    DataType type = (DataType)typeList[j];
                    FiledInfo fi = new FiledInfo();
                    row.mFiledInfoList.Add(fi);
                    switch (type)
                    {
                        case DataType.eInt:
                            {
                                int value = br.ReadInt32();
                                fi.mInt = value;
                            }
                            break;
                        case DataType.eFloat:
                            {
                                float value = br.ReadSingle();
                                fi.mFloat = value;
                            }
                            break;
                        case DataType.eString:
                            {
                                string value = br.ReadString();
                                fi.mString = value;
                            }
                            break;
                        case DataType.eBool:
                            {
                                bool value = br.ReadBoolean();
                                fi.mBool = value;
                            }
                            break;
                        case DataType.eShort:
                            {
                                short value = br.ReadInt16();
                                fi.mShort = value;
                            }
                            break;
                        case DataType.eIntList:
                            {
                                int length = br.ReadInt32();
                                int[] valueList = new int[length];                
                                for(int k = 0; k < length; k++)
                                {
                                    valueList[k] = br.ReadInt32();
                                }
                                fi.mIntList = valueList;
                            }
                            break;
                        case DataType.eFloatList:
                            {
                                int length = br.ReadInt32();
                                float[] valueList = new float[length];
                                for (int k = 0; k < length; k++)
                                {
                                    valueList[k] = br.ReadSingle();
                                }
                                fi.mFloatList = valueList;
                            }
                            break;
                        case DataType.eStringList:
                            {
                                int length = br.ReadInt32();
                                string[] valueList = new string[length];
                                for (int k = 0; k < length; k++)
                                {
                                    valueList[k] = br.ReadString();
                                }
                                fi.mStringList = valueList;
                            }
                            break;
                        case DataType.eBoolList:
                            {
                                int length = br.ReadInt32();
                                bool[] valueList = new bool[length];
                                for (int k = 0; k < length; k++)
                                {
                                    valueList[k] = br.ReadBoolean();
                                }
                                fi.mBoolList = valueList;
                            }
                            break;
                        case DataType.eIntIntDic:
                            {
                                int length = br.ReadInt32();
                                Dictionary<int, int> dic = new Dictionary<int, int>();
                                for(int k = 0; k < length; k++)
                                {
                                    dic[br.ReadInt32()] = br.ReadInt32();
                                }
                                fi.mIntIntDic = dic;
                            }
                            break;
                        case DataType.eIntStringDic:
                            {
                                int length = br.ReadInt32();
                                Dictionary<int, string> dic = new Dictionary<int, string>();
                                for (int k = 0; k < length; k++)
                                {
                                    dic[br.ReadInt32()] = br.ReadString();
                                }
                                fi.mIntStringDic = dic;
                            }
                            break;
                        case DataType.eStringIntDic:
                            {
                                int length = br.ReadInt32();
                                Dictionary<string, int> dic = new Dictionary<string, int>();
                                for (int k = 0; k < length; k++)
                                {
                                    dic[br.ReadString()] = br.ReadInt32();
                                }
                                fi.mStringIntDic = dic;
                            }
                            break;
                        case DataType.eStringStringDic:
                            {
                                int length = br.ReadInt32();
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                for (int k = 0; k < length; k++)
                                {
                                    dic[br.ReadString()] = br.ReadString();
                                }
                                fi.mStringStringDic = dic;
                            }
                            break;
                    }
                }
            }

            ms.Close();
            br.Close();
            return rowList;
        }
    }

}

