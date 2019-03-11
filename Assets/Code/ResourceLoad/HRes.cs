using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public class HRes
    {
        protected string mABName;
        protected string mAssetName;
        protected List<string> mAllABList = new List<string>();

        //该资源加载次数
        public int LoadCount
        {
            get;
            set;
        }

        public HRes()
        {
        }

        public virtual void Init(string abName)
        {
            if (!string.IsNullOrEmpty(abName) && mAllABList.Count == 0)
            {
                mABName = abName;
                mAllABList.Add(abName);
                if (ResourceManager.Instance.mAssestBundleManifest != null)
                {
                    string[] depList = ResourceManager.Instance.mAssestBundleManifest.GetAllDependencies(mABName);
                    mAllABList.AddRange(depList);
                }
            }
        }

        //同步加载
        public virtual T LoadSync<T>(string abName, string assetName) where T : UnityEngine.Object
        {
            LoadCount++;
            return default(T);
        }

        //异步加载
        public virtual void Load<T>(string abName, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            LoadCount++;
            ABRequest abRequest = new ABRequest();
            abRequest.Load(mAllABList);
            ResourceManager.Instance.StartCoroutine(Load(abRequest, assetName, success, error));
        }

        protected virtual IEnumerator Load<T>(ABRequest abRequest, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
        {
            yield return null;
        }

        public void ReleaseAll()
        {
            int count = LoadCount;
            for (int i = 0; i < count; i++)
            {
                Release();
            }
        }

        public virtual void Release()
        {
            LoadCount--;
            //该资源每次引用对应的ab依赖都要进行释放
            for (int j = 0; j < mAllABList.Count; j++)
            {
                if (ResourceManager.Instance.mResMap.ContainsKey(mAllABList[j]))
                {
                    HAssetBundle ab = ResourceManager.Instance.mResMap[mAllABList[j]] as HAssetBundle;
                    ab.LoadStatus = HAssetBundle.ABLoadStatus.eRelease; //标记AB为待释放,因为可能释放资源的时候，它引用的ab还在加载中..
                    if (ab != null && ab.RefCount > 0)
                    {
                        ab.RefCount--;
                        if (ab.RefCount == 0)
                        {
                            if (ab.AB != null)
                            {
                                ab.AB.Unload(true);
                            }
                            else
                            {
                                Debug.LogError("HRes Release ab is null, please check!");
                            }
                        }
                    }
                }
            }
        }
    }
}
