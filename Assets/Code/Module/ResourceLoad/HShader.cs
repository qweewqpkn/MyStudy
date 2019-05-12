using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HShader : HRes
    {
        public HShader()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Shader> callback)
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
                    callback(data.mAsset as Shader);
                };
            }

            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
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

        public static Shader Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HShader res = Get<HShader>(abName, assetName, AssetType.eShader);
            res.StartLoad(true, false, false, null);
            return res.AssetData.mAsset as Shader;
        }
    }
}
