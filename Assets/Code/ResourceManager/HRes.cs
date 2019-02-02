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

            //该资源被引用的次数
            public int RefCount
            {
                get;
                set;
            }

            public HRes(string abName, string assetName, AssetType assetType)
            {
                mABName = abName;
                mAssetName = assetName;
                mAssetType = assetType;

                string name = ResourceManager.GetResName(abName, assetName);
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

            public virtual void Load<T>(Action<T> success, Action error) where T : UnityEngine.Object
            {
                Load();
            }

            public virtual void Load<T>(string assetName, Action<T> success, Action error) where T : UnityEngine.Object
            {
                Load();
            }

            public virtual void Load(Action<byte[]> success, Action error)
            {
                Load();
            }

            private void Load()
            {
                RefCount++;
            }

            public virtual void Release()
            {
                //该资源被引用了多少次
                for(int i = 0; i < RefCount; i++)
                {
                    //该资源每次引用对应的ab依赖都要进行释放
                    for(int j = 0; j < mAllABList.Count; j++)
                    {
                        if(ResourceManager.Instance.mResMap.ContainsKey(mAllABList[j]))
                        {
                            HAssetBundle ab = ResourceManager.Instance.mResMap[mAllABList[j]] as HAssetBundle;
                            if(ab != null)
                            {
                                ab.RefCount--;
                                if(ab.RefCount == 0)
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
}
