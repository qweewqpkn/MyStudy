﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HAudioCilp : HRes
    {
        public HAudioCilp()
        { 
        }

        public static void LoadAsync(string abName, string assetName, Action<AudioClip> callback)
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
                    callback(data.mAsset as AudioClip);
                };
            }

            HAudioCilp res = Get<HAudioCilp>(abName, assetName, AssetType.eAudioClip);
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

        public static AudioClip Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HAudioCilp res = Get<HAudioCilp>(abName, assetName, AssetType.eAudioClip);
            res.StartLoad(true, false, false, null);
            return res.AssetData.mAsset as AudioClip;
        }
    }
}
