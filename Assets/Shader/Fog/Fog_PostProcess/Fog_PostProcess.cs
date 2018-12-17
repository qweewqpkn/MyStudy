using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog_PostProcess : MonoBehaviour {

    public Material mMaterial;
    public Color mFogColor;
    public float mFogStart;
    public float mFogEnd;
    private Camera mCamera;


    void Start()
    {
        mCamera = GetComponent<Camera>();
        if(mCamera == null)
        {
            Debug.LogError("请将脚本挂在摄像机上");
            return;
        }

        mCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mMaterial.SetFloat("_FogStart", mFogStart);
        mMaterial.SetFloat("_FogEnd", mFogEnd);
        mMaterial.SetColor("_FogColor", mFogColor);
        mMaterial.SetMatrix("_ProjectionInverseMatrix", mCamera.projectionMatrix.inverse);
        Graphics.Blit(source, destination, mMaterial);
    }
}
