using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HLua : HRes
    {
        public HLua() 
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null);
                }
                return;
            }

            Action<AssetLoadData> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (data) =>
                {
                    callback(data.mAsset as TextAsset);
                };
            }

            HLua res = Get<HLua>(abName, assetName, AssetType.eLua);
            res.StartLoad(false, true, false, tCallBack);
        }

        //使用协程等待异步请求，而不用回调的形式
        public static AsyncRequest LoadCoRequest(string abName, string assetName)
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

        public static TextAsset Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HLua res = Get<HLua>(abName, assetName, AssetType.eLua);
            res.StartLoad(true, true, false, null);
            return res.AssetData.mAsset as TextAsset;
        }
    }
}
