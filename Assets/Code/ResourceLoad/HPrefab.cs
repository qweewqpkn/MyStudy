
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

        public static void LoadAsync(string abName, string assetName, Action<GameObject> callback)
        {
            Action<UnityEngine.Object> tCallBack = null;
            if (callback != null)
            {
                tCallBack = (obj) =>
                {
                    callback(obj as GameObject);
                };
            }

            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(assetName, false, tCallBack);
        }

        public static GameObject Load(string abName, string assetName)
        {
            HPrefab res = Get<HPrefab>(abName, assetName, AssetType.ePrefab);
            res.StartLoad(assetName, true, null);
            return res.InstObj as GameObject;
        }

        protected override void OnCompleted(UnityEngine.Object obj, Action<UnityEngine.Object> callback)
        {
            AssetObj = obj;

            if (obj != null)
            {
                InstObj = GameObject.Instantiate(obj as GameObject);
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
