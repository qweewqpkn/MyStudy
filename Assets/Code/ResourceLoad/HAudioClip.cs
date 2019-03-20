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

        protected override void OnCompleted(AssetRequest request, string assetName)
        {
            if (mAudioClip == null)
            {
                mAudioClip = request.GetAssets<AudioClip>(assetName);
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
