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
        class HText : HRes
        {
            private byte[] mBytes;

            public HText(string assetName) : base(assetName, assetName)
            {
            }

            public override void Load(Action<byte[]> success, Action error)
            {
                base.Load(success, error);
                ResourceManager.Instance.StartCoroutine(LoadInternal(success, error));
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
}
