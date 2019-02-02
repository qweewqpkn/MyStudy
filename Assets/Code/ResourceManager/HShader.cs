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

            public override void Load<T>(Action<T> success, Action error)
            {
                base.Load(success, error);
                Action<Shader> complete = (ab) =>
                {
                    success(ab as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, complete, error));
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

            public override void Release()
            {
                base.Release();
                mShader = null;
            }
        }
    }
}
