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
            private Shader mShader;

            public HShader(string abName, string assetName) : base(abName, assetName, AssetType.eShader)
            {
            }

            public override void Load<T>(string abName, string assetName, Action<T> success, Action error)
            {
                base.Load(abName, assetName, success, error);
                ABRequest abRequest = new ABRequest();
                abRequest.Load(abName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load<T>(ABRequest abRequest, Action<T> success, Action error) where T : UnityEngine.Object
            {
                yield return abRequest;
                if(mShader == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mShader = assetRequest.GetAssets<Shader>(mAssetName);
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
}
