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

        public AssetRequest(){}

        public IEnumerator Load(AssetBundle ab, string assetName, bool isAll)
        {
            if (ab == null)
            {
                Debug.Log(string.Format("ab is null in load {0} AssetRequest", assetName));
                mIsError = true;
            }
            else
            {
                if (isAll)
                {
                    mRequest = ab.LoadAllAssetsAsync();
                }
                else
                {
                    mRequest = ab.LoadAssetAsync(assetName);
                }

                yield return mRequest;
            }
        }

        public UnityEngine.Object[] GetAssets()
        {
            if (mRequest != null && mRequest.isDone && mIsError == false)
            {
                return mRequest.allAssets;
            }

            return null;
        }

        public T GetAssets<T>(string name) where T : UnityEngine.Object
        {
            if (mRequest != null && mRequest.isDone && mIsError == false)
            {
                int length = mRequest.allAssets.Length;
                for (int i = 0; i < length; i++)
                {
                    if (mRequest.allAssets[i].name.ToLower() == name.ToLower())
                    {
                        return mRequest.allAssets[i] as T;
                    }
                }
            }

            return null;
        }
    }
}