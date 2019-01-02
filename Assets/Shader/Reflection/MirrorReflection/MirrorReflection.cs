using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorReflection : MonoBehaviour {

    //原摄像机
    public Camera mCamera;
    //材质
    public List<Renderer> mReflectMaterialList = new List<Renderer>();
    //投影平面的偏移
    public float mPlaneOffset = 0.0f;
    //投影平面trans
    public Transform mPlaneTrans;
    //RT的宽和高
    public int mRTWidth = 256;
    public int mRTHeight = 256;
    //反射显示的摄像机层
    public LayerMask mLayerMask;
    //灯光位置（用于调整高光的位置,和反射无关）
    public Transform mLightPos;

    private Vector3 mPlaneNormal = new Vector3(0.0f, 1.0f, 0.0f);
    private RenderTexture mReflectRT;
    private Camera mReflectCamera;

    //1.一定要在OnWillRenderObject中调用GL.invertCulling 而不能再update中调用，否则不能正确反转剔除
    //2.OnWillRenderObject 需要物体在摄像机视野范围内才能被调用，所以这个脚本挂载的物体要在视野范围内
    private void OnWillRenderObject()
    {
        //生成反射camera
        UpdateReflectCamera(ref mReflectCamera);
        //设置RT
        UpdateReflectRT(ref mReflectRT);

        //渲染
        GL.invertCulling = true;
        mReflectCamera.Render();
        GL.invertCulling = false;
    }

    void UpdateReflectCamera(ref Camera reflectCamera)
    {
        if(reflectCamera == null)
        {
            GameObject reflectObj = new GameObject();
            reflectObj.name = "Reflect Camera";
            reflectCamera = reflectObj.AddComponent<Camera>();
        }

        //定义平面
        Vector3 mPlanePosition = mPlaneTrans.position;
        float D = -Vector3.Dot(mPlaneNormal, mPlanePosition + mPlaneNormal * mPlaneOffset);
        Vector4 plane = new Vector4(mPlaneNormal.x, mPlaneNormal.y, mPlaneNormal.z, D);

        //计算反射矩阵
        Matrix4x4 reflectMat = Matrix4x4.zero;
        CalcReflectMatrix(ref reflectMat, plane);
        reflectCamera.clearFlags = CameraClearFlags.Skybox;
        reflectCamera.backgroundColor = Color.white;
        reflectCamera.depth = 0;
        reflectCamera.fieldOfView = mCamera.fieldOfView;
        reflectCamera.aspect = mCamera.aspect;
        reflectCamera.cullingMask = mLayerMask;
        reflectCamera.transform.position = reflectMat.MultiplyPoint3x4(mCamera.transform.position);
        reflectCamera.transform.rotation = mCamera.transform.rotation;
        //这里会修改摄像机的视角方向,而不再使用摄像机的up，forward，right决定了
        reflectCamera.worldToCameraMatrix = mCamera.worldToCameraMatrix * reflectMat;

        //计算裁剪面
        Vector4 cameraSpaceClipPlane = CalcClipPlane(reflectCamera, mPlanePosition, mPlaneNormal, 1);
        Matrix4x4 projectionMat = mCamera.projectionMatrix;
        //计算投影矩阵
        CalcObliqueMatrix(ref projectionMat, cameraSpaceClipPlane);
        reflectCamera.projectionMatrix = projectionMat;

    }

    void UpdateReflectRT(ref RenderTexture reflectRT)
    {
        if(reflectRT == null || reflectRT.width != mRTWidth || reflectRT.height != mRTHeight)
        {
            if(reflectRT != null)
            {
                reflectRT.Release();
            }

            reflectRT = new RenderTexture(mRTWidth, mRTHeight, 16);
            reflectRT.wrapMode = TextureWrapMode.Clamp;
            reflectRT.isPowerOfTwo = true;
        }

        mReflectCamera.targetTexture = reflectRT;

        for(int i = 0; i < mReflectMaterialList.Count; i++)
        {
            if(mReflectMaterialList[i] != null)
            {
                for (int j = 0; j < mReflectMaterialList[i].materials.Length; j++)
                {
                    mReflectMaterialList[i].materials[j].SetTexture("_ReflectTex", reflectRT);
                    mReflectMaterialList[i].materials[j].SetVector("_LightPos", mLightPos.transform.forward);
                }
            }
        }
    }
    
	//计算摄像机空间下的指定裁剪面
    Vector4 CalcClipPlane(Camera camera, Vector3 pos, Vector3 normal, float front)
    {
        pos = pos + normal * mPlaneOffset;
        Vector3 cpos = camera.worldToCameraMatrix.MultiplyPoint(pos);
        Vector3 cnormal = camera.worldToCameraMatrix.MultiplyVector(normal);
        Vector4 plane = new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        return plane;
    }

    private static float sgn(float a)
    {
        if (a > 0.0f)
            return 1.0f;
        if (a < 0.0f)
            return -1.0f;
        return 0.0f;
    }

    //计算指定裁剪面的投影矩阵
    private void CalcObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(sgn(clipPlane.x),sgn(clipPlane.y),1.0f,1.0f);
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    //计算反射矩阵
    //plane(Nx,Ny,Nz,D)
    void CalcReflectMatrix(ref Matrix4x4 matrix, Vector4 plane)
    {
        matrix.m00 = (1F - 2F * plane[0] * plane[0]);
        matrix.m01 = (-2F * plane[0] * plane[1]);
        matrix.m02 = (-2F * plane[0] * plane[2]);
        matrix.m03 = (-2F * plane[3] * plane[0]);

        matrix.m10 = (-2F * plane[1] * plane[0]);
        matrix.m11 = (1F - 2F * plane[1] * plane[1]);
        matrix.m12 = (-2F * plane[1] * plane[2]);
        matrix.m13 = (-2F * plane[3] * plane[1]);

        matrix.m20 = (-2F * plane[2] * plane[0]);
        matrix.m21 = (-2F * plane[2] * plane[1]);
        matrix.m22 = (1F - 2F * plane[2] * plane[2]);
        matrix.m23 = (-2F * plane[3] * plane[2]);

        matrix.m30 = 0F;
        matrix.m31 = 0F;
        matrix.m32 = 0F;
        matrix.m33 = 1F;
    }
}
