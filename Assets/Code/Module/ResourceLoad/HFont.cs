using System;
using TMPro;
using UnityEngine;

namespace AssetLoad
{
    class HFont : HRes
    {
        public HFont()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<TMP_FontAsset> callback)
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
                    callback(data.mAsset as TMP_FontAsset);
                };
            }

            HFont res = Get<HFont>(abName, assetName, AssetType.eFont);
            res.StartLoad(false, false, false, tCallBack);
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

        public static TMP_FontAsset Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HFont res = Get<HFont>(abName, assetName, AssetType.eFont);
            res.StartLoad(true, false, false, null);
            return res.AssetData.mAsset as TMP_FontAsset;
        }
    }
}
