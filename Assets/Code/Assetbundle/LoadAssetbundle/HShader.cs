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
            private Action<Shader[]> mSuccess;
            private Action mError;

            public HShader(string abName, Action<Shader[]> success, Action error)
            {
                mABName = abName;
                mSuccess = success;
                mError = error;
                ResourceManager.Instance.StartCoroutine(Load(new ABAssetLoadRequest(this)));
            }

            IEnumerator Load(ABAssetLoadRequest request)
            {
                yield return request;
                UnityEngine.Object[] objs = request.GetAssets();
                if (objs != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(objs.Cast<Shader>().ToArray());
                    }
                }
                else
                {
                    if (mError != null)
                    {
                        mError();
                    }
                }
            }
        }
    }
}
