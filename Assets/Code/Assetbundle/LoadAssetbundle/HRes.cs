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
            public string mABName;
            public string mAssetName;
            public List<string> mABDepList;

            public HRes()
            {
                if(!string.IsNullOrEmpty(mABName))
                {
                    mABDepList = new List<string>();
                    string[] depList = ResourceManager.Instance.mAssestBundleManifest.GetAllDependencies(mABName);
                    mABDepList.AddRange(depList);
                }
            }

            public virtual void Release()
            {
                //卸载自身
                ReleaseAB(mABName);
                //卸载它依赖的AB
                for (int i = 0; i < mABDepList.Count; i++)
                {
                    ReleaseAB(mABDepList[i]);
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
