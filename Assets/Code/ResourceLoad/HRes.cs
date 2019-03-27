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
        public static Dictionary<HAssetBundle, string> mRemoveMap = new Dictionary<HAssetBundle, string>();


        private Dictionary<string, UnityEngine.Object> AssetMap
        {
            get;
            set;
        }

        private ABRequest ABRequest
        {
            get;
            set;
        }

        private AssetRequest AssetRequest
        {
            get;
            set;
        }

        //最终加载出来的资源对象
        public UnityEngine.Object Asset
        {
            get;
            set;
        }

        //该资源依赖的AB资源
        public HAssetBundle ABDep
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

        public HRes()
        {
            AssetMap = new Dictionary<string, UnityEngine.Object>();
            ABRequest = new ABRequest();
            AssetRequest = new AssetRequest();
        }

        public static string GetResName(string abName, string assetName, AssetType assetType)
        {
            switch(assetType)
            {
                case AssetType.eSprite:
                    {
                        return string.Format("{0}/{1}", abName, "*");
                    }
                default:
                    {
                        return string.IsNullOrEmpty(assetName) ? abName : string.Format("{0}/{1}", abName, assetName);
                    }
            }
        }

        public static T Get<T>(string abName, string assetName, AssetType assetType) where T : HRes, new()
        {
            HRes res = null;
            abName = abName.ToLower();
            assetName = assetName.ToLower();
            string resName = GetResName(abName, assetName, assetType);
            if (!mResMap.TryGetValue(resName, out res))
            {
                res = new T();
                mResMap.Add(resName, res);
                res.Init(abName, assetName, resName);
            }

            res.RefCount++;
            return res as T;
        }

        protected virtual void Init(string abName, string assetName, string resName)
        {
            ABName = abName;
            AssetName = assetName;
            ResName = resName;
        }

        protected virtual void StartLoad(string assetName, bool isSync, bool isAll, Action<UnityEngine.Object> callback)
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(assetName, isSync, isAll, callback));
        }

        protected virtual IEnumerator CoLoad(string assetName, bool isSync, bool isAll, Action<UnityEngine.Object> callback)
        {
            ABDep = Get<HAssetBundle>(ABName, "", AssetType.eAB);

            //加载AB
            ABRequest.Load(ABDep, isSync);
            while(!ABRequest.IsComplete)
            {
                yield return null;
            }

            if(isAll)
            {
                if(AssetMap.Count == 0)
                {
                    //资源还未加载过,那么加载AB中的资源
                    AssetRequest.Load(ABDep.AB, assetName, isSync, isAll);
                    while (!AssetRequest.IsComplete)
                    {
                        yield return null;
                    }

                    //缓存AB中加载的所有资源，为了下次使用
                    if (AssetRequest.AssetList != null)
                    {
                        for (int i = 0; i < AssetRequest.AssetList.Length; i++)
                        {
                            AssetMap[AssetRequest.AssetList[i].name.ToLower()] = AssetRequest.AssetList[i];
                        }
                    }
                }

                if (AssetMap.ContainsKey(assetName))
                {
                    Asset = AssetMap[assetName];
                }
                else
                {
                    Asset = null;
                }
            }
            else
            {
                if(AssetName != assetName || Asset == null)
                {
                    AssetName = assetName;
                    //资源还未加载过,那么加载AB中的资源
                    AssetRequest.Load(ABDep.AB, assetName, isSync, isAll);
                    while (!AssetRequest.IsComplete)
                    {
                        yield return null;
                    }

                    Asset = AssetRequest.Asset;
                }
            }

            OnCompleted(callback);
        }

        protected virtual void OnCompleted(Action<UnityEngine.Object> callback) 
        {
            if(callback != null)
            {
                callback(Asset);
            }
        }

        public void ReleaseAll()
        {
            int count = RefCount;
            for (int i = 0; i < count; i++)
            {
                Release();
            }
        }

        public virtual void AddRef()
        {
            RefCount++;
            //增加该资源对应AB的计数
            if(ABDep != null)
            {
                ABDep.AddRef();
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
            if(ABDep != null)
            {
                  ABDep.Release();
            }
        }
    }
}
