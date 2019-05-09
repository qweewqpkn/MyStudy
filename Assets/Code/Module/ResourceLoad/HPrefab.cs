
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        //原始prefab
        public GameObject Prefab
        {
            get;
            set;
        }

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
                return res.Prefab;
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

            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as GameObject, args);
                };
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(false, false, isPreLoad, tCallBack);
        }

        //协程请求prefab
        public static AsyncRequest LoadCoRequest(string abName, string assetName, bool isPreLoad, params object[] args)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, assetName, isPreLoad, (obj, datas) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            }, args);

            return request;
        }

        protected override void OnCompleted(bool isPreLoad, Action<UnityEngine.Object> callback)
        {
            if (Asset != null)
            {
                Prefab = Asset as GameObject;

                if (isPreLoad)
                {
                    if (callback != null)
                    {
                        callback(Asset);
                    }
                }
                else
                {
                    InstObj = GameObject.Instantiate(Asset as GameObject);
                    PrefabAutoDestroy autoDestroy = InstObj.AddComponent<PrefabAutoDestroy>();
                    autoDestroy.mRes = this;
                    if (callback != null)
                    {
                        callback(InstObj);
                    }
                }
            }
            else
            {
                if(callback != null)
                {
                    callback(null);
                }
            }
        }
    }
    
}
