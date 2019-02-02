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
        class HMaterial : HRes
        {
            private Material mMaterial;

            public HMaterial(string abName, string assestName) : base(abName, assestName)
            {
                string name = string.Format("{0}/{1}", abName, assestName);
                ResourceManager.Instance.mResMap.Add(name, this);
            }

            public override void Load<T>(Action<T> success, Action error)
            {
                base.Load(success, error);
                Action<Material> complete = (ab) =>
                {
                    success(ab as T);
                };

                ABRequest abRequest = new ABRequest();
                abRequest.Load(mABName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(abRequest, complete, error));
            }

            private IEnumerator Load(ABRequest abRequest, Action<Material> success, Action error)
            {
                yield return abRequest;
                if (mMaterial == null)
                {
                    AssetRequest assetRequest = new AssetRequest(abRequest.mAB, mAssetName);
                    yield return assetRequest;
                    mMaterial = assetRequest.GetAssets<Material>(mAssetName);
                }

                if (mMaterial != null)
                {
                    if (success != null)
                    {
                        success(mMaterial);
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
                mMaterial = null;
            }
        }
    }
}
