using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HMaterial : HRes
    {
        private Material mMaterial;

        public HMaterial()
        {
        }

        public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
        {
            base.Load(abName, assetName, success, error);
            ABRequest abRequest = new ABRequest();
            abRequest.Load(mABName, mAllABList);
            ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, success, error));
        }

        private IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            yield return abRequest;
            if (mMaterial == null)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, assetName);
                yield return assetRequest;
                mMaterial = assetRequest.GetAssets<Material>(assetName);
            }

            if (mMaterial != null)
            {
                if (success != null)
                {
                    success(mMaterial as T);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }

        public override void Release()
        {
            base.Release();
            mMaterial = null;
        }
    }
}
