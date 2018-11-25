using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public class AssetLoadRequest : IEnumerator
    {
        public AssetLoadRequest()
        {
        }

        public object Current { get { return null; } }
        public void Reset() { }

        public virtual bool MoveNext()
        {
            return false;
        }

        public virtual void ProcessABAsset<T>(Action<T> success, Action error) where T : UnityEngine.Object
        {

        }

        public virtual void ProcessAsset<T>(Action<T> success, Action error)
        {

        }
    }

    public class TextLoadRequest : AssetLoadRequest
    {
        private string mAssetName;
        private WWW mWWW;

        public TextLoadRequest(string assetName)
        {
            mAssetName = assetName;
            ResourceManager.Instance.StartCoroutine(StartLoad());
        }

        public IEnumerator StartLoad()
        {
            mWWW = new WWW(ResourceManager.Instance.URL(mAssetName, AssetType.eText));
            yield return mWWW;
        }

        public override bool MoveNext()
        {
            if(mWWW != null && mWWW.isDone)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public override void ProcessAsset<T>(Action<T> success, Action error)
        {
            System.Object bytes = mWWW.bytes;
            if(bytes != null)
            {
                if(success != null)
                {
                    success((T)bytes);
                }
            }
            else
            {
                if(error != null)
                {
                    error();
                }
            }
        }
    }

    public class ABLoadRequest : AssetLoadRequest
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
            if(dependencyList != null)
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
                if(loadedInfo == null)
                {
                    //AB不存在
                    AssetLoadingInfo loadingInfo = ResourceManager.Instance.GetLoadingAsset(mABList[i]);
                    if(loadingInfo == null)
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
                loadingInfo.Completed();
                ResourceManager.Instance.RemoveLoadingAsset(name);
                ResourceManager.Instance.AddLoadedAsset(name, www.assetBundle, loadingInfo.mRequestList.Count);
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

        public override bool MoveNext()
        {
            if (IsABLoadComplete())
            {
                if(!string.IsNullOrEmpty(mAssetName))
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

        public override void ProcessABAsset<T>(Action<T> success, Action error)
        {
            if(!string.IsNullOrEmpty(mAssetName))
            {
                //加载AB中的具体资源
                T asset = GetAssets<T>(mAssetName);
                if (asset != null)
                {
                    if (success != null)
                    {
                        success(asset);
                    }
                }
                else
                {
                    if (error != null)
                    {
                        error();
                    }
                }
            }
            else
            {
                //加载AB自身
                if(success != null)
                {
                    success(null);
                }
            }
        }
    }

    public class PrefabLoadRequest : ABLoadRequest
    {
        public PrefabLoadRequest(string abName, string assetName) : base(abName, assetName) { }

        public override void ProcessABAsset<T>(Action<T> success, Action error)
        {
            T asset = GetAssets<T>(mAssetName);
            if (asset != null)
            {
                T obj = GameObject.Instantiate(asset);
                if (success != null)
                {
                    success(asset);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }
    }

    public class ShaderLoadRequest : ABLoadRequest
    {
        public ShaderLoadRequest(string abName, string assetName) : base(abName, assetName) { }

        public override void ProcessABAsset<T>(Action<T> success, Action error)
        {
            UnityEngine.Object[] objs = GetAssets();
            ResourceManager.Instance.CacheAllShader(objs);
            if (objs != null)
            {
                if (success != null)
                {
                    success(null);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }
    }

    public class ManifestLoadRequest : ABLoadRequest
    {
        public ManifestLoadRequest(string abName, string assetName) : base(abName, assetName) { }

        public override void ProcessABAsset<T>(Action<T> success, Action error)
        {
            AssetBundleManifest manifest = GetAssets<AssetBundleManifest>(mAssetName);
            ResourceManager.Instance.SetABManifest(manifest);
            if (manifest != null)
            {
                if (success != null)
                {
                    success(null);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }
    }
}