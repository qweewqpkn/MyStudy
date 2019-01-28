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
            private ABRequest mABRequest;
            private AssetRequest mAssetRequest;
            private Action<Shader[]> mSuccess;
            private Action mError;

            public HShader(string abName) : base(abName, "")
            {
            }

            public override void Load(Action<Shader[]> success, Action error)
            {
                mABRequest = new ABRequest(mAllABList);
                mSuccess += success;
                mError += error;
                ResourceManager.Instance.StartCoroutine(Load());
            }

            private IEnumerator Load()
            {
                yield return mABRequest;          
                AssetBundle ab = mABRequest.GetAB();
                mAssetRequest = new AssetRequest(ab, "", true);
                yield return mAssetRequest;
                UnityEngine.Object[] objs = mAssetRequest.GetAssets();
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

                mSuccess = null;
                mError = null;
            }
        }
    }
}
