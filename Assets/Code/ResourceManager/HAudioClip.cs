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
        class HAudioCilp : HRes
        {
            private AudioClip mAudioClip;

            public HAudioCilp(string abName, string assetName) : base(abName, assetName, AssetType.eAudioClip)
            {
            }

            //对于反复加载同一个资源，不论ab是否已经存在，我们都要走ab请求的逻辑，为了在内部能正常进行ab的引用计数，这样才能正确释放资源。
            public override void Load<T>(Action<T> success, Action error)
            {
                base.Load(success, error);
                Action<AudioClip> complete = (ab) =>
                {
                    success(ab as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, complete, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<AudioClip> success, Action error)
            {
                yield return abRequest;
                if (mAudioClip == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mAudioClip = assetRequest.GetAssets<AudioClip>(mAssetName);
                }

                if (mAudioClip != null)
                {
                    if (success != null)
                    {
                        success(mAudioClip);
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
                mAudioClip = null;
            }
        }
    }
}
