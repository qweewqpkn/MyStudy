using UnityEngine;
using System.Collections;

public class AverageBlur : MonoBehaviour {

    //模糊半径  
    public float BlurRadius = 10.0f;

    public int downSample = 2;

    public int iteration = 1;

    public Material material
    {
        get
        {
            if(mMaterial == null)
            {
                mMaterial = new Material(Shader.Find("LH/AverageBlur"));
            }

            return mMaterial;
        }
    }

    public Material mMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material)
        {
            RenderTexture rt1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            RenderTexture rt2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);

            Graphics.Blit(source, rt1);

            for(int i = 0; i < iteration; i++)
            {
                material.SetFloat("_BlurRadius", BlurRadius);
                Graphics.Blit(rt1, rt2, material);
                Graphics.Blit(rt2, rt1, material);
            }

            Graphics.Blit(rt1, destination);

            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
        }
    }
}
