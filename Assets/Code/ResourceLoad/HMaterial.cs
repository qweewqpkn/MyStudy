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

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            if (mMaterial == null)
            {
                AssetRequest assetRequest = new AssetRequest(ab, assetName);
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
