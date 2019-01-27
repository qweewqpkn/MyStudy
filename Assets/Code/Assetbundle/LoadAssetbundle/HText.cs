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
            private Action<byte[]> mSuccess;
            private Action mError;
            private byte[] mBytes;

            public HText(string assetName) : base(assetName, assetName)
            {

            }

            public override void Load(Action<byte[]> success, Action error)
            {
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                WWW www = new WWW(ResourceManager.Instance.URL(mAssetName, AssetType.eText));
                yield return www;

                if(mBytes == null)
                {
                    mBytes = www.bytes;
                }

                if (string.IsNullOrEmpty(www.error))
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mBytes);
                    }
                }
                else
                {
                    if (mError != null)
                    {
                        mError();
                    }
                }

                mSuccess = null;
                mError = null;
            }
        }
    }
}
