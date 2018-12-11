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
            private ABAssetLoadRequest mRequest;

            public HShader(string abName, Action<Shader[]> success, Action error) : base(abName, "")
            {
                mABName = abName;
                mSuccess = success;
                mError = error;
                mRequest = new ABAssetLoadRequest(abName, abName, mAllABList);
                ResourceManager.Instance.StartCoroutine(Load(mRequest));
            }

            IEnumerator Load(ABAssetLoadRequest request)
            {
                yield return request;
                UnityEngine.Object[] objs = request.GetAssets();
                List<Shader> shaderList = new List<Shader>();
                for(int i = 0; i < objs.Length; i++)
                {
                    Shader shader = objs[i] as Shader;
                    if(shader != null)
                    {
                        shaderList.Add(shader);
                    }
                }

                if (objs != null)
                {
                    if (mSuccess != null)
                    {
                        mSuccess(shaderList.ToArray());
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
