using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAO1 : MonoBehaviour {

    private List<Vector3> mAOSampleList = new List<Vector3>();
    private int mSampleNum = 10;
    public Material mSSAOMaterial;
    public Camera mCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	void GeneratorAOSampleKernel()
    {
        if(mAOSampleList.Count == mSampleNum)
        {
            return;
        }

        for (int i = 0; i < mSampleNum; i++)
        {
            Vector3 dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
            mAOSampleList.Add(dir);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        GeneratorAOSampleKernel();

        Matrix4x4 projMatrixInv = GL.GetGPUProjectionMatrix(mCamera.projectionMatrix, false);
        mSSAOMaterial.SetMatrix("_ProjMatrixInv", projMatrixInv.inverse);
        Graphics.Blit(source, destination, mSSAOMaterial, 0);

        //// Z buffer to linear 0..1 depth (0 at eye, 1 at far plane)
        //inline float Linear01Depth(float z)
        //{
        //    return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
        //}
        //// Z buffer to linear depth
        //inline float LinearEyeDepth(float z)
        //{
        //    return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
        //}

    }
}
