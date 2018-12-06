using System;
using System.Collections;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAssetBundle : HRes
        {
            private Action mSuccess;
            private Action mError;

            public HAssetBundle(string abName, Action success, Action error)
            {
                mABName = abName;
                mSuccess = success;
                mError = error;
                ResourceManager.Instance.StartCoroutine(Load(new ABAssetLoadRequest(this)));
            }

            IEnumerator Load(ABAssetLoadRequest request)
            {
                yield return request;
                if (mSuccess != null)
                {
                    mSuccess();
                }
            }
        }
    }
}
