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
        private static Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
        private List<Action<UnityEngine.Object>> mCallBackList = new List<Action<UnityEngine.Object>>();
        private AssetRequest mAssetRequest;

        public UnityEngine.Object AssetObj
        {
            get;
            set;
        }

        public string ABName
        {
            get;
            set;
        }

        public string AssetName
        {
            get;
            set;
        }

        public string ResName
        {
            get;
            set;
        }

        //该资源加载次数
        public int LoadCount
        {
            get;
            set;
        }

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

        public static T LoadRes<T>(string abName, string assetName, Action<UnityEngine.Object> callback) where T : HRes, new()
        {
            HRes res = null;
            string resName = GetResName(abName, assetName);
            if (!mResMap.TryGetValue(resName, out res))
            {
                res = new T();
                mResMap.Add(resName, res);
                res.Init(abName, assetName, resName);
            }

            if (callback != null)
            {
                if(res.IsCompleted)
                {
                    callback(res.AssetObj);
                }
                else
                {
                    res.mCallBackList.Add(callback);
                }
            }

            res.LoadCount++;
            return res as T;
        }

        protected virtual void Init(string abName, string assetName, string resName)
        {
            ABName = abName;
            AssetName = assetName;
            ResName = resName;
        }

        protected virtual IEnumerator CoLoad(AssetBundle ab, string abName, string assetName)
        {
            AssetRequest assetRequest = new AssetRequest();
            yield return assetRequest.Load(ab, assetName);
            OnCompleted(assetRequest.AssetObj);
        }

        protected virtual void OnCompleted(UnityEngine.Object obj) 
        {
            IsCompleted = true;
            AssetObj = obj;
        }

        protected void OnCallBack(UnityEngine.Object obj)
        {
            for(int i = 0; i < mCallBackList.Count; i++)
            {
                mCallBackList[i](obj);
            }
            mCallBackList.Clear();
        }

        public void ReleaseAll()
        {
            int count = LoadCount;
            for (int i = 0; i < count; i++)
            {
                Release();
            }
        }

        public virtual void Release()
        {
            //if(LoadCount <= 0)
            //{
            //    return;
            //}
            //
            //LoadCount--;
            //if(LoadCount == 0)
            //{
            //    //todo，bug:要修复这里移除了AB，但是没有释放AB，ab.AB.Unload(true);考虑一下如何修复
            //    if (ResourceManager.Instance.mResMap.ContainsKey(ResName))
            //    {
            //        ResourceManager.Instance.mResMap.Remove(ResName);
            //    }
            //}
            //
            ////该资源每次引用对应的ab依赖都要进行释放
            //for (int j = 0; j < mAllABList.Count; j++)
            //{
            //    if (ResourceManager.Instance.mResMap.ContainsKey(mAllABList[j]))
            //    {
            //        HAssetBundle ab = ResourceManager.Instance.mResMap[mAllABList[j]] as HAssetBundle;
            //        ab.LoadStatus = HAssetBundle.ABLoadStatus.eRelease; //标记AB为待释放,因为可能释放资源的时候，它引用的ab还在加载中..
            //        if (ab != null)
            //        {
            //            if(ab.RefCount > 0)
            //            {
            //                ab.RefCount--;
            //            }
            //            
            //            if (ab.RefCount == 0)
            //            {
            //                if (ResourceManager.Instance.mResMap.ContainsKey(ab.ResName))
            //                {
            //                    ResourceManager.Instance.mResMap.Remove(ab.ResName);
            //                }
            //
            //                if (ab.AB != null)
            //                {
            //                    ab.AB.Unload(true);
            //                }
            //                else
            //                {
            //                    Debug.LogError("HRes Release ab is null, please check!");
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
