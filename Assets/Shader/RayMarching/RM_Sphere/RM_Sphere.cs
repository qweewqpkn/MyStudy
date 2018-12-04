using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM_Sphere : MonoBehaviour {

    public Material mMaterial;
    public Camera mCamera;

	// Use this for initialization
	void Start () {
        float fov = mCamera.fieldOfView;
        float halfHeight = mCamera.nearClipPlane * Mathf.Tan(fov * Mathf.Deg2Rad / 2);
        float halfWidth = halfHeight * mCamera.aspect;
        Vector3 halfUp = mCamera.transform.up * halfHeight;
        Vector3 halfRight = mCamera.transform.right * halfWidth;
        Vector3 TR = mCamera.transform.forward * mCamera.nearClipPlane + halfRight + halfUp;
        Vector3 TL = mCamera.transform.forward * mCamera.nearClipPlane - halfRight + halfUp;
        Vector3 BR = mCamera.transform.forward * mCamera.nearClipPlane + halfRight - halfUp;
        Vector3 BL = mCamera.transform.forward * mCamera.nearClipPlane - halfRight - halfUp;

        Matrix4x4 frustumCorners = Matrix4x4.identity;
        frustumCorners.SetRow(0, BL);
        frustumCorners.SetRow(1, BR);
        frustumCorners.SetRow(2, TL);
        frustumCorners.SetRow(3, TR);
        mMaterial.SetMatrix("_FrustumCornerRay", frustumCorners);
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Start();
        Graphics.Blit(source, destination, mMaterial);
    }
}
