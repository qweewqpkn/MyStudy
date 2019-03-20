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
    
        public AssetBundle AB
        {
            get;
            set;
        }
    
        public ABLoadStatus LoadStatus
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }
    
    
        public HAssetBundle()
        {
        }
    
        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            if (ab != null)
            {
                if (success != null)
                {
                    success(ab as T);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }

            yield return null;
        }
    
        public override void Release()
        {
            base.Release();
            AB = null;
        }
    }
}
