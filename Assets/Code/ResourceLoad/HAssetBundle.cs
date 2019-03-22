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
        eRelease,
    }

    public class HAssetBundle : HRes
    {
        public static AssetBundleManifest mAssestBundleManifest;
        private List<HAssetBundle> mDepList = new List<HAssetBundle>();
   
        //拥有的AB
        public AssetBundle AB
        {
            get;
            set;
        }
    
        public HAssetBundle()
        {
        }

        private void LoadManifest()
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathManager.URL("Assetbundle", AssetType.eManifest, false));
            HAssetBundle.mAssestBundleManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        }

        public static HAssetBundle Load(string abName, Action<AssetBundle> callback, bool isDep = false)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if(callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as AssetBundle);
                };
            }

            HAssetBundle res = LoadRes<HAssetBundle>(abName, "", tCallBack, isDep) as HAssetBundle;
            return res;
        }

        protected override void StartLoad(params object[] Datas)
        {
            bool isDep = (bool)Datas[0];
            ResourceManager.Instance.StartCoroutine(CoLoad(isDep));
        }

        IEnumerator CoLoad(bool isDep)
        {
            if (HAssetBundle.mAssestBundleManifest == null)
            {
                LoadManifest();
            }

            if (!isDep)
            {
                //加载依赖的AB
                mDepList.Clear();
                string[] depNameList = HAssetBundle.mAssestBundleManifest.GetAllDependencies(ABName);
                for (int i = 0; i < depNameList.Length; i++)
                {
                    mDepList.Add(HAssetBundle.Load(depNameList[i], null, true));
                }

                for (int i = 0; i < mDepList.Count; i++)
                {
                    if (!mDepList[i].IsCompleted)
                    {
                        yield return null;
                    }
                }
            }

            string url = PathManager.URL(ABName, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;

            AB = www.assetBundle;
            OnCompleted(AB);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            AB = obj as AssetBundle;
            //检测是否加载完成时，外部已经标记为释放了
            if (RefCount <= 0)
            {
                mResMap.Remove(ResName);
                if (AB != null)
                {
                    AB.Unload(true);
                }
                OnCallBack(null);
            }
            else
            {
                OnCallBack(AssetObj);
            }
        }

        public override void Release()
        {
            OnRelease();
            for (int i = 0; i < mDepList.Count; i++)
            {
                mDepList[i].OnRelease();
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
                    }
                }
            }
        }
    }
}
