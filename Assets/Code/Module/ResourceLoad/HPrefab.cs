
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        //实例obj
        public GameObject InstObj
        {
            get;
            set;
        }

        public HPrefab()
        {
        }

        //同步请求prefab
        public static GameObject Load(string abName, string assetName, bool isPreLoad)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(true, false, isPreLoad, null);
            if(isPreLoad)
            {
                return res.AssetData.mAsset as GameObject;
            }
            else
            {
                return res.InstObj;
            }
        }

        //异步请求prefab
        public static void LoadAsync(string abName, string assetName, bool isPreLoad, Action<GameObject, object[]> callback, params object[] args)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null, null);
                }
                return;
            }

            Action<AssetLoadData> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (data) =>
                {
                    if(data != null)
                    {
                        callback(data.mAsset as GameObject, args);
                    }
                    else
                    {
                        callback(null, args);
                    }
                };
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(false, false, isPreLoad, tCallBack);
        }

        //协程请求prefab
        public static AsyncRequest LoadCoRequest(string abName, string assetName, bool isPreLoad)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, assetName, isPreLoad, (obj, args) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            });

            return request;
        }

        protected override void OnCompleted(AssetRequest request, bool isPreLoad, Action<AssetLoadData> callback)
        {
            if (request.Asset == null && request.Assets == null)
            {
                Debuger.LogError("ASSET_LOAD", string.Format("Load Res Error, ABName {0}, AssetName {1}", ABName, AssetName));
            }

            AssetData.mAsset = request.Asset;
            if (AssetData.mAsset != null)
            {
                if (isPreLoad)
                {
                    if (callback != null)
                    {
                        callback(AssetData);
                    }
                }
                else
                {
                    InstObj = GameObject.Instantiate(AssetData.mAsset as GameObject);
                    PrefabAutoDestroy autoDestroy = InstObj.AddComponent<PrefabAutoDestroy>();
                    autoDestroy.mRes = this;
                    AssetData.mAsset = InstObj;
                    if (callback != null)
                    {
                        callback(AssetData);
                    }
                }
            }
            else
            {
                if(callback != null)
                {
                    callback(AssetData);
                }
            }
        }
    }
    
}
