
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        public GameObject InstObj
        {
            get;
            set;
        }

        public HPrefab()
        {
        }

        public static void LoadAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                if (callback != null)
                {
                    callback(null, null);
                }
                return;
            }

            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as GameObject, args);
                };
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(assetName, false, false, tCallBack);
        }

        public static GameObject Load(string abName, string assetName)
        {
            if (string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("abName or assetName is null!!!");
                return null;
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(assetName, true, false, null);
            return res.InstObj as GameObject;
        }

        protected override void OnCompleted(Action<UnityEngine.Object> callback)
        {
            if (Asset != null)
            {
                InstObj = GameObject.Instantiate(Asset as GameObject);
                PrefabAutoDestroy autoDestroy = InstObj.AddComponent<PrefabAutoDestroy>();
                autoDestroy.mRes = this;
                if(callback != null)
                {
                    callback(InstObj);
                }
            }
            else
            {
                if(callback != null)
                {
                    callback(null);
                }
            }
        }
    }
    
}
