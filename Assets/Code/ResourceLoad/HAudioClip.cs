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

        public static void LoadAsync(string abName, string assetName, Action<AudioClip> callback)
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
                    callback(obj as AudioClip);
                };
            }

            HAudioCilp res = Get<HAudioCilp>(abName, assetName, AssetType.eAudioClip);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        public static AudioClip Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HAudioCilp res = Get<HAudioCilp>(abName, assetName, AssetType.eAudioClip);
            res.StartLoad(assetName, true, false, null);
            return res.Asset as AudioClip;
        }
    }
}
