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
        public bool IsComplete
        {
            get;
            private set;
        }

        public UnityEngine.Object Asset
        {
            get;
            private set;
        }

        public UnityEngine.Object[] AssetList
        {
            get;
            set;
        }

        public AssetRequest()
        {
            Asset = null;
            AssetList = null;
            IsComplete = false;
        }

        public void Load(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            ResourceManager.Instance.StartCoroutine(CoLoad(ab, assetName, isSync, isAll));
        }

        public IEnumerator CoLoad(AssetBundle ab, string assetName, bool isSync, bool isAll = false)
        {
            if (ab == null)
            {
                IsComplete = true;
            }
            else
            {
                if(string.IsNullOrEmpty(assetName))
                {
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
                            AssetList = ab.LoadAllAssets();
                        }
                        else
                        {
                            Asset = ab.LoadAsset(assetName);
                        }
                    }
                    else
                    {
                        if (isAll)
                        {
                            AssetBundleRequest request = ab.LoadAllAssetsAsync();
                            yield return request;
                            AssetList = request.allAssets;
                        }
                        else
                        {
                            AssetBundleRequest request = ab.LoadAssetAsync(assetName);
                            yield return request;
                            Asset = request.asset;
                        }
                    }

                    IsComplete = true;
                }
            }
        }
    }
}