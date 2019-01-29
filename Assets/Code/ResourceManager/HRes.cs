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

            public HRes(string abName, string assetName)
            {
                mABName = abName;
                mAssetName = assetName;

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

                string name = assetName == "" ? abName : string.Format("{0}/{1}", abName, assetName);
                ResourceManager.Instance.mResMap.Add(name, this);
            }

            public virtual IEnumerator Load<T>(Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return null;
            }

            public virtual void Load(Action<byte[]> success, Action error)
            {
            }

            public virtual void Load(Action<AssetBundle> success, Action error)
            {

            }

            public virtual void Load(Action<Shader[]> success, Action error)
            {
            }

            public virtual void Load(Action<GameObject> success, Action error)
            {
            }

            public virtual void Load(Action<Texture> success, Action error)
            {
            }

            public virtual void Load(Action<AssetBundleManifest> success, Action error)
            {
            }

            public virtual void Load(Action<AudioClip> success, Action error)
            {
            }

            public virtual void Load(Action<Material> success, Action error)
            {
            }

            public virtual void Load(Action<Sprite> success, Action error)
            {

            }

            public virtual void Release()
            {
                //卸载AB
                for (int i = 0; i < mAllABList.Count; i++)
                {
                    string name = mAllABList[i];
                    if (ResourceManager.Instance.mResMap.ContainsKey(name))
                    {
                        HAssetBundle ab = ResourceManager.Instance.mResMap[name] as HAssetBundle;
                        ab.RefCount--;
                        if (ab.RefCount == 0)
                        {
                            if(ab.AB != null)
                            {
                                ab.AB.Unload(true);
                            }
                            else
                            {
                                Debug.Log(string.Format("AB {0} release is null, please check", name));
                            }
                        }
                    }
                }
            }
        }
    }
}
