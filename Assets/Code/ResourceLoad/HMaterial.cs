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

        public static void Load(string abName, string assetName, Action<Material> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Material);
            };

            HMaterial res = Get<HMaterial>(abName, assetName, tCallBack);
            res.StartLoad();
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }
    }
}
