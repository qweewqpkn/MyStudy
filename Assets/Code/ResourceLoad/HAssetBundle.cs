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

        public static void Load(string abName, Action<AssetBundle> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if(callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as AssetBundle);
                };
            }

            HAssetBundle res = Get<HAssetBundle>(abName, "", tCallBack) as HAssetBundle;
            res.StartLoad();
        }

        protected override void Init(string abName, string assetName, string resName)
        {
            base.Init(abName, assetName, resName);
            DepList = new List<string>();
            string[] depList = AssetBundleManifest.GetAllDependencies(ABName);
            if (depList != null && depList.Length > 0)
            {
                DepList.AddRange(depList);
            }
        }

        protected override IEnumerator CoLoad()
        {
            ABRequest abRequest = new ABRequest();
            yield return abRequest.Load(this);
            OnCompleted(AB);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
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
                        HAssetBundle hab = mResMap[ResName] as HAssetBundle;
                        //如果还在加载中，那么加入移除列表，等待加载完成后移除资源
                        if(hab.Status == LoadStatus.eLoading)
                        {
                            mRemoveMap.Add(hab, hab.ResName);
                        }
                        mResMap.Remove(ResName);
                    }

                    if (AB != null)
                    {
                        AB.Unload(true);
                    }
                }
            }
        }
    }
}
