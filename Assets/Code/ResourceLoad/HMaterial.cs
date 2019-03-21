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

            LoadRes<HMaterial>(abName, assetName, tCallBack);
        }

        protected override void Init(string abName, string assetName, string resName)
        {
            base.Init(abName, assetName, resName);
            HAssetBundle.Load(abName, (ab) =>
            {
                ResourceManager.Instance.StartCoroutine(CoLoad(ab, abName, assetName));
            });
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }

        public override void Release()
        {
            base.Release();
        }
    }
}
