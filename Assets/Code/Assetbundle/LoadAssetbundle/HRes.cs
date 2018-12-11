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
            protected List<string> mAllABList;

            public HRes(string abName, string AssetName)
            {
                mABName = abName;
                mAssetName = AssetName;

                if(!string.IsNullOrEmpty(mABName))
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

            public virtual IEnumerator Load<T>(Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return null;
            }

            public virtual IEnumerator Load(Action success, Action error)
            {
                yield return null;
            }

            public virtual void Release()
            {
                //卸载AB
                for (int i = 0; i < mAllABList.Count; i++)
                {
                    ReleaseAB(mAllABList[i]);
                }
            }

            private void ReleaseAB(string name)
            {
                AssetLoadedInfo info;
                if (ResourceManager.Instance.mABLoadedMap.TryGetValue(mABName, out info))
                {
                    info.Ref--;
                    if (info.Ref == 0)
                    {
                        info.AB.Unload(true);
                        ResourceManager.Instance.mABLoadedMap.Remove(mABName);
                    }
                }
            }
        }
    }
}
