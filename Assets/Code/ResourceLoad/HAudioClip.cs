using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HAudioCilp : HRes
    {
        public HAudioCilp()
        { 
        }

        public static void Load(string abName, string assetName, Action<AudioClip> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as AudioClip);
            };

            HAudioCilp res = Get<HAudioCilp>(abName, assetName, tCallBack);
            res.StartLoad();
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            OnCallBack(AssetObj);
        }
    }
}
