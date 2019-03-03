
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{   
    class HPrefab : HRes
    {
        private GameObject mPrefab;

        public HPrefab()
        {
        }

        public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
        {
            base.Load(abName, assetName,success, error);
            ABRequest abRequest = new ABRequest();
            abRequest.Load(abName, mAllABList);
            ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, success, error));
        }

        private IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            yield return abRequest;

            if(mPrefab == null)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, assetName, false);
                yield return assetRequest;
                mPrefab = assetRequest.GetAssets<GameObject>(assetName);
            }

            if(mPrefab != null)
            {
                GameObject newObj = GameObject.Instantiate(mPrefab);
                PrefabAutoDestory audoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                audoDestroy.mABName = mABName;
                if (success != null)
                {
                    success(newObj as T);
                }
            }
            else
            {
                if(error != null)
                {
                    error();
                }
            }
        }

        public override void Release()
        {
            base.Release();
            mPrefab = null;
        }
    }
    
}
