
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    class HTexture : HRes
    {
        public HTexture() 
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Texture> callback)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if(callback != null)
                {
                    callback(null);
                }
                return;
            }

            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Texture);
                };
            }

            HTexture res = Get<HTexture>(abName, assetName, AssetType.eTexture);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        //使用协程等待异步请求，而不用回调的形式
        public static AsyncRequest LoadAsync(string abName, string assetName)
        {
            AsyncRequest request = new AsyncRequest();
            LoadAsync(abName, assetName, (obj) =>
            {
                request.isDone = true;
                request.progress = 1;
                request.Asset = obj;
            });

            return request;
        }

        public static Texture Load(string abName, string assetName)
        {
            if(string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HTexture res = Get<HTexture>(abName, assetName, AssetType.eTexture);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as Texture;
        }
    }
}
