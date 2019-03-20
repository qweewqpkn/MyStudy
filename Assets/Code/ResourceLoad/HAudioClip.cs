using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HAudioCilp : HRes
    {
        private AudioClip mAudioClip;

        public HAudioCilp()
        { 
        }

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            if (mAudioClip == null)
            {
                AssetRequest assetRequest = new AssetRequest(ab, assetName);
                yield return assetRequest;
                mAudioClip = assetRequest.GetAssets<AudioClip>(assetName);
            }

            if (mAudioClip != null)
            {
                if (success != null)
                {
                    success(mAudioClip as T);
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
