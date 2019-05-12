using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HSprite : HRes
    {
        public HSprite()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Sprite> callback)
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
                    callback(data.mAsset as Sprite);
                };
            }

            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
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

        public static Sprite Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(true, true, false, null);
            return res.AssetData.mAsset as Sprite;
        }
    }
}
