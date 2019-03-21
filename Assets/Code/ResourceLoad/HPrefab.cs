
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        public HPrefab()
        {
        }

        public static void Load(string abName, string assetName, Action<GameObject> callback)
        {
            Action<UnityEngine.Object> tCallBack = (obj) =>
            {
                callback(obj as GameObject);
            };
            LoadRes<HPrefab>(abName, assetName, tCallBack);
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
            if(obj != null)
            {
                GameObject newObj = GameObject.Instantiate(obj as GameObject);
                PrefabAutoDestory autoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                autoDestroy.mRes = this;
                OnCallBack(newObj);
            }
            else
            {
                OnCallBack(null);
            }
        }

        public override void Release()
        {
            base.Release();
        }
    }
    
}
