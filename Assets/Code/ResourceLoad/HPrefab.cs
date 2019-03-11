
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

        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
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
                PrefabAutoDestory autoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                autoDestroy.mABName = mABName;
                autoDestroy.mAssetName = assetName;
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
