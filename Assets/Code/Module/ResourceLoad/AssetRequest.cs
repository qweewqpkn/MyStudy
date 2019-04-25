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

        public bool IsLoading
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
            IsLoading = false;
        }

        public void Load(HRes res, bool isSync, bool isAll = false)
        {
            string assetName = res.AssetName;
            if(IsLoading)
            {
                return;
            }

            if(isAll)
            {        
                if(AssetList != null)
                {
                    if (res.AssetMap.ContainsKey(assetName))
                    {
                        res.Asset = res.AssetMap[assetName];
                        return;
                    }
                }
                else
                {
                    HRes shareRes = null;
                    if(HRes.mShareResMap.TryGetValue(res.ABName, out shareRes))
                    {
                        if (shareRes.AssetMap.ContainsKey(assetName))
                        {
                            res.Asset = res.AssetMap[assetName];
                            return;
                        }
                    }
                }
            }
            else
            {
                if(Asset != null)
                {
                    res.Asset = Asset;
                    return;
                }
            }

            ResourceManager.Instance.StartCoroutine(CoLoad(res, assetName, isSync, isAll));
        }

        public IEnumerator CoLoad(HRes res, string assetName, bool isSync, bool isAll = false)
        {
            AssetBundle ab = res.ABDep.AB;
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
                    IsLoading = true;
                    if (isSync)
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

                    if(isAll)
                    {
                        if (AssetList != null)
                        {
                            for (int i = 0; i < AssetList.Length; i++)
                            {
                                res.AssetMap[AssetList[i].name.ToLower()] = AssetList[i];
                            }

                            if (res.AssetMap.ContainsKey(assetName))
                            {
                                res.Asset = res.AssetMap[assetName];
                            }

                            HRes.mShareResMap[res.ABName] = res;
                        }
                    }
                    else
                    {
                        res.Asset = Asset;
                    }

                    IsLoading = false;
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