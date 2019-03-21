
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

        public static void Load(string abName, string assetName, Action<Texture> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Texture);
            };
            LoadRes<HTexture>(abName, assetName, tCallBack);
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
