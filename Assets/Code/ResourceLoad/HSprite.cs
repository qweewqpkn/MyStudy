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
            HSprite res = Get<HSprite>(abName, assetName, AssetType.eSprite);
            res.StartLoad(assetName, true, true, null);
            return res.Asset as Sprite;
        }
    }
}
