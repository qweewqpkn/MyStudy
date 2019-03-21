using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public enum ABLoadStatus
    {
        eNone,
        eLoading,
        eLoaded,
        eLoadError,
        eRelease,
    }

    public class HAssetBundle : HRes
    {
        public static AssetBundleManifest mAssestBundleManifest;
        private List<HAssetBundle> mDepList = new List<HAssetBundle>();

        //ab被引用的次数
        public int RefCount
        {
            get;
            set;
        }
    
        public ABLoadStatus LoadStatus
        {
            get;
            set;
        }
    
        public HAssetBundle()
        {
        }

        public static HAssetBundle Load(string abName, Action<AssetBundle> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if(callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as AssetBundle);
                };
            }

            return LoadRes<HAssetBundle>(abName, "", tCallBack);
        }

        protected override void Init(string abName, string assetName, string resName)
        {
            base.Init(abName, assetName, resName);
            ResourceManager.Instance.StartCoroutine(CoLoad(null, abName, assetName));
        }

        protected override IEnumerator CoLoad(AssetBundle ab, string abName, string assetName)
        {
            ABRequest abRequest = new ABRequest();
            yield return abRequest.Load(abName);
            OnCompleted(abRequest.AB);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            LoadStatus = ABLoadStatus.eLoaded;
            OnCallBack(AssetObj);
        }

        public override void Release()
        {
            base.Release();
        }
    }
}
