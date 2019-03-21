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

        public static void Load(string abName, string assetName, Action<TextAsset> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as TextAsset);
            };
            LoadRes<HText>(abName, assetName, tCallBack);
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
