using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HText : HRes
    {
        private byte[] mBytes;
    
        public HText()
        {
        }
    
        public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
        {
            //base.Load(abName, assetName success, error);
            //ResourceManager.Instance.StartCoroutine(LoadInternal(success, error));
        }
    
        private IEnumerator LoadInternal(Action<byte[]> success, Action error)
        {
            WWW www = new WWW(PathManager.URL(mAssetName, AssetType.eText));
            yield return www;
    
            if(mBytes == null)
            {
                mBytes = www.bytes;
            }
    
            if (string.IsNullOrEmpty(www.error))
            {
                if (success != null)
                {
                    success(mBytes);
                }
            }
            else
            {
                if (error != null)
                {
                    error();
                }
            }
        }
    
        public override void Release()
        {
            base.Release();
            mBytes = null;
        }
    }
}
