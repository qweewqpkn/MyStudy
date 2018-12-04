using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        public class AssetLoadRequest : IEnumerator
        {
            private string mAssetName;
            private WWW mWWW;
            public object Current { get { return null; } }
            public void Reset() { }

            public AssetLoadRequest(string assetName)
            {
                mAssetName = assetName;
                ResourceManager.Instance.StartCoroutine(StartLoad());
            }

            public IEnumerator StartLoad()
            {
                mWWW = new WWW(ResourceManager.Instance.URL(mAssetName, AssetType.eText));
                yield return mWWW;
            }

            public byte[] GetText()
            {
                return mWWW.bytes;
            }

            public bool MoveNext()
            {
                if (mWWW != null && mWWW.isDone)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public class ABLoadRequest : IEnumerator
        {
            protected string mABName;
            //当前加载的AB数量
            private int mLoadABNum;
            //需要加载的AB数量
            private int mNeedABNum;
            //加载列表
            List<string> mABList = new List<string>();
            //AB包含的资源名字
            protected string mAssetName;
            //资源请求
            protected AssetBundleRequest mAssetRequest;

            public ABLoadRequest(string abName, string assetName)
            {
                List<string> dependencyList = ResourceManager.Instance.GetABDependency(abName);
                if (dependencyList != null)
                {
                    mABList.AddRange(dependencyList);
                }
                mABList.Add(abName);
                mABName = abName;
                mAssetName = assetName;
                mNeedABNum = mABList.Count;
                StartLoad();
            }

            private void StartLoad()
            {
                for (int i = 0; i < mABList.Count; i++)
                {
                    AssetLoadedInfo loadedInfo = ResourceManager.Instance.GetLoadedAsset(mABList[i]);
                    if (loadedInfo == null)
                    {
                        //AB不存在
                        AssetLoadingInfo loadingInfo = ResourceManager.Instance.GetLoadingAsset(mABList[i]);
                        if (loadingInfo == null)
                        {
                            //AB没有在加载
                            ResourceManager.Instance.AddLoadingAsset(mABList[i]);
                            ResourceManager.Instance.StartCoroutine(LoadAB(mABList[i]));
                        }
                        else
                        {
                            //AB正在加载中
                            loadingInfo.AddLoadRequest(this);
                        }
                    }
                    else
                    {
                        //已经存在了这个AB
                        AddLoadABNum();
                        ResourceManager.Instance.AddRef(mABList[i]);
                    }
                }
            }

            private IEnumerator LoadAB(string name)
            {
                AssetLoadingInfo loadingInfo = ResourceManager.Instance.GetLoadingAsset(name);
                if (loadingInfo != null)
                {
                    loadingInfo.AddLoadRequest(this);
                    string url = ResourceManager.Instance.URL(name, AssetType.eAB);
                    WWW www = new WWW(url);
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError("xxxxxxxx www load is error : " + name + " " + www.error);
                    }
                    else
                    {
                        loadingInfo.Completed();
                        ResourceManager.Instance.RemoveLoadingAsset(name);
                        ResourceManager.Instance.AddLoadedAsset(name, www.assetBundle, loadingInfo.mRequestList.Count);
                    }
                }
            }

            public void AddLoadABNum()
            {
                mLoadABNum++;
            }

            protected bool IsABLoadComplete()
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

            public object Current { get { return null; } }
            public void Reset() { }
            public bool MoveNext()
            {
                if (IsABLoadComplete())
                {
                    if (!string.IsNullOrEmpty(mAssetName))
                    {
                        if (mAssetRequest == null)
                        {
                            AssetLoadedInfo info = ResourceManager.Instance.GetLoadedAsset(mABName);
                            mAssetRequest = info.mAB.LoadAllAssetsAsync();
                        }

                        if (mAssetRequest != null && mAssetRequest.isDone)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            public virtual UnityEngine.Object[] GetAssets()
            {
                if (mAssetRequest != null && mAssetRequest.isDone)
                {
                    return mAssetRequest.allAssets;
                }

                return null;
            }

            public virtual T GetAssets<T>(string name) where T : UnityEngine.Object
            {
                if (mAssetRequest != null && mAssetRequest.isDone)
                {
                    int length = mAssetRequest.allAssets.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (mAssetRequest.allAssets[i].name.ToLower() == name.ToLower())
                        {
                            return mAssetRequest.allAssets[i] as T;
                        }
                    }
                }

                return null;
            }
        }
    }
}