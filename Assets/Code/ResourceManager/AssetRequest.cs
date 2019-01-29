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
                    if(ResourceManager.Instance.mResMap.ContainsKey(mABList[i]))
                    {
                        HAssetBundle ab = ResourceManager.Instance.mResMap[mABList[i]] as HAssetBundle;
                        if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eNone)
                        {
                            ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoading;
                            ResourceManager.Instance.StartCoroutine(LoadAB(mABList[i], ab));
                        }
                        else if (ab.LoadStatus == HAssetBundle.ABLoadStatus.eLoading)
                        {
                            ab.AddRequest(this);
                        }
                        else if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eLoaded)
                        {
                            //已经存在了这个AB
                            AddLoadABNum();
                            ab.RefCount++;
                        }
                    }
                    else
                    {
                        ResourceManager.Instance.LoadAB(mABList[i], null);
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
                    Debug.LogError("xxxxxxxx www load is error : " + name + " " + www.error);
                }
                else
                {
                    if(mMainABName == name)
                    {
                        mAB = www.assetBundle;
                    }                                                              
                    ab.CompleteRequest();
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