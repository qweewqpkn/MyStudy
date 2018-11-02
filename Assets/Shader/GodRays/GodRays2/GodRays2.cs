using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRays2 : MonoBehaviour {

    public Material material;
    public float mBlurOffsetScale;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture luminanceTex = RenderTexture.GetTemporary(source.width, source.height, 16, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, luminanceTex, material, 0);


        for (int i = 0; i < 2; i++)
        {
            RenderTexture blurTex1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4,  16, RenderTextureFormat.ARGB32);
            Graphics.Blit(luminanceTex, blurTex1, material, 1);
            Graphics.Blit(blurTex1, luminanceTex, material, 1);
            RenderTexture.ReleaseTemporary(blurTex1);
        }

        material.SetTexture("_LuminanceTex", luminanceTex);
        Graphics.Blit(source, destination, material, 2);
        luminanceTex.Release();
    }
}
