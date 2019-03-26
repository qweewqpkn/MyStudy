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
            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as Material);
                };
            }

            HMaterial res = Get<HMaterial>(abName, assetName, AssetType.eMaterial);
            res.StartLoad(assetName, false, tCallBack);
        }

        public static Material Load(string abName, string assetName)
        {
            HMaterial res = Get<HMaterial>(abName, assetName, AssetType.eMaterial);
            res.StartLoad(assetName, true, null);
            return res.AssetObj as Material;
        }
    }
}
