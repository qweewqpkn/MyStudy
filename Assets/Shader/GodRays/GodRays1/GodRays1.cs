using UnityEngine;
using System.Collections;

public class GodRays1 : MonoBehaviour {

    public Material mMaterial;
    public Camera mCamera;
    public int blurCount = 1;

	// Use this for initialization
	void Start () {
        mCamera = GetComponent<Camera>();
        mCamera.depthTextureMode = DepthTextureMode.Depth;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnRenderImage(RenderTexture src, RenderTexture des)
    {
        mMaterial.SetVector("lightScreenPos", new Vector4(0.5f, 0.7f, 0.0f, 0.0f));

        RenderTexture tempA = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
        Graphics.Blit(src, tempA, mMaterial, 0);

        for (int i = 0; i < blurCount; i++)
        {
            RenderTexture tempB = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
            Graphics.Blit(tempA, tempB, mMaterial, 1);
            tempA.Release();
            tempA = RenderTexture.GetTemporary(src.width / 4, src.height / 4);
            Graphics.Blit(tempB, tempA, mMaterial, 1);
            tempB.Release();
        }

        mMaterial.SetTexture("_BlendTex", tempA);
        Graphics.Blit(src, des, mMaterial, 2);
        tempA.Release();
    }
}
