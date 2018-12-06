using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HText : HRes
        {
            private AssetLoadRequest mRequest;
            private Action<byte[]> mSuccess;
            private Action mError;

            public HText(string assetName, Action<byte[]> success, Action error)
            {
                mAssetName = assetName;
                mRequest = new AssetLoadRequest(this);
                mSuccess = success;
                mError = error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            IEnumerator Load()
            {
                yield return mRequest;
                byte[] bytes = mRequest.GetText();
                if (bytes != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(bytes);
                    }
                }
                else
                {
                    if (mError != null)
                    {
                        mError();
                    }
                }
            }
        }
    }
}
