using System;
using System.Collections;
using UnityEngine;

namespace AssetLoad
{
    public enum LoadType
    {
        eOneAsset,
        eAllAsset,
        eOnlyAB,
    }

    public abstract class AssetLoadRequest : IEnumerator
    {
        protected int mNeedLoadNum;
        protected int mCurLoadNum;
        protected string mABName;
        protected string mAssetName;
        protected LoadType mLoadType;

        public AssetLoadRequest()
        {
            mNeedLoadNum = 0;
            mCurLoadNum = 0;
            mABName = "";
            mAssetName = "";
            mLoadType = LoadType.eOneAsset;
        }

        public void AddNeedLoadNum()
        {
            mNeedLoadNum++;
        }

        public void AddCurLoadNum()
        {
            mCurLoadNum++;
        }

        public virtual bool IsLoadFinish()
        {
            //是否加载完主资源和依赖资源
            return mCurLoadNum >= mNeedLoadNum;
        }

        public void Init(string abName, string assetName, LoadType loadType = LoadType.eOneAsset)
        {
            mABName = abName;
            mAssetName = assetName;
            mLoadType = loadType;
        }

        public virtual bool Update()
        {
            return true;
        }

        public abstract bool MoveNext();

        public abstract void Reset();

        public object Current
        {
            get
            {
                return null;
            }
        }

    }

    public class NormalAssetLoadRequest : AssetLoadRequest
    {
        public AssetLoadedInfo mAssetLoadedInfo;

        public NormalAssetLoadRequest()
        {

        }

        public override bool Update()
        {
            if(IsLoadFinish())
            {
                ResourceManager.Instance.mABLoadedMap.TryGetValue(mAssetName, out mAssetLoadedInfo);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Reset()
        {

        }


        public override bool MoveNext()
        {
            if(IsLoadFinish())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class ABAssetLoadRequest : AssetLoadRequest
    {
        private AssetBundleRequest mABRequest;

        public ABAssetLoadRequest()
        {
            mABRequest = null;
        }

        public override void Reset()
        {
            
        }

        public override bool Update()
        {
            if (IsLoadFinish())
            {
                AssetLoadedInfo loadedInfo = null;
                if (ResourceManager.Instance.mABLoadedMap.TryGetValue(mABName, out loadedInfo))
                {
                    switch (mLoadType)
                    {
                        case LoadType.eOneAsset:
                            {
                                mABRequest = loadedInfo.mAB.LoadAssetAsync(mAssetName);
                            }
                            break;
                        case LoadType.eAllAsset:
                            {
                                mABRequest = loadedInfo.mAB.LoadAllAssetsAsync();
                            }
                            break;
                        case LoadType.eOnlyAB:
                            {

                            }
                            break;
                    }
                    return true;
                }
            }

            return false;
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            if (mABRequest != null && mABRequest.isDone)
            {
                return mABRequest.asset as T;
            }

            return null;
        }

        public UnityEngine.Object[] GetAssets<T>() where T : UnityEngine.Object
        {
            if (mABRequest != null && mABRequest.isDone)
            {
                return mABRequest.allAssets;
            }

            return null;
        }

        public T GetAssets<T>(string name) where T : UnityEngine.Object
        {
            if (mABRequest != null && mABRequest.isDone)
            {
                int length = mABRequest.allAssets.Length;
                for (int i = 0; i < length; i++)
                {
                    if(mABRequest.allAssets[i].name == name)
                    {
                        return mABRequest.allAssets[i] as T;
                    }
                }
            }

            return null;
        }

        public override bool MoveNext()
        {
            if (mLoadType == LoadType.eOnlyAB)
            {
                if(IsLoadFinish())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (mABRequest != null && mABRequest.isDone)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

}