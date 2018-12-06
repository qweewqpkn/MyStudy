using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM_Sphere : MonoBehaviour {

    public Material mMaterial;
    public Camera mCamera;
    public Vector3 TR;
    public Vector3 TL;
    public Vector3 BR;
    public Vector3 BL;

    // Use this for initialization
    void Start () {
        float fov = mCamera.fieldOfView;
        float halfHeight = mCamera.nearClipPlane * Mathf.Tan(fov * Mathf.Deg2Rad / 2);
        float halfWidth = halfHeight * mCamera.aspect;
        Vector3 halfUp = mCamera.transform.up * halfHeight;
        Vector3 halfRight = mCamera.transform.right * halfWidth;
        TR = mCamera.transform.forward * mCamera.nearClipPlane + halfRight + halfUp;
        TL = mCamera.transform.forward * mCamera.nearClipPlane - halfRight + halfUp;
        BR = mCamera.transform.forward * mCamera.nearClipPlane + halfRight - halfUp;
        BL = mCamera.transform.forward * mCamera.nearClipPlane - halfRight - halfUp;

        Matrix4x4 frustumCorners = Matrix4x4.identity;
        frustumCorners.SetRow(0, BL);
        frustumCorners.SetRow(1, BR);
        frustumCorners.SetRow(2, TL);
        frustumCorners.SetRow(3, TR);
        mMaterial.SetMatrix("_FrustumCornerRay", frustumCorners);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mCamera.transform.position, TR * 10);
        Gizmos.DrawRay(mCamera.transform.position, TL * 10);
        Gizmos.DrawRay(mCamera.transform.position, BR * 10);
        Gizmos.DrawRay(mCamera.transform.position, BL * 10);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Start();
        Graphics.Blit(source, destination, mMaterial);
    }
}
