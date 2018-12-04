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
            private string mABName;
            private Action<Shader[]> mSuccess;
            private Action mError;
            private ABLoadRequest mRequest;

            public HShader(string abName, Action<Shader[]> success, Action error)
            {
                mABName = abName;
                mSuccess = success;
                mError = error;
                mRequest = new ABLoadRequest(abName, abName);
                ResourceManager.Instance.StartCoroutine(Load());
            }

            IEnumerator Load()
            {
                yield return mRequest;
                UnityEngine.Object[] objs = mRequest.GetAssets();
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
