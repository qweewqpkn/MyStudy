using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace AssetLoad
{
    public class AssetRequest
    {
        private AssetBundleRequest mRequest;

        public UnityEngine.Object AssetObj
        {
            get;
            private set;
        }

        public AssetRequest(){}

        public IEnumerator Load(AssetBundle ab, string assetName)
        {
            if (ab == null)
            {
                Debug.LogError(string.Format("AssetRequest ab is null, assetName is {0}", assetName));
                AssetObj = null;
            }
            else
            {
                if(string.IsNullOrEmpty(assetName))
                {
                    AssetObj = null;
                    Debug.LogError("AssetRequest assetName is null");
                    yield break;
                }
                else
                {
                    mRequest = ab.LoadAssetAsync(assetName);
                    yield return mRequest;
                    AssetObj = mRequest.asset;
                }
            }
        }
    }
}