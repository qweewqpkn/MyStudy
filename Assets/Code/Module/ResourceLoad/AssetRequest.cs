using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public class AssetRequest
    {
        static List<AssetBundleRequest> mRequestList= new List<AssetBundleRequest>();

        public bool IsComplete
        {
            get;
            private set;
        }

        public UnityEngine.Object Asset
        {
            get;
            private set;
        }

        public UnityEngine.Object[] AssetList
        {
            get;
            set;
        }

        public AssetRequest()
        {
            Asset = null;
            AssetList = null;
            IsComplete = false;
        }

        public void Load(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(ab, assetName, isSync, isAll));
        }

        public IEnumerator CoLoad(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            if (ab == null)
            {
                IsComplete = true;
            }
            else
            {
                if(string.IsNullOrEmpty(assetName))
                {
                    IsComplete = true;
                    Debug.LogError("AssetRequest assetName is null");
                    yield break;
                }
                else
                {
                    if(isSync)
                    {
                        if(isAll)
                        {
                            AssetList = ab.LoadAllAssets();
                        }
                        else
                        {
                            Asset = ab.LoadAsset(assetName);
                        }
                    }
                    else
                    {
                        if (isAll)
                        {
                            AssetBundleRequest request = ab.LoadAllAssetsAsync();
                            mRequestList.Add(request);
                            yield return request;
                            mRequestList.Remove(request);
                            AssetList = request.allAssets;
                        }
                        else
                        {
                            AssetBundleRequest request = ab.LoadAssetAsync(assetName);
                            mRequestList.Add(request);
                            yield return request;
                            mRequestList.Remove(request);
                            Asset = request.asset;
                        }
                    }

                    IsComplete = true;
                }
            }
        }

        public static void StopAllRequest()
        {
            List<AssetBundleRequest> requestList = new List<AssetBundleRequest>();
            for (int i = 0; i < mRequestList.Count; i++)
            {
                requestList.Add(mRequestList[i]);
            }

            //访问AssetBundleRequest 异步请求的allAssets会导致该ab的加载立马返回，相当于变为同步加载
            for (int i = 0; i < requestList.Count; i++)
            {
                UnityEngine.Object[] assets = requestList[i].allAssets;
            }

            mRequestList.Clear();
        }
    }
}