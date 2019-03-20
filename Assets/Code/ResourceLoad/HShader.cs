﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetLoad
{
    class HShader : HRes
    {
        private Shader mShader;

        public HShader()
        {
        }

        protected override IEnumerator LoadAsset<T>(AssetBundle ab, string assetName, Action<T> success, Action error)
        {
            if(mShader == null)
            {
                AssetRequest assetRequest = new AssetRequest(ab, assetName);
                yield return assetRequest;
                mShader = assetRequest.GetAssets<Shader>(assetName);
            }

            if (mShader != null)
            {
                if (success != null)
                {
                    success(mShader as T);
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
            mShader = null;
        }
    }
}
