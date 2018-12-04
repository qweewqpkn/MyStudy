using System;
using System.Collections;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAssetBundle : HRes
        {
            private string mABName;
            private Action mSuccess;
            private Action mError;
            private ABLoadRequest mRequest;

            public HAssetBundle(string abName, Action success, Action error)
            {
                mABName = abName;
                mSuccess = success;
                mError = error;
                mRequest = new ABLoadRequest(abName, "");
                ResourceManager.Instance.StartCoroutine(Load());
            }

            IEnumerator Load()
            {
                yield return mRequest;
                if (mSuccess != null)
                {
                    mSuccess();
                }
            }
        }
    }
}
