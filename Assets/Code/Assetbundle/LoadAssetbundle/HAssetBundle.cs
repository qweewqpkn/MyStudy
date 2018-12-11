using System;
using System.Collections;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HAssetBundle : HRes
        {
            private ABAssetLoadRequest mRequest;

            public HAssetBundle(string abName, Action success, Action error) : base(abName, "")
            {
                mABName = abName;
                mRequest = new ABAssetLoadRequest(abName, "", mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(success, error));
            }

            public override IEnumerator Load(Action success, Action error)
            {
                yield return mRequest;
                if (success != null)
                {
                    success();
                }
            }
        }
    }
}
