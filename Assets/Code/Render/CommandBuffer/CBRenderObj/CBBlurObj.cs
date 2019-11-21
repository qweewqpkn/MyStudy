using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBBlurObj : MonoBehaviour {

    public Material mMaterial;
    private RenderTexture mRenderTexture;

    private void OnPreRender()
    {
        mRenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        Camera.main.targetTexture = mRenderTexture;
    }

    private void OnPostRender()
    {
        Camera.main.targetTexture = null;
        RenderTexture source = mRenderTexture;
        RenderTexture tempRT = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, tempRT, mMaterial);
        Graphics.Blit(tempRT, null as RenderTexture, mMaterial);
        RenderTexture.ReleaseTemporary(tempRT);
        RenderTexture.ReleaseTemporary(mRenderTexture);
    }
}
