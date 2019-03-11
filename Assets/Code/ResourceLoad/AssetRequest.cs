﻿using System;
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
    
    //职责：负责加载Assetbundle
    public class ABRequest : RequestAsync
    {
        //当前加载的AB数量
        private int mLoadABNum;
        //需要加载的AB数量
        private int mNeedABNum;
        private string mMainName;
        public AssetBundle mAB
        {
            get
            {
                if(ResourceManager.Instance.mResMap.ContainsKey(mMainName))
                {
                    HAssetBundle ab = ResourceManager.Instance.mResMap[mMainName] as HAssetBundle;
                    return ab.AB;
                }

                return null;
            }
        }

        public ABRequest()
        {
        }

        public void Load(List<string> abList)
        {
            if (abList.Count <= 0)
            {
                Debug.LogError("ABRequest load ablist is null");
                return;
            }
            mMainName = abList[0];
            mNeedABNum = abList.Count;

            for (int i = 0; i < abList.Count; i++)
            {
                if(ResourceManager.Instance.mResMap.ContainsKey(abList[i]))
                {
                    HAssetBundle ab = ResourceManager.Instance.mResMap[abList[i]] as HAssetBundle;
                    if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eNone)
                    {
                        //对于加载HAssetbundle资源时，会出现此情况
                        ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoading;
                        ResourceManager.Instance.StartCoroutine(LoadAB(abList[i], ab));
                    }
                    else if (ab.LoadStatus == HAssetBundle.ABLoadStatus.eLoading)
                    {
                        ab.AddRequest(this);
                    }
                    else if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eLoaded)
                    {
                        //已经存在了这个AB
                        AddLoadNum();
                        ab.RefCount++;
                    }
                }
                else
                {
                    HAssetBundle ab = (HAssetBundle)ResourceManager.Instance.CreateRes(abList[i], "", AssetType.eAB);
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoading;
                    ResourceManager.Instance.StartCoroutine(LoadAB(abList[i], ab));
                }
            }
        }

        private IEnumerator LoadAB(string name, HAssetBundle ab)
        {
            ab.AddRequest(this);
            string url = PathManager.URL(name, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoadError;
                ab.CompleteRequest(null);
                Debug.LogError("xxxxxxxx www load is error : " + name + " " + www.error);
            }
            else
            {
                //加载完成后，可能外部已经释放了这个资源
                if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eRelease)
                {
                    www.assetBundle.Unload(true);
                    ab.CompleteRequest(null);
                }
                else
                {
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoaded;
                    ab.CompleteRequest(www.assetBundle);
                }
            }
        }

        public void AddLoadNum()
        {
            mLoadABNum++;
        }

        protected bool IsLoadComplete()
        {
            if (mLoadABNum >= mNeedABNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool MoveNext()
        {
            if (IsLoadComplete())
            {
                return false;
            }

            return true;
        }
    }

    //同步加载AB
    public class ABRequestSync
    {
        public AssetBundle Load(string mainName, List<string> abList, AssetType assetType)
        {
            if (abList.Count <= 0)
            {
                Debug.LogError("ABRequest load ablist is null");
                return null;
            }

            for (int i = 0; i < abList.Count; i++)
            {
                bool isLoad = false;
                if (ResourceManager.Instance.mResMap.ContainsKey(abList[i]))
                {
                    HAssetBundle ab = ResourceManager.Instance.mResMap[abList[i]] as HAssetBundle;
                    if (ab.LoadStatus == HAssetBundle.ABLoadStatus.eLoaded)
                    {
                        ab.RefCount++;
                    }
                    else
                    {
                        isLoad = true;
                    }
                }
                else
                {
                    isLoad = true;
                }

                if (isLoad)
                {
                    HAssetBundle ab = (HAssetBundle)ResourceManager.Instance.CreateRes(abList[i], "", AssetType.eAB);
                    ab.AB = AssetBundle.LoadFromFile(PathManager.URL(abList[i], assetType, false));
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoaded;
                    ab.RefCount++;
                }
            }

            HAssetBundle mainAB = ResourceManager.Instance.mResMap[mainName] as HAssetBundle;
            return mainAB.AB;
        }
    }
}