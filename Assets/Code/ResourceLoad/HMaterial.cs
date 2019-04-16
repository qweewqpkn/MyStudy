using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HMaterial : HRes
    {
        public HMaterial()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<Material> callback)
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
                    callback(obj as Material);
                };
            }

            HMaterial res = Get<HMaterial>(abName, assetName, AssetType.eMaterial);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        public static Material Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HMaterial res = Get<HMaterial>(abName, assetName, AssetType.eMaterial);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as Material;
        }
    }
}
