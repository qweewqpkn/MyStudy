using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
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
            res.StartLoad(false, false, false, tCallBack);
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
            res.StartLoad(true, false, false, null);
            return res.Asset as AssetBundle;
        }

        protected override IEnumerator CoLoad(bool isSync, bool isAll, bool isPreLoad, Action<UnityEngine.Object> callback)
        {
            ABRequest.Load(this, isSync, AssetType);
            while(!ABRequest.IsComplete)
            {
                yield return null;
            }

            Asset = AB;
            OnCompleted(null, isPreLoad, callback);
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
                    //停止对这个AB的请求
                    ABRequest.StopRequest(ABName);
                    //停止这个对这个AB中资源的请求
                    AssetRequest.StopRequest(ABName);
                    //缓存列表中移除
                    if (mResMap.ContainsKey(ResName))
                    {
                        mResMap.Remove(ResName);
                    }
                    //释放这个AB的所有资源,从这个AB中加载的资源Asset都将变为"null"
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
