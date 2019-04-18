using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public enum LoadStatus
    {
        eNone,
        eLoading,
        eLoaded,
    }

    public class HAssetBundle : HRes
    {
        private static AssetBundleManifest mAssetBundleManifest;
        public static AssetBundleManifest AssetBundleManifest
        {
            get
            {
                if(mAssetBundleManifest == null)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathManager.URL("Assetbundle", AssetType.eManifest, false));
                    mAssetBundleManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                }

                return mAssetBundleManifest;
            }
        }

        public List<string> DepList
        {
            get;
            set;
        }

        public AssetBundle AB
        {
            get;
            set;
        }

        //加载状态

        public LoadStatus Status
        {
            get;
            set;
        }

        public HAssetBundle()
        {
        }

        protected override void Init(string abName, string assetName, string resName, AssetType assetType)
        {
            base.Init(abName, assetName, resName, assetType);
            if (DepList == null)
            {
                DepList = new List<string>();
                string[] depList = AssetBundleManifest.GetAllDependencies(ABName);
                if (depList != null && depList.Length > 0)
                {
                    DepList.AddRange(depList);
                }
            }
        }

        public static void LoadAsync(string abName, Action<AssetBundle> callback)
        {
            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null);
                }
                return;
            }

            Action<UnityEngine.Object> tCallBack = null;
            if(callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as AssetBundle);
                };
            }

            HAssetBundle res = Get<HAssetBundle>(abName, "", AssetType.eAB);
            res.StartLoad("", false, false, tCallBack);
        }

        //使用协程等待异步请求，而不用回调的形式
        public static AsyncRequest LoadAsync(string abName)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, (obj) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            });

            return request;
        }

        public static AssetBundle Load(string abName)
        {
            if (string.IsNullOrEmpty(abName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HAssetBundle res = Get<HAssetBundle>(abName, "", AssetType.eAB);
            res.StartLoad("", true, false, null);
            return res.Asset as AssetBundle;
        }

        protected override IEnumerator CoLoad(string assetName, bool isSync, bool isAll, Action<UnityEngine.Object> callback)
        {
            ABRequest abRequest = new ABRequest();
            abRequest.Load(this, isSync, AssetType);
            while(!abRequest.IsComplete)
            {
                yield return null;
            }

            Asset = AB;
            OnCompleted(callback);
        }

        public override void AddRef()
        {
            OnAddRef();
            AddDepRef();
        }

        private void OnAddRef()
        {
            RefCount++;
        }

        public void AddDepRef()
        {
            for (int i = 0; i < DepList.Count; i++)
            {
                if (mResMap.ContainsKey(DepList[i]))
                {
                    HAssetBundle res = mResMap[DepList[i]] as HAssetBundle;
                    res.OnAddRef();
                }
            }
        }

        public override void Release()
        {
            //减少自身
            OnRelease();
            //减少依赖
            for (int i = 0; i < DepList.Count; i++)
            {
                if(mResMap.ContainsKey(DepList[i]))
                {
                    HAssetBundle res = mResMap[DepList[i]] as HAssetBundle;
                    res.OnRelease();
                }
            }
        }

        private void OnRelease()
        {
            if (RefCount > 0)
            {
                RefCount--;
                if (RefCount <= 0)
                {
                    if (mResMap.ContainsKey(ResName))
                    {
                        mResMap.Remove(ResName);
                    }

                    if (AB != null)
                    {
                        AB.Unload(true);
                        AB = null;
                    }
                }
            }
        }
    }
}
