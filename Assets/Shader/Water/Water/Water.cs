using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//思路：选定一个反射平面后，生成对称的摄像机，然后这个摄像机会看见需要反射的物体(使用cullingmask来决定),看见的物体被渲染到rendertexture中，
//然后我们将会把这张rendertexture设置到反射平面的材质中，然后反射平面的材质shader会使用屏幕坐标来采样这张rendertexture,显示反射图像。
//1.一定要在OnWillRenderObject中调用GL.invertCulling 而不能再update中调用，否则不能正确反转剔除
//2.OnWillRenderObject 需要物体在摄像机视野范围内才能被调用，所以这个脚本挂载的物体要在视野范围内
public class Water : MonoBehaviour
{
    public enum WaterMode
    {
        eAll,
        eRefract,
        eReflect,
    }
        

    //原摄像机
    public Camera mCamera;
    //反射面的render列表(列表中的render的材质将会被设置反射贴图，将会显示反射图)
    public List<Renderer> mReflectList = new List<Renderer>();
    //投影平面的偏移
    public float mPlaneOffset = 0.0f;
    //投影平面trans(用于指定投影平面的位置)
    public Transform mPlaneTrans;
    //RT的宽和高(它的大小将决定反射的清晰度)
    public int mRTWidth = 256;
    public int mRTHeight = 256;
    //反射摄像机的显示层设置，用于设置那些物体是需要显示在反射投影中的
    public LayerMask mReflectLayerMask;
    //折射摄像机显示哪些层的设置
    public LayerMask mRefractLayerMask;
    //灯光位置（用于调整高光的位置,和反射无关）
    public Transform mLightPos;
    //水模式
    public WaterMode mWaterMode = WaterMode.eAll;

    private Vector3 mPlaneNormal = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector4 mPlane = Vector4.zero;
    private RenderTexture mReflectRT;
    private RenderTexture mRefractRT;
    private Camera mRenderCamera;

    public void Start()
    {
        InitCamera();
    }

    private void InitCamera()
    {
        if (mRenderCamera == null)
        {
            GameObject cameraObj = new GameObject();
            cameraObj.name = "Render Camera";
            mRenderCamera = cameraObj.AddComponent<Camera>();
            mRenderCamera.clearFlags = CameraClearFlags.Skybox;
            mRenderCamera.backgroundColor = Color.white;
            mRenderCamera.depth = 0;
            mRenderCamera.fieldOfView = mCamera.fieldOfView;
            mRenderCamera.aspect = mCamera.aspect;
        }
    }

    private void InitRT()
    {
        if (mReflectRT == null)
        {
            mReflectRT = new RenderTexture(mRTWidth, mRTHeight, 16);
            mReflectRT.wrapMode = TextureWrapMode.Clamp;
            mReflectRT.isPowerOfTwo = true;
        }

        if(mRefractRT == null)
        {
            mRefractRT = new RenderTexture(mRTWidth, mRTHeight, 16);
            mRefractRT.wrapMode = TextureWrapMode.Clamp;
            mRefractRT.isPowerOfTwo = true;
        }

        for (int i = 0; i < mReflectList.Count; i++)
        {
            if (mReflectList[i] != null)
            {
                for (int j = 0; j < mReflectList[i].sharedMaterials.Length; j++)
                {
                    if (mReflectList[i].sharedMaterials[j] != null)
                    {
                        mReflectList[i].sharedMaterials[j].SetTexture("_ReflectTex", mReflectRT);
                        mReflectList[i].sharedMaterials[j].SetTexture("_RefractTex", mRefractRT);
                        //mReflectList[i].materials[j].SetVector("_LightPos", mLightPos.transform.forward);
                    }
                }
            }
        }
    }

    private void OnWillRenderObject()
    {
        InitRT();

        //更新平面
        UpdatePlane();

        //更新反射camera
        if(mWaterMode == WaterMode.eAll || mWaterMode == WaterMode.eReflect)
        {
            UpdateReflectCamera();
        }

        //更新折射camera
        if (mWaterMode == WaterMode.eAll || mWaterMode == WaterMode.eRefract)
        {
            UpdateRefractCamera();
        }
    }

    void UpdatePlane()
    {
        //定义平面
        float D = -Vector3.Dot(mPlaneNormal, mPlaneTrans.position + mPlaneNormal * mPlaneOffset);
        mPlane = new Vector4(mPlaneNormal.x, mPlaneNormal.y, mPlaneNormal.z, D);
    }

    void UpdateReflectCamera()
    {
        //计算反射矩阵
        Matrix4x4 reflectMat = Matrix4x4.zero;
        CalcReflectMatrix(ref reflectMat, mPlane);
        mRenderCamera.cullingMask = mReflectLayerMask;
        mRenderCamera.transform.position = reflectMat.MultiplyPoint3x4(mCamera.transform.position);
        mRenderCamera.transform.rotation = mCamera.transform.rotation;
        mRenderCamera.targetTexture = mReflectRT;
        //这里会修改摄像机的视角方向,而不再使用摄像机的up，forward，right决定了
        mRenderCamera.worldToCameraMatrix = mCamera.worldToCameraMatrix * reflectMat;

        //计算裁剪面
        Vector4 cameraSpaceClipPlane = CalcClipPlane(mRenderCamera, mPlaneTrans.position, mPlaneNormal, 1);
        Matrix4x4 projectionMat = mCamera.projectionMatrix;
        //计算投影矩阵
        CalcObliqueMatrix(ref projectionMat, cameraSpaceClipPlane);
        mRenderCamera.projectionMatrix = projectionMat;

        //渲染
        GL.invertCulling = true;
        mRenderCamera.Render();
        GL.invertCulling = false;
    }

    void UpdateRefractCamera()
    {
        //设置摄像机位置和view矩阵
        mRenderCamera.cullingMask = mRefractLayerMask;
        mRenderCamera.transform.position = mCamera.transform.position;
        mRenderCamera.transform.rotation = mCamera.transform.rotation;
        mRenderCamera.targetTexture = mRefractRT;
        //这里会修改摄像机的视角方向,而不再使用摄像机的up，forward，right决定了
        mRenderCamera.worldToCameraMatrix = mCamera.worldToCameraMatrix;

        //计算裁剪面
        Vector4 cameraSpaceClipPlane = CalcClipPlane(mRenderCamera, mPlaneTrans.position, mPlaneNormal, -1);
        Matrix4x4 projectionMat = mCamera.projectionMatrix;
        //计算投影矩阵
        CalcObliqueMatrix(ref projectionMat, cameraSpaceClipPlane);
        mRenderCamera.projectionMatrix = projectionMat;

        //渲染
        mRenderCamera.Render();
    }

    //计算摄像机空间下的指定裁剪面
    Vector4 CalcClipPlane(Camera camera, Vector3 pos, Vector3 normal, float front)
    {
        pos = pos + normal * mPlaneOffset;
        Vector3 cpos = camera.worldToCameraMatrix.MultiplyPoint(pos);
        Vector3 cnormal = camera.worldToCameraMatrix.MultiplyVector(normal).normalized * front;
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
        Vector4 q = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1.0f, 1.0f);
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

