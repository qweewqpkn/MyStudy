using AssetLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BinaryConfig
{
    public interface IBinaryData
    {
        void Init(BinaryConfigRow row);
    }

    class BinaryConfigManager : Singleton<BinaryConfigManager>
    {
        private Dictionary<string, byte[]> mBinaryDataMap = new Dictionary<string, byte[]>();

        public void LoadAllBinaryData(string name, Action callback)
        {
            ResourceManager.Instance.LoadTextAsync(name, name, (data) =>
            {
                if (data != null)
                {
                    CoroutineUtility.Instance.StartCoroutine(ParseAll(data.bytes, callback));
                }
                else
                {
                    Debug.LogError(string.Format("加载二进制合并文件{0}错误", name));
                }
            });
        }

        public IEnumerator ParseAll(byte[] bytes, Action callback)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader br = new BinaryReader(ms);
            int loadNum = 0;
            int restNum = 5; //一帧读5个
            while (ms.Position != ms.Length)
            {
                string name = br.ReadString();
                int length = br.ReadInt32();
                byte[] fileBytes = br.ReadBytes(length);
                mBinaryDataMap[name.ToLower()] = fileBytes;
                loadNum++;
                if (loadNum > restNum)
                {
                    loadNum = 0;
                    yield return null;
                }
            }

            ms.Close();
            br.Close();
            if (callback != null)
            {
                callback();
            }
        }

        public List<T> LoadBinaryData<T>(string name) where T : IBinaryData, new()
        {
            List<T> list = new List<T>();
            name = name.ToLower();
            if(mBinaryDataMap.ContainsKey(name))
            {
                List<BinaryConfigRow> rowList = BinaryConfigParse.Parse(mBinaryDataMap[name]);
                for (int i = 0; i < rowList.Count; i++)
                {
                    T data = new T();
                    data.Init(rowList[i]);
                    list.Add(data);
                }
            }
            else
            {
                Debug.LogError(string.Format("没有{0}对应的二进制数据，请查证！", name));
            }

            return list;
        }
            
    }
}
