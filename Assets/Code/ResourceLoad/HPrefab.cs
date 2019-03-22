﻿
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

        protected override void StartLoad(params object[] datas)
        {
            HAB = HAssetBundle.Load(ABName, (ab) =>
            {
                ResourceManager.Instance.StartCoroutine(CoLoad(ab));
            }, false);
        }

        IEnumerator CoLoad(AssetBundle ab)
        {
            AssetRequest assetRequest = new AssetRequest();
            yield return assetRequest.Load(ab, AssetName);
            OnCompleted(assetRequest.AssetObj);
        }

        protected override void OnCompleted(UnityEngine.Object obj)
        {
            base.OnCompleted(obj);
            if(obj != null)
            {
                for (int i = 0; i < mCallBackList.Count; i++)
                {
                    GameObject newObj = GameObject.Instantiate(obj as GameObject);
                    PrefabAutoDestory autoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                    autoDestroy.mRes = this;
                    mCallBackList[i](newObj);
                }
                mCallBackList.Clear();
            }
            else
            {
                OnCallBack(null);
            }
        }
    }
    
}
