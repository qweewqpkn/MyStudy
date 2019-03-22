using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public class HRes
    {
        public static Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
        protected List<Action<UnityEngine.Object>> mCallBackList = new List<Action<UnityEngine.Object>>();

        //最终加载出来的资源对象
        public UnityEngine.Object AssetObj
        {
            get;
            set;
        }

        //该资源依赖的AB资源
        public HAssetBundle HAB
        {
            get;
            set;
        }

        //AB的名字
        public string ABName
        {
            get;
            set;
        }

        //资源名
        public string AssetName
        {
            get;
            set;
        }

        //资源完整名字
        public string ResName
        {
            get;
            set;
        }

        //该资源加载次数
        public int RefCount
        {
            get;
            set;
        }

        //资源是否加载完成
        public bool IsCompleted
        {
            get;
            set;
        }

        public HRes(){}

        public static string GetResName(string abName, string assetName)
        {
            return string.IsNullOrEmpty(assetName) ? abName : string.Format("{0}/{1}", abName, assetName);
        }

        public static T LoadRes<T>(string abName, string assetName, Action<UnityEngine.Object> callback, params object[] datas) where T : HRes, new()
        {
            HRes res = null;
            string resName = GetResName(abName, assetName);
            if (!mResMap.TryGetValue(resName, out res))
            {
                res = new T();
                mResMap.Add(resName, res);
                res.Init(abName, assetName, resName);
                res.StartLoad(datas);
            }

            if (callback != null)
            {
                res.mCallBackList.Add(callback);
                if (res.IsCompleted)
                {
                    res.OnCompleted(res.AssetObj);
                }
            }

            res.RefCount++;
            return res as T;
        }

        private void Init(string abName, string assetName, string resName)
        {
            ABName = abName;
            AssetName = assetName;
            ResName = resName;
        }

        protected virtual void StartLoad(params object[] datas)
        {
        }

        protected virtual void OnCompleted(UnityEngine.Object obj) 
        {
            IsCompleted = true;
            AssetObj = obj;
        }

        protected virtual void OnCallBack(UnityEngine.Object obj)
        {
            for (int i = 0; i < mCallBackList.Count; i++)
            {
                mCallBackList[i](obj);
            }
            mCallBackList.Clear();
        }

        public void ReleaseAll()
        {
            int count = RefCount;
            for (int i = 0; i < count; i++)
            {
                Release();
            }
        }

        public virtual void Release()
        {
            RefCount--;
            if(RefCount <= 0)
            {
                if(mResMap.ContainsKey(ResName))
                {
                    mResMap.Remove(ResName);
                }
            }

            //释放依赖的AB资源
            if(HAB != null)
            {
                HAB.Release();
            }
        }
    }
}
