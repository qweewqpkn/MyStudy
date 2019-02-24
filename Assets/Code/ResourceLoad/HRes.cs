using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        public class HRes
        {
            protected string mABName;
            protected string mAssetName;
            protected AssetType mAssetType;
            protected List<string> mAllABList;

            //该资源加载次数
            public int LoadCount
            {
                get;
                set;
            }

            public HRes(string abName, string assetName, AssetType assetType)
            {
                mABName = abName;
                mAssetName = assetName;
                mAssetType = assetType;

                string name = ResourceManager.GetResName(abName, assetName, assetType);
                ResourceManager.Instance.mResMap.Add(name, this);

                if (!string.IsNullOrEmpty(mABName))
                {
                    mAllABList = new List<string>();
                    mAllABList.Add(mABName);
                    if(ResourceManager.Instance.mAssestBundleManifest != null)
                    {
                        string[] depList = ResourceManager.Instance.mAssestBundleManifest.GetAllDependencies(mABName);
                        mAllABList.AddRange(depList);
                    }
                }
            }

            //同步加载
            public virtual T LoadSync<T>(string abName, string assetName) where T : UnityEngine.Object
            {
                Load();
                return default(T);
            }

            //异步加载
            public virtual void Load<T>(string abName, string assetName, Action<T> success, Action error) where T : UnityEngine.Object
            {
                Load();
            }

            public virtual void Load(Action<byte[]> success, Action error)
            {
                Load();
            }

            private void Load()
            {
                LoadCount++;
            }

            public void ReleaseAll()
            {
                int count = LoadCount;
                for(int i = 0; i < count; i++)
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
                        if (ab != null)
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
}
