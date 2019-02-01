using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetLoad
{
    public partial class ResourceManager
    {
        class HSprite : HRes
        {
            private Sprite mSprite;

            public HSprite(string abName, string assestName) : base(abName, assestName)
            {
            }

            public override void Load(Action<Sprite> success, Action error)
            {
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<Sprite> success, Action error)
            {
                yield return abRequest;
                if(mSprite == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mSprite = assetRequest.GetAssets<Sprite>(mAssetName);
                }
                if (mSprite != null)
                {
                    if (success != null)
                    {
                        success(mSprite);
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
        }
    }
}
