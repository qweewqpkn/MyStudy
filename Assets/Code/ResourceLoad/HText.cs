using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HText : HRes
    {
        public HText()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if(callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as TextAsset);
                };
            }

            HText res = Get<HText>(abName, assetName, AssetType.eText);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        public static TextAsset Load(string abName, string assetName)
        {
            HText res = Get<HText>(abName, assetName, AssetType.eText);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as TextAsset;
        }
    }
}
