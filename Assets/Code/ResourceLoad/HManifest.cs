using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HManifest : HRes
    {
        private AssetBundleManifest mManifest;

        public HManifest()
        {
        }

        protected override IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error)
        {
            yield return abRequest;
            if (mManifest == null)
            {
                AssetRequest assetRequest = new AssetRequest(abRequest.mAB, assetName);
                yield return assetRequest;
                mManifest = assetRequest.GetAssets<AssetBundleManifest>(assetName);
            }

            if (mManifest != null)
            {
                if (success != null)
                {
                    success(mManifest as T);
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
            mManifest = null;
        }
    }
}
