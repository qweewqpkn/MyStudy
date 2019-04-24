using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAO : MonoBehaviour {

    public Camera mCamera;
    public Material mMaterial;
    List<Vector4> mSampleList = new List<Vector4>();

    // Use this for initialization
    void Start () {
        mCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
        mCamera.depthTextureMode |= DepthTextureMode.Depth;

        for (int i = 0; i < 64; i++)
        {
            Vector4 sample = new Vector4(
                Random.Range(0.0f, 1.0f) * 2 - 1.0f,
                Random.Range(0.0f, 1.0f) * 2 - 1.0f,
                Random.Range(0.0f, 1.0f), 0.0f);
            sample = sample.normalized;
            // sample *= Random.Range(0.0f, 1.0f);
            mSampleList.Add(sample);
        }

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mMaterial.SetMatrix("_ProjectionInverseMatrix", mCamera.projectionMatrix.inverse);
        mMaterial.SetMatrix("_ProjectionMatrix", mCamera.projectionMatrix);
        mMaterial.SetVectorArray("_Samples", mSampleList.ToArray());

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 32);
        Graphics.Blit(source, temp, mMaterial, 0);
        mMaterial.SetTexture("_SSAOTex", temp);
        Graphics.Blit(source, destination, mMaterial,1);
        RenderTexture.ReleaseTemporary(temp);
    }
}
