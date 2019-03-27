
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

        public static Texture Load(string abName, string assetName)
        {
            HTexture res = Get<HTexture>(abName, assetName, AssetType.eTexture);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as Texture;
        }
    }
}
