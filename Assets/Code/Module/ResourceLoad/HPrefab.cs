
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        public GameObject InstObj
        {
            get;
            set;
        }

        public HPrefab()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
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
            res.StartLoad(false, false, false, tCallBack);
        }

        //使用协程等待异步请求，而不用回调的形式
        public static AsyncRequest LoadAsync(string abName, string assetName, params object[] args)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, assetName, (obj, datas) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            }, args);

            return request;
        }

        //预加载资源(与LoadAsync的区别是从AB中加载出了prefab后，不进行实例化而已)
        public static void PreLoadAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
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
            res.StartLoad(false, false, true, tCallBack);
        }

        public static AsyncRequest PreLoadAsync(string abName, string assetName, params object[] args)
        {
            AsyncRequest request = new AsyncRequest();
            PreLoadAsync(abName, assetName, (obj, datas) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            }, args);

            return request;
        }

        public static GameObject Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(true, false, false, null);
            return res.InstObj as GameObject;
        }

        protected override void OnCompleted(bool isPreLoad, Action<UnityEngine.Object> callback)
        {
            if (Asset != null)
            {
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
