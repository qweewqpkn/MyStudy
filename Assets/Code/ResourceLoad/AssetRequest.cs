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
        private bool mIsError = false;

        public UnityEngine.Object AssetObj
        {
            get;
            private set;
        }

        public AssetRequest(){}

        public IEnumerator Load(AssetBundle ab, string assetName)
        {
            if(string.IsNullOrEmpty(assetName))
            {
                AssetObj = ab;
                yield break;
            }

            if (ab == null)
            {
                Debug.Log(string.Format("ab is null in load {0} AssetRequest", assetName));
                AssetObj = null;
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