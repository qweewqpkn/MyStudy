using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.U2D;

namespace AssetLoad
{
    public class AssetRequest
    {
        private AssetBundleRequest mRequest;

        private bool mIsComplete = false;
        public bool IsComplete
        {
            get
            {
                return mIsComplete;
            }

            private set
            {
                mIsComplete = value;
            }
        }

        public UnityEngine.Object AssetObj
        {
            get;
            private set;
        }

        public UnityEngine.Object[] AssetsList
        {
            get;
            set;
        }

        public AssetRequest(){}

        public void Load(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(ab, assetName, isSync, isAll));
        }

        public IEnumerator CoLoad(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            if (ab == null)
            {
                AssetObj = null;
                IsComplete = true;
            }
            else
            {
                if(string.IsNullOrEmpty(assetName))
                {
                    AssetObj = null;
                    IsComplete = true;
                    Debug.LogError("AssetRequest assetName is null");
                    yield break;
                }
                else
                {
                    if(isSync)
                    {
                        if(isAll)
                        {
                            AssetsList = ab.LoadAllAssets();
                        }
                        else
                        {
                            AssetObj = ab.LoadAsset(assetName);
                        }
                    }
                    else
                    {
                        if (isAll)
                        {
                            mRequest = ab.LoadAllAssetsAsync();
                            yield return mRequest;
                            AssetsList = mRequest.allAssets;
                        }
                        else
                        {
                            mRequest = ab.LoadAssetAsync(assetName);
                            yield return mRequest;
                            AssetObj = mRequest.asset;
                        }
                    }

                    IsComplete = true;
                }
            }
        }
    }
}