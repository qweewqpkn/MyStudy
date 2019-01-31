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
        class HShader : HRes
        {
            private ABRequest mABRequest = new ABRequest();
            private AssetRequest mAssetRequest;
            private Action<Shader> mSuccess;
            private Action mError;
            private Shader mShader;

            public HShader(string abName, string assetName) : base(abName, assetName)
            {
            }

            public override void Load(Action<Shader> success, Action error)
            {
                mABRequest.Load(mABName, mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;
                if(mShader == null)
                {
                    mAssetRequest = new AssetRequest(mABRequest.mainAB, mAssetName, false);
                    yield return mAssetRequest;
                    mShader = mAssetRequest.GetAssets<Shader>(mAssetName);
                }

                if (mShader != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(mShader);
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
