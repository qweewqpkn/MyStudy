
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

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            if(mPrefab == null)
            {
                AssetRequest assetRequest = new AssetRequest(ab, assetName, false);
                yield return assetRequest;
                mPrefab = assetRequest.GetAssets<GameObject>(assetName);
            }

            if(mPrefab != null)
            {
                GameObject newObj = GameObject.Instantiate(mPrefab);
                PrefabAutoDestory autoDestroy = newObj.AddComponent<PrefabAutoDestory>();
                autoDestroy.mABName = ABName;
                autoDestroy.mAssetName = assetName;
                autoDestroy.mPrefab = mPrefab;
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

        public GameObject GetPrefab()
        {
            return mPrefab;
        }

        public override void Release()
        {
            base.Release();
            mPrefab = null;
        }
    }
    
}
