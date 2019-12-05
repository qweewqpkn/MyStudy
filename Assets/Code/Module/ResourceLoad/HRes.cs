using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public class AssetLoadData
    {
        public UnityEngine.Object mAsset;
        public List<UnityEngine.Object> mAssets;
    }


    public class HRes
    {
        public static Dictionary<string, HRes> mResMap = new Dictionary<string, HRes>();
        private static List<string> mActiveVariantNameList = new List<string>();

        protected ABRequest ABRequest
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
        //public UnityEngine.Object Asset
        //{
        //    get;
        //    set;
        //}

        public AssetLoadData AssetData
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

        //资源类型
        public AssetType AssetType
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
            ABRequest = new ABRequest();
            AssetRequest = new AssetRequest();
            AssetData = new AssetLoadData();
        }

        public static void ActivateVariantName(string variantName)
        {
            mActiveVariantNameList.Add(variantName);
        }

        public static void RemoveVariantName(string variantName)
        {
            mActiveVariantNameList.Remove(variantName);
        }

        public static string RemapVariantName(string abName, bool isDep)
        {
            if(mActiveVariantNameList.Count == 0 || isDep)
            {
                return abName;
            }
            else
            {
                string[] split = abName.Split('.');
                string[] variantList = HAssetBundle.AssetBundleManifest.GetAllAssetBundlesWithVariant();
                for(int i = 0; i < variantList.Length; i++)
                {
                    string[] curSplit = variantList[i].Split('.');
                    if (curSplit[0] != split[0])
                        continue;

                    int found = mActiveVariantNameList.FindIndex((item) => { return item == curSplit[1]; });
                    if(found != -1)
                    {
                        return variantList[i];
                    }
                }

                return abName;
            }
        }

        public static string GetResName(string abName, string assetName)
        {
            return string.IsNullOrEmpty(assetName) ? abName : string.Format("{0}/{1}", abName, assetName);
        }

        public static T Get<T>(string abName, string assetName, AssetType assetType, bool isDep = false) where T : HRes, new()
        {
            HRes res = null;
            abName = RemapVariantName(abName.ToLower(), isDep);
            assetName = assetName.ToLower();
            string resName = GetResName(abName, assetName);
            if (!mResMap.TryGetValue(resName, out res))
            {
                res = new T();
                mResMap.Add(resName, res);
                res.Init(abName, assetName, resName, assetType);
            }

            res.RefCount++;
            return res as T;
        }

        protected virtual void Init(string abName, string assetName, string resName, AssetType assetType)
        {
            ABName = abName;
            AssetName = assetName;
            ResName = resName;
            AssetType = assetType;
        }

        protected virtual void StartLoad(bool isSync, bool isAll, bool isPreLoad, Action<AssetLoadData> callback)
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(isSync, isAll, isPreLoad, callback));
        }

        protected virtual IEnumerator CoLoad(bool isSync, bool isAll, bool isPreLoad, Action<AssetLoadData> callback)
        {
            ABDep = Get<HAssetBundle>(ABName, "", AssetType.eAB);

            //加载AB
            ABRequest.Load(ABDep, isSync, AssetType);
            while(!ABRequest.IsComplete)
            {
                yield return null;
            }

            //加载Asset
            AssetRequest.Load(ABDep.AB, AssetName, isSync, isAll);
            while (!AssetRequest.IsComplete)
            {
                yield return null;
            }

            //回调
            OnCompleted(AssetRequest, isPreLoad, callback);
        }

        protected virtual void OnCompleted(AssetRequest request, bool isPreLoad, Action<AssetLoadData> callback) 
        {
            if(request.Asset == null && request.Assets == null)
            {
                Debuger.LogError("ASSET_LOAD", string.Format("Load Res Error, ABName {0}, AssetName {1}", ABName, AssetName));
            }

            AssetData.mAsset = request.Asset;
            AssetData.mAssets = request.Assets;
            if (callback != null)
            {
                callback(AssetData);
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
