using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    public enum AssetType
    {
        eNone,
        eAB,
        ePrefab,
        eTexture,
        eAudioClip,
        eText,
        eShader,
        eSprite,
        eLua,
        eManifest,
        eMaterial,
    }

    public class ResourceManager : MonoBehaviour
    {
        private AssetRequestQueue mAssetRequestQueue = new AssetRequestQueue();
        public Action mInitComplete = null;

        private static ResourceManager mInstance;
        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "ResourceManager";
                    mInstance = obj.AddComponent<ResourceManager>();
                    mInstance.Init();
                    DontDestroyOnLoad(obj);
                }

                return mInstance;
            }
        }

        private void Init()
        {
        }

        //单独加载AB
        public void LoadAB(string abName, Action<AssetBundle> callback)
        {
            HAssetBundle.Load(abName, callback);
        }

        //加载text
        public void LoadText(string abName, string assetName, Action<TextAsset> callback)
        {
            HText.Load(abName, assetName, callback);
        }

        //加载prefab
        public void LoadPrefab(string abName, string assetName, Action<GameObject> callback)
        {
            HPrefab.Load(abName, assetName, callback);
        }

        //加载图集
        public void LoadSprite(string abName, string assetName, Action<Sprite> callback)
        {
            HSprite.Load(abName, assetName, callback);
        }

        //加载贴图
        public void LoadTexture(string abName, string assetName, Action<Texture> callback)
        {
            HTexture.Load(abName, assetName, callback);
        }

        //加载音频
        public void LoadAudioClip(string abName, string assetName, Action<AudioClip> callback)
        {
            HAudioCilp.Load(abName, assetName, callback);
        }

        //加载材质
        public void LoadMaterial(string abName, string assetName, Action<Material> callback)
        {
            HMaterial.Load(abName, assetName, callback);
        }
        
        public void LoadShader(string abName, string assetName, Action<Shader> callback)
        {
            HShader.Load(abName, assetName, callback);
        }
        
        public void LoadLua(string abName, string assetName, Action<TextAsset> callback)
        {
            HLua.Load(abName, assetName, callback);
        }

        public void Release(string name)
        {
            //name = name.ToLower();
            //Release(name, name);
        }

        //public void Release(string abName, string assetName)
        //{
        //    HRes res;
        //    string name = HRes.GetResName(abName.ToLower(), assetName);
        //    if (HRes.mResMap.TryGetValue(name, out res))
        //    {
        //        res.Release();
        //    }
        //}
        //
        //public void ReleaseAll()
        //{
        //    List<HRes> resList = new List<HRes>();
        //    foreach (var item in mResMap)
        //    {
        //        resList.Add(item.Value);
        //    }
        //
        //    for(int i = 0; i < resList.Count; i++)
        //    {
        //        resList[i].ReleaseAll();
        //    }
        //
        //    //清空正在请求的队列
        //    mAssetRequestQueue.ReleaseAll();
        //}

        void Update()
        {
            mAssetRequestQueue.Update();
        }
    }
}
