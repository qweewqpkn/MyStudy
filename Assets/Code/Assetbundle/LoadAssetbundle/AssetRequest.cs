using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        //负责加载Assetbundle中的资源
        public class AssetRequest : IEnumerator
        {
            public object Current { get { return null; } }
            public void Reset() { }

            private AssetBundleRequest mRequest;

            public AssetRequest(AssetBundle ab, string assetName, bool isAll = false)
            {
                if(isAll)
                {
                    mRequest = ab.LoadAllAssetsAsync();
                }
                else
                {
                    mRequest = ab.LoadAssetAsync(assetName);
                }
            }

            public bool MoveNext()
            {
                if (mRequest != null && mRequest.isDone)
                {
                    return false;
                }

                return true;
            }

            public UnityEngine.Object[] GetAssets()
            {
                if (mRequest != null && mRequest.isDone)
                {
                    return mRequest.allAssets;
                }

                return null;
            }

            public T GetAssets<T>(string name) where T : UnityEngine.Object
            {
                if (mRequest != null && mRequest.isDone)
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

        //职责：负责加载Assetbundle
        public class ABRequest : IEnumerator
        {
            public object Current { get { return null; } }
            public void Reset() { }
            //当前加载的AB数量
            private int mLoadABNum;
            //需要加载的AB数量
            private int mNeedABNum;
            //ab名字
            private AssetBundle mAB;
            //主ab名字
            private string mMainABName;
            //加载列表
            List<string> mABList = new List<string>();

            public ABRequest(List<string> abList)
            {
                if(abList.Count > 0)
                {
                    mMainABName = abList[0];
                }
                else
                {
                    Debug.LogError("ABRequest load ablist is null");
                    return;
                }
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
                        if(mMainABName == name)
                        {
                            mAB = www.assetBundle;
                        }
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
                    return false;
                }

                return true;
            }

            public AssetBundle GetAB()
            {
                return mAB;
            }
        }
    }
}