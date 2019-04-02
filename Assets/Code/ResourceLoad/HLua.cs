﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    class HLua : HRes
    {
        public HLua() 
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as TextAsset);
                };
            }

            HLua res = Get<HLua>(abName, assetName, AssetType.eLua);
            res.StartLoad(assetName, false, true, tCallBack);
        }

        public static TextAsset Load(string abName, string assetName)
        {
            HLua res = Get<HLua>(abName, assetName, AssetType.eLua);
            res.StartLoad(assetName, true, true, null);
            return res.Asset as TextAsset;
        }
    }
}
