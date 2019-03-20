using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace AssetLoad
{

    public class RequestAsync : IEnumerator
    {
        public object Current { get { return null; } }

        public virtual bool MoveNext()
        {
            return false;
        }

        public virtual void Reset()
        {
            
        }
    }

    //负责加载Assetbundle中的资源
    public class AssetRequest : RequestAsync
    {
        private AssetBundleRequest mRequest;
        private bool mIsError = false;

        public AssetRequest(AssetBundle ab, string assetName, bool isAll = false)
        {
            if(ab == null)
            {
                Debug.Log(string.Format("ab is null in load {0} AssetRequest", assetName));
                mIsError = true;
                return;
            }

            if(isAll)
            {
                mRequest = ab.LoadAllAssetsAsync();
            }
            else
            {
                mRequest = ab.LoadAssetAsync(assetName);
            }
        }

        public override bool MoveNext()
        {
            if ((mRequest != null && mRequest.isDone) || mIsError)
            {
                return false;
            }

            return true;
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

    public class AssetRequestSync
    {
        public AssetRequestSync()
        {
           
        }

        public UnityEngine.Object[] LoadAll(AssetBundle ab)
        {
            if (ab == null)
            {
                Debug.LogError(string.Format("ab is null in load all AssetRequestSync"));
                return null;
            }

            return ab.LoadAllAssets();
        }

        public UnityEngine.Object Load(AssetBundle ab, string assetName)
        {
            if (ab == null)
            {
                Debug.LogError(string.Format("ab is null in load {0} AssetRequestSync", assetName));
                return null;
            }

            return ab.LoadAsset(assetName);
        }     
    }
}