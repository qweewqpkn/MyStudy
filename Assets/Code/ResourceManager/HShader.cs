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

            public HShader(string abName, string assetName) : base(abName, assetName)
            {
            }

            public override void Load(Action<Shader> success, Action error)
            {
                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, success, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<Shader> success, Action error)
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
                        success(mShader);
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
