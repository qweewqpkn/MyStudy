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

        public static void Load(string abName, string assetName, Action<Sprite> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as Sprite);
            };
            LoadRes<HSprite>(abName, assetName, tCallBack);
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
