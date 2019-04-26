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

    public class ResourceManager : SingletonMono<ResourceManager>
    {
        #region AB
        //异步加载AB
        public void LoadABAsync(string abName, Action<AssetBundle> callback)
        {
            HAssetBundle.LoadAsync(abName, callback);
        }

        //协程加载AB
        public AsyncRequest LoadABAsync(string abName)
        {
            return HAssetBundle.LoadAsync(abName);
        }

        //同步加载AB
        public AssetBundle LoadAB(string abName)
        {
            return HAssetBundle.Load(abName);
        }
        #endregion

        #region text
        //异步加载text
        public void LoadTextAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            HText.LoadAsync(abName, assetName, callback);
        }

        //协程加载text
        public AsyncRequest LoadTextAsync(string abName, string assetName)
        {
            return HText.LoadAsync(abName, assetName);
        }

        //同步加载text
        public TextAsset LoadText(string abName, string assetName)
        {
            return HText.Load(abName, assetName);
        }
        #endregion

        #region prefab
        //异步加载prefab
        public void LoadPrefabAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            HPrefab.LoadAsync(abName, assetName, callback, args);
        }

        //协程加载prefab
        public AsyncRequest LoadPrefabAsync(string abName, string assetName, params object[] args)
        {
            return HPrefab.LoadAsync(abName, assetName, args);
        }

        //预加载prefab
        public void PreLoadPrefabAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            HPrefab.PreLoadAsync(abName, assetName, callback, args);
        }

        //预加载prefab协程形式
        public AsyncRequest PreLoadPrefabAsync(string abName, string assetName, params object[] args)
        {
            return HPrefab.PreLoadAsync(abName, assetName, args);
        }

        //同步加载prefab
        public GameObject LoadPrefab(string abName, string assetName)
        {
            return HPrefab.Load(abName, assetName);
        }
        #endregion

        #region sprite
        //异步加载图集 
        public void LoadSpriteAsync(string abName, string assetName, Action<Sprite> callback)
        {
            HSprite.LoadAsync(abName, assetName, callback);
        }

        //协程加载图集
        public AsyncRequest LoadSpriteAsync(string abName, string assetName)
        {
            return HSprite.LoadAsync(abName, assetName);
        }

        //同步加载图集
        public Sprite LoadSprite(string abName, string assetName)
        {
            return HSprite.Load(abName, assetName);
        }
        #endregion

        #region texture
        //异步加载贴图
        public void LoadTextureAsync(string abName, string assetName, Action<Texture> callback)
        {
            HTexture.LoadAsync(abName, assetName, callback);
        }

        //协程加载贴图
        public AsyncRequest LoadTextureAsync(string abName, string assetName)
        {
            return HTexture.LoadAsync(abName, assetName);
        }

        //同步加载贴图
        public Texture LoadTexture(string abName, string assetName)
        {
            return HTexture.Load(abName, assetName);
        }
        #endregion

        #region AudioClip
        //异步加载音频
        public void LoadAudioClipAsync(string abName, string assetName, Action<AudioClip> callback)
        {
            HAudioCilp.LoadAsync(abName, assetName, callback);
        }

        //协程加载贴图
        public AsyncRequest LoadAudioClipAsync(string abName, string assetName)
        {
            return HAudioCilp.LoadAsync(abName, assetName);
        }

        //同步加载音频
        public AudioClip LoadAudioClip(string abName, string assetName)
        {
            return HAudioCilp.Load(abName, assetName);
        }
        #endregion

        #region Material
        //异步加载材质
        public void LoadMaterialAsync(string abName, string assetName, Action<Material> callback)
        {
            HMaterial.LoadAsync(abName, assetName, callback);
        }

        //协程加载材质
        public AsyncRequest LoadMaterialAsync(string abName, string assetName)
        {
            return HMaterial.LoadAsync(abName, assetName);
        }

        //同步加载材质
        public Material LoadMaterial(string abName, string assetName)
        {
            return HMaterial.Load(abName, assetName);
        }
        #endregion

        #region Shader
        //异步加载shader
        public void LoadShaderAsync(string abName, string assetName, Action<Shader> callback)
        {
            HShader.LoadAsync(abName, assetName, callback);
        }

        //协程加载shader
        public AsyncRequest LoadShaderAsync(string abName, string assetName)
        {
            return HShader.LoadAsync(abName, assetName);
        }

        //同步加载shader
        public Shader LoadShader(string abName, string assetName)
        {
            return HShader.Load(abName, assetName);
        }
        #endregion

        #region Lua
        //异步加载lua
        public void LoadLuaAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            HLua.LoadAsync(abName, assetName, callback);
        }

        //协程加载lua
        public AsyncRequest LoadLuaAsync(string abName, string assetName)
        {
            return HLua.LoadAsync(abName, assetName);
        }

        //同步加载lua
        public TextAsset LoadLua(string abName, string assetName)
        {
            return HLua.Load(abName, assetName);
        }
        #endregion

        public void Release(string name)
        {
            Release(name, name);
        }

        public void Release(string abName, string assetName)
        {
            HRes res;
            abName = abName.ToLower();
            assetName = assetName.ToLower();
            string name = HRes.GetResName(abName, assetName);
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

            //停止掉正在加载的AB
            ABRequest.StopAllRequest();
            //停止掉正在加载的AB中的资源
            AssetRequest.StopAllRequest();

            //释放AB资源
            for (int i = 0; i < resList.Count; i++)
            {
                resList[i].ReleaseAll();
            }
        }
    }
}
