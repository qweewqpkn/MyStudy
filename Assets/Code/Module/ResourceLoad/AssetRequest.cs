using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public class AssetRequest
    {
        public class ABAssetData
        {
            //AB中的资源缓存
            public Dictionary<string, UnityEngine.Object> mAssetMap = new Dictionary<string, UnityEngine.Object>();
            //AB中所有资源的请求
            public AssetBundleRequest mAllAssetRequest;
            //AB中的每个资源对应了一个请求
            public Dictionary<string, AssetBundleRequest> mAssetRequestMap = new Dictionary<string, AssetBundleRequest>();
        }

        //每个AB的资源数据
        static Dictionary<string, ABAssetData> mABAssetDataMap = new Dictionary<string, ABAssetData>();

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

        public AssetRequest()
        {
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
                yield break;
            }
            else
            {
                if(string.IsNullOrEmpty(assetName))
                {
                    IsComplete = true;
                    Debuger.LogError("ASSET_LOAD", "AssetRequest assetName is null");
                    yield break;
                }
                else
                {
                    //获取该AB对应的请求数据
                    ABAssetData cacheData = null;
                    if (!mABAssetDataMap.TryGetValue(ab.name, out cacheData))
                    {
                        cacheData = new ABAssetData();
                        mABAssetDataMap.Add(ab.name, cacheData);
                    }

                    //是否有该资源的缓存
                    if(!cacheData.mAssetMap.ContainsKey(assetName))
                    {
                        if (isSync)
                        {
                            if (isAll)
                            {
                                UnityEngine.Object[] assetList = ab.LoadAllAssets();
                                for (int i = 0; i < assetList.Length; i++)
                                {
                                    cacheData.mAssetMap[assetList[i].name.ToLower()] = assetList[i];
                                }
                            }
                            else
                            {
                                UnityEngine.Object asset = ab.LoadAsset(assetName);
                                cacheData.mAssetMap[asset.name.ToLower()] = asset;
                            }
                        }
                        else
                        {
                            //获取AB中指定资源的请求
                            AssetBundleRequest cacheRequest = null;
                            if (isAll)
                            {
                                cacheRequest = cacheData.mAllAssetRequest;
                            }
                            else
                            {
                                //如果已经存在加载所有资源的请求了,就不需要单独资源的了,因为所有资源请求中就包含了所有资源
                                if(cacheData.mAllAssetRequest != null)
                                {
                                    cacheRequest = cacheData.mAllAssetRequest;
                                }
                                else
                                {
                                    if (cacheData.mAssetRequestMap.ContainsKey(assetName))
                                    {
                                        cacheRequest = cacheData.mAssetRequestMap[assetName];
                                    }
                                }
                            }

                            if (cacheRequest == null)
                            {
                                //AB中没有加载这个资源的请求，那么新启一个
                                if (isAll)
                                {
                                    cacheRequest = ab.LoadAllAssetsAsync();
                                    cacheData.mAllAssetRequest = cacheRequest;
                                    yield return cacheRequest;
                                }
                                else
                                {
                                    cacheRequest = ab.LoadAssetAsync(assetName);
                                    cacheData.mAssetRequestMap.Add(assetName, cacheRequest);
                                    yield return cacheRequest;
                                }
                            }
                            else
                            {
                                //AB中已经有这个资源的对应请求了,那么检测是否还在加载中
                                while (!cacheRequest.isDone)
                                {
                                    yield return null;
                                }
                            }

                            for (int i = 0; i < cacheRequest.allAssets.Length; i++)
                            {
                                cacheData.mAssetMap[cacheRequest.allAssets[i].name.ToLower()] = cacheRequest.allAssets[i];
                            }
                        }
                    }

                    if(cacheData.mAssetMap.ContainsKey(assetName))
                    {
                        Asset = cacheData.mAssetMap[assetName];
                    }
                    else
                    {
                        Debuger.LogError("ASSET_LOAD", string.Format("加载资源失败,请确认该{0}中是否有资源{1}", ab.name, assetName));
                    }

                    IsComplete = true;
                }
            }
        }

        public static void StopAllRequest()
        {
            foreach (var data in mABAssetDataMap)
            {
                StopRequest(data.Value);
            }

            mABAssetDataMap.Clear();
        }

        public static void StopRequest(string abName)
        {
            ABAssetData data = null;
            if (mABAssetDataMap.TryGetValue(abName, out data))
            {
                StopRequest(data);
            }
        }

        private static void StopRequest(ABAssetData data)
        {
            if(data == null)
            {
                return;
            }

            foreach (var item in data.mAssetRequestMap)
            {
                if (!item.Value.isDone)
                {
                    //如果资源还未加载完成，那么访问allAssets属性后，就会将资源加载变成同步加载,之前yield会在同步加载完成后立即返回执行
                    //协程后面的逻辑后,再返回这里执行,有点像goto语句的行为
                    UnityEngine.Object[] assets = item.Value.allAssets;
                }
            }

            if (data.mAllAssetRequest != null && !data.mAllAssetRequest.isDone)
            {
                UnityEngine.Object[] assets = data.mAllAssetRequest.allAssets;
            }
        }
    }
}