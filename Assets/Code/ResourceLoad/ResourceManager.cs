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
                    DontDestroyOnLoad(obj);
                }

                return mInstance;
            }
        }

        //异步加载AB
        public void LoadABAsync(string abName, Action<AssetBundle> callback)
        {
            HAssetBundle.LoadAsync(abName, callback);
        }

        //同步加载AB
        public AssetBundle LoadAB(string abName)
        {
            return HAssetBundle.Load(abName);
        }

        //异步加载text
        public void LoadTextAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            HText.LoadAsync(abName, assetName, callback);
        }
        
        //同步加载text
        public TextAsset LoadText(string abName, string assetName)
        {
            return HText.Load(abName, assetName);
        }

        //异步加载prefab
        public void LoadPrefabAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            HPrefab.LoadAsync(abName, assetName, callback, args);
        }

        //同步加载prefab
        public GameObject LoadPrefab(string abName, string assetName)
        {
            return HPrefab.Load(abName, assetName);
        }

        //异步加载图集
        public void LoadSpriteAsync(string abName, string assetName, Action<Sprite> callback)
        {
            HSprite.LoadAsync(abName, assetName, callback);
        }

        //同步加载图集
        public Sprite LoadSprite(string abName, string assetName)
        {
            return HSprite.Load(abName, assetName);
        }

        //异步加载贴图
        public void LoadTextureAsync(string abName, string assetName, Action<Texture> callback)
        {
            HTexture.LoadAsync(abName, assetName, callback);
        }

        //同步加载贴图
        public Texture LoadTexture(string abName, string assetName)
        {
            return HTexture.Load(abName, assetName);
        }

        //异步加载音频
        public void LoadAudioClipAsync(string abName, string assetName, Action<AudioClip> callback)
        {
            HAudioCilp.LoadAsync(abName, assetName, callback);
        }

        //同步加载音频
        public AudioClip LoadAudioClip(string abName, string assetName)
        {
            return HAudioCilp.Load(abName, assetName);
        }

        //异步加载材质
        public void LoadMaterialAsync(string abName, string assetName, Action<Material> callback)
        {
            HMaterial.LoadAsync(abName, assetName, callback);
        }

        //同步加载材质
        public Material LoadMaterial(string abName, string assetName)
        {
            return HMaterial.Load(abName, assetName);
        }

        //异步加载shader
        public void LoadShaderAsync(string abName, string assetName, Action<Shader> callback)
        {
            HShader.LoadAsync(abName, assetName, callback);
        }

        //同步加载shader
        public Shader LoadShader(string abName, string assetName)
        {
            return HShader.Load(abName, assetName);
        }

        //异步加载lua
        public void LoadLuaAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            HLua.LoadAsync(abName, assetName, callback);
        }

        //同步加载lua
        public TextAsset LoadLua(string abName, string assetName)
        {
            return HLua.Load(abName, assetName);
        }

        public void Release(string name, AssetType assetType)
        {
            Release(name, name, assetType);
        }

        public void Release(string abName, string assetName, AssetType assetType)
        {
            HRes res;
            abName = abName.ToLower();
            assetName = assetName.ToLower();
            string name = HRes.GetResName(abName, assetName, assetType);
            if (HRes.mResMap.TryGetValue(name, out res))
            {
                res.Release();
            }
        }
        
        public void ReleaseAll()
        {
            List<HRes> resList = new List<HRes>();
            foreach (var item in HRes.mResMap)
            {
                resList.Add(item.Value);
            }


            Debug.Log("stop load AB : " + Time.frameCount);
            //停止掉正在加载的AB
            ABRequest.StopAllRequest();
            //停止掉正在加载的AB中的资源
            AssetRequest.StopAllRequest();

            Debug.Log("release AB : " + Time.frameCount);
            //释放AB资源
            for (int i = 0; i < resList.Count; i++)
            {
                resList[i].ReleaseAll();
            }
        }
    }
}
