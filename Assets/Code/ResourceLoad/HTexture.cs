
using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    class HTexture : HRes
    {
        private Texture mTexture;
    
        public HTexture() 
        {
        }
    
        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
        {
            yield return abRequest;
            if(mTexture == null)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, assetName);
                yield return assetRequest;
                mTexture = assetRequest.GetAssets<Texture>(assetName);
            }
    
            if (mTexture != null)
            {
                if (success != null)
                {
                    success(mTexture as T);
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
            mTexture = null;
        }
    }
}
