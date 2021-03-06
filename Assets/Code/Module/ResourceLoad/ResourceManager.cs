﻿using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
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
        eSpriteAtlas,
        eFont,
    }

    public class ResourceManager : SingletonMono<ResourceManager>
    {
        private Dictionary<string, Material> mMaterialMap = new Dictionary<string, Material>();

        public void ActivateVariantName(string name)
        {
            HRes.ActivateVariantName(name);
        }

        #region AB
        //异步加载AB
        public void LoadABAsync(string abName, Action<AssetBundle> callback)
        {
            HAssetBundle.LoadAsync(abName, callback);
        }

        //协程加载AB
        public AsyncRequest LoadABRequest(string abName)
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
        //同步加载text
        public TextAsset LoadText(string abName, string assetName)
        {
            return HText.Load(abName, assetName);
        }

        //异步加载text
        public void LoadTextAsync(string abName, string assetName, Action<TextAsset> callback)
        {
            HText.LoadAsync(abName, assetName, callback);
        }

        //协程加载text
        public AsyncRequest LoadTextCoRequest(string abName, string assetName)
        {
            return HText.LoadCoRequest(abName, assetName);
        }
        #endregion

        #region prefab
        //加载prefab同步
        public GameObject LoadPrefab(string abName, string assetName)
        {
            return HPrefab.Load(abName, assetName, false);
        }

        //加载prefab异步
        public void LoadPrefabAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            HPrefab.LoadAsync(abName, assetName, false, callback, args);
        }

        //加载prefab协程
        public AsyncRequest LoadPrefabCoRequest(string abName, string assetName)
        {
            return HPrefab.LoadCoRequest(abName, assetName, false);
        }

        //预加载prefab同步(返回原始prefab,不实例)
        public GameObject PreLoadPrefab(string abName, string assetName)
        {
            return HPrefab.Load(abName, assetName, true);
        }

        //预加载prefab异步(返回原始prefab,不实例)
        public void PreLoadPrefabAsync(string abName, string assetName, Action<GameObject, object[]> callback, params object[] args)
        {
            HPrefab.LoadAsync(abName, assetName, true, callback, args);
        }

        //预加载prefab协程(返回原始prefab,不实例)
        public AsyncRequest PreLoadPrefabCoRequest(string abName, string assetName)
        {
            return HPrefab.LoadCoRequest(abName, assetName, true);
        }
        #endregion

        #region sprite
        //异步加载图集 
        public void LoadSpriteAsync(string abName, string assetName, Action<Sprite> callback)
        {
            HSprite.LoadAsync(abName, assetName, callback);
        }

        //协程加载图集
        public AsyncRequest LoadSpriteCoRequest(string abName, string assetName)
        {
            return HSprite.LoadCoRequest(abName, assetName);
        }

        //同步加载图集
        public Sprite LoadSprite(string abName, string assetName)
        {
            return HSprite.Load(abName, assetName);
        }
        #endregion

        #region sprite atlas
        //异步加载图集 
        public void LoadSpriteAtlasAsync(string abName, Action<List<Sprite>> callback)
        {
            HSpriteAtlas.LoadAsync(abName, callback);
        }

        //协程加载图集
        public AsyncRequest LoadSpriteAtlasCoRequest(string abName)
        {
            return HSpriteAtlas.LoadCoRequest(abName);
        }

        //同步加载图集
        public List<Sprite> LoadSpriteAtlas(string abName)
        {
            return HSpriteAtlas.Load(abName);
        }

        #endregion

        #region texture
        //异步加载贴图
        public void LoadTextureAsync(string abName, string assetName, Action<Texture> callback)
        {
            HTexture.LoadAsync(abName, assetName, callback);
        }

        //协程加载贴图
        public AsyncRequest LoadTextureCoRequest(string abName, string assetName)
        {
            return HTexture.LoadCoRequest(abName, assetName);
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
        public AsyncRequest LoadAudioClipCoRequest(string abName, string assetName)
        {
            return HAudioCilp.LoadCoRequest(abName, assetName);
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
        public AsyncRequest LoadMaterialCoRequest(string abName, string assetName)
        {
            return HMaterial.LoadCoRequest(abName, assetName);
        }

        //同步加载材质
        public Material LoadMaterial(string abName, string assetName)
        {
            return HMaterial.Load(abName, assetName);
        }

        //shaderName是shader内部的名字
        public Material GetMaterial(string shaderName)
        {
            if(!mMaterialMap.ContainsKey(shaderName))
            {
                Shader shader = LoadShader("allshader", shaderName);
                Material material = new Material(shader);
                mMaterialMap[shaderName] = material;
            }

            return mMaterialMap[shaderName];
        }
        #endregion

        #region Shader
        //异步加载shader
        public void LoadShaderAsync(string abName, string assetName, Action<Shader> callback)
        {
            HShader.LoadAsync(abName, assetName, callback);
        }

        //协程加载shader
        public AsyncRequest LoadShaderCoRequest(string abName, string assetName)
        {
            return HShader.LoadCoRequest(abName, assetName);
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
        public AsyncRequest LoadLuaCoRequest(string abName, string assetName)
        {
            return HLua.LoadCoRequest(abName, assetName);
        }

        //同步加载lua
        public TextAsset LoadLua(string abName, string assetName)
        {
            return HLua.Load(abName, assetName);
        }
        #endregion

        #region Font
        //异步加载字体
        public void LoadFontAsync(string abName, string assetName, Action<TMP_FontAsset> callback)
        {
            HFont.LoadAsync(abName, assetName, callback);
        }

        //协程加载贴图
        public AsyncRequest LoadFontCoRequest(string abName, string assetName)
        {
            return HFont.LoadCoRequest(abName, assetName);
        }

        //同步加载音频
        public TMP_FontAsset LoadFont(string abName, string assetName)
        {
            return HFont.Load(abName, assetName);
        }
        #endregion

        public void Release(string name)
        {
            Release(name, name);
        }

        public void Release(string abName, string assetName)
        {
            if (abName == null || assetName == null)
            {
                Debug.LogError("Release abName or assetName is null!!!");
                return;
            }

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

            HAssetBundle.ReleaseAssetBundleManifest();
        }

        public void LogRef()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in HRes.mResMap)
            {
                sb.AppendLine(item.Key + "/" + item.Value.RefCount);
            }
            //Debuger.WriteToFile("ResourceRef", sb.ToString());
        }
    }
}
