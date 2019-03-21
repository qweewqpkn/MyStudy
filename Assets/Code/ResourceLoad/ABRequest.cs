using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class ABRequest
    {
        private List<HAssetBundle> mDepList = new List<HAssetBundle>();
        public AssetBundle AB
        {
            get;
            set;
        }

        private void LoadManifest()
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathManager.URL("Assetbundle", AssetType.eManifest, false));
            HAssetBundle.mAssestBundleManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        }

        public IEnumerator Load(string abName, bool isDep = false)
        {
            if (HAssetBundle.mAssestBundleManifest == null)
            {
                LoadManifest();
            }

            if (!isDep)
            {
                //加载依赖的AB
                List<HAssetBundle> depResList = new List<HAssetBundle>();
                string[] depNameList = HAssetBundle.mAssestBundleManifest.GetAllDependencies(abName);
                for (int i = 0; i < depNameList.Length; i++)
                {
                    depResList.Add(HAssetBundle.Load(depNameList[i], null));
                }

                for(int i = 0; i < depResList.Count; i++)
                {
                    if(!depResList[i].IsCompleted)
                    {
                        yield return null;
                    }
                }
            }

            string url = PathManager.URL(abName, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;

            AB = www.assetBundle;
        }
    }
}
