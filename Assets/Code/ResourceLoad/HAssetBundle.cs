using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{  
    public class HAssetBundle : HRes
    {
        private static AssetBundleManifest mAssestBundleManifest;
        private List<HAssetBundle> mDepList = new List<HAssetBundle>();

        //ab被引用的次数
        public int RefCount
        {
            get;
            set;
        }
    
        public enum ABLoadStatus
        {
            eNone,
            eLoading,
            eLoaded,
            eLoadError,
            eRelease,
        }
    
        public AssetBundle AB
        {
            get;
            private set;
        }
    
        public ABLoadStatus LoadStatus
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            private set;
        }
    
    
        public HAssetBundle()
        {
        }

        //加载manifest
        private void LoadManifest()
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathManager.URL("Assetbundle", AssetType.eManifest, false));
            AssetBundleManifest mAssestBundleManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        }

        public IEnumerator LoadAB(bool isDep = false)
        {
            if (mAssestBundleManifest == null)
            {
                LoadManifest();
            }

            if (!isDep)
            {
                string[] depList = mAssestBundleManifest.GetAllDependencies(ABName);
                for (int i = 0; i < depList.Length; i++)
                {
                    HAssetBundle depAB = GetRes<HAssetBundle>(depList[i], "", AssetType.eAB);
                    mDepList.Add(depAB);
                }

                for(int i = 0; i < mDepList.Count; i++)
                {
                    while(!mDepList[i].IsCompleted)
                    {
                        yield return null;
                    }
                }
            }

            string url = PathManager.URL(ABName, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;

            RefCount++;
            AB = www.assetBundle;
            LoadStatus = ABLoadStatus.eLoaded;
            IsCompleted = true;
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
