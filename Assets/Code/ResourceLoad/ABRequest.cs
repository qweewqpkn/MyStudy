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
            AssetBundleManifest mAssestBundleManifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        }

        public IEnumerator Load(string ABName, bool isDep = false)
        {
            if (HAssetBundle.mAssestBundleManifest == null)
            {
                LoadManifest();
            }

            HAssetBundle ab = HRes.GetRes<HAssetBundle>(ABName, "", AssetType.eAB);
            if (!isDep)
            {
                string[] depList = HAssetBundle.mAssestBundleManifest.GetAllDependencies(ABName);
                for (int i = 0; i < depList.Length; i++)
                {
                    yield return Load(depList[i], true);
                }
            }

            string url = PathManager.URL(ABName, AssetType.eAB);
            WWW www = new WWW(url);
            yield return www;

            AB = www.assetBundle;
            ab.RefCount++;
            ab.AB = AB;
            ab.LoadStatus = ABLoadStatus.eLoaded;
            ab.IsCompleted = true;
        }
    }
}
