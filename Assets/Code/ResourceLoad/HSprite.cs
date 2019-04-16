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

            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Sprite);
                };
            }

            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(assetName, false, true, tCallBack);
        }

        public static Sprite Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(assetName, true, true, null);
            return res.Asset as Sprite;
        }
    }
}
