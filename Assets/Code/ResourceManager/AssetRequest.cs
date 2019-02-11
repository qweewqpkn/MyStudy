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
                if(ab == null)
                {
                    Debug.Log(string.Format("ab is null in load {0} AssetRequest", assetName));
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
        public class ABRequest : IEnumerator
        {
            public object Current { get { return null; } }
            public void Reset() { }
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

            public void Load(string mainName, List<string> abList)
            {
                if (abList.Count <= 0)
                {
                    Debug.LogError("ABRequest load ablist is null");
                    return;
                }
                mMainName = mainName;
                mNeedABNum = abList.Count;

                for (int i = 0; i < abList.Count; i++)
                {
                    if(ResourceManager.Instance.mResMap.ContainsKey(abList[i]))
                    {
                        HAssetBundle ab = ResourceManager.Instance.mResMap[abList[i]] as HAssetBundle;
                        if(ab.LoadStatus == HAssetBundle.ABLoadStatus.eNone)
                        {
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
                        HAssetBundle ab = new HAssetBundle(abList[i]);
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
                    Debug.LogError("xxxxxxxx www load is error : " + name + " " + www.error);
                }
                else
                {
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eLoaded;
                    ab.CompleteRequest(www.assetBundle);
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

            public bool MoveNext()
            {
                if (IsLoadComplete())
                {
                    return false;
                }

                return true;
            }
        }

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
                        HAssetBundle abRes = new HAssetBundle(abList[i]);
                        abRes.AB = AssetBundle.LoadFromFile(PathManager.URL(abList[i], assetType, false));
                        abRes.LoadStatus = HAssetBundle.ABLoadStatus.eLoaded;
                        abRes.RefCount++;
                    }
                }

                HAssetBundle mainAB = ResourceManager.Instance.mResMap[mainName] as HAssetBundle;
                return mainAB.AB;
            }
        }
    }
}