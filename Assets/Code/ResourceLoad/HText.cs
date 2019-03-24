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
            HText res = Get<HText>(abName, assetName, tCallBack);
            res.StartLoad();
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }
    }
}
