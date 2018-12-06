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
            //资源名字
            HRes mRes;
            //请求信息
            private WWW mWWW;

            public object Current { get { return null; } }
            public void Reset() { }

            public AssetLoadRequest(HRes res)
            {
                mRes = res;
                mWWW = new WWW(ResourceManager.Instance.URL(res.mAssetName, AssetType.eText));
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
            //当前加载的AB数量
            private int mLoadABNum;
            //需要加载的AB数量
            private int mNeedABNum;
            //加载列表
            List<string> mABList = new List<string>();
            //资源请求
            private AssetBundleRequest mAssetRequest;
            //资源类
            private HRes mRes;
            //ab
            private AssetBundle mAssetBundle;

            public object Current { get { return null; } }
            public void Reset() { }

            public ABAssetLoadRequest(HRes res)
            {
                mABList.Add(res.mABName);
                mABList.AddRange(res.mABDepList);
                mNeedABNum = mABList.Count;
                mRes = res;
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
                        mAssetBundle = www.assetBundle;
                        ResourceManager.Instance.mABLoadedMap.Add(name, new AssetLoadedInfo(mAssetBundle, loadingInfo.mRequestList.Count));
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
                    if (mAssetBundle != null && mAssetRequest == null)
                    {
                        mAssetRequest = mAssetBundle.LoadAllAssetsAsync();
                    }

                    if (mAssetRequest != null && mAssetRequest.isDone)
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