using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        public class AssetLoadRequest : IEnumerator
        {
            //请求信息
            private WWW mWWW;

            public object Current { get { return null; } }
            public void Reset() { }

            public AssetLoadRequest(string assetName)
            {
                mWWW = new WWW(ResourceManager.Instance.URL(assetName, AssetType.eText));
            }

            public bool MoveNext()
            {
                if(mWWW != null && mWWW.isDone)
                {
                    return false;
                }

                return true;
            }

            public byte[] GetText()
            {
                return mWWW.bytes;
            }
        }

        public class ABAssetLoadRequest : IEnumerator
        {
            public object Current { get { return null; } }
            public void Reset() { }
            //当前加载的AB数量
            private int mLoadABNum;
            //需要加载的AB数量
            private int mNeedABNum;
            //ab名字
            private string mABName;
            //加载资源(如果为空,那么就只加载AB,不会加载其中的资源)
            private string mAssetName;
            //加载列表
            List<string> mABList = new List<string>();
            //资源请求
            private AssetBundleRequest mAssetRequest;

            public ABAssetLoadRequest(string abName, string assetName, List<string> abList)
            {
                mABName = abName;
                mAssetName = assetName;
                mABList.AddRange(abList);
                mNeedABNum = mABList.Count;
                StartLoad();
            }

            private void StartLoad()
            {
                for (int i = 0; i < mABList.Count; i++)
                {
                    AssetLoadedInfo loadedInfo;
                    ResourceManager.Instance.mABLoadedMap.TryGetValue(mABList[i], out loadedInfo);
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
                        loadedInfo.Ref++;
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
                        ResourceManager.Instance.mABLoadedMap.Add(name, new AssetLoadedInfo(www.assetBundle, loadingInfo.mRequestList.Count));
                        loadingInfo.Completed();
                        ResourceManager.Instance.RemoveLoadingAsset(name);
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

            public bool MoveNext()
            {
                if (IsABLoadComplete())
                {
                    if(string.IsNullOrEmpty(mAssetName))
                    {
                        return false;
                    }
                    else
                    {
                        if (mAssetRequest == null)
                        {
                            AssetLoadedInfo info;
                            if(ResourceManager.Instance.mABLoadedMap.TryGetValue(mABName, out info))
                            {
                                mAssetRequest = info.AB.LoadAllAssetsAsync();
                            }
                            else
                            {
                                Debug.LogError("load ab error, please check!");
                                return false;
                            }
                        }

                        if (mAssetRequest != null && mAssetRequest.isDone)
                        {
                            return false;
                        }
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