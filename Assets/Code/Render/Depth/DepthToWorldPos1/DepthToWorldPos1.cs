using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthToWorldPos1 : MonoBehaviour {
    public Camera mCamera;
    public Material mMaterial;

	// Use this for initialization
	void Start () {
        mCamera.depthTextureMode = DepthTextureMode.Depth;
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Matrix4x4 projMatrix = GL.GetGPUProjectionMatrix(mCamera.projectionMatrix, false);
        //projMatrix = projMatrix * mCamera.worldToCameraMatrix;
        mMaterial.SetMatrix("_ProjMatInv",  projMatrix.inverse);
        Graphics.Blit(source, destination, mMaterial);
    }
}
