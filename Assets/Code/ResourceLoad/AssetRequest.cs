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

        public IEnumerator Load(AssetBundle ab, string assetName, bool isAll = false)
        {
            if (ab == null)
            {
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
                    if(isAll)
                    {
                        mRequest = ab.LoadAllAssetsAsync();
                        yield return mRequest;
                        for(int i = 0; i < mRequest.allAssets.Length; i++)
                        {
                            if(mRequest.allAssets[i].name.ToLower() == assetName.ToLower())
                            {
                                AssetObj = mRequest.allAssets[i];
                                break;
                            }
                        }
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
}