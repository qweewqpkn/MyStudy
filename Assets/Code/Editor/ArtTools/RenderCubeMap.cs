using UnityEditor;
using UnityEngine;

public class RenderCubeMap : EditorWindow {

    private static GameObject mCameraObj;
    private static Cubemap mCubeMap;
    private static int mWidth;
    private static int mHeight;

    [MenuItem("ArtTools/渲染CubeMap", false, 10)]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<RenderCubeMap>();
    }

    private void OnGUI()
    {
        mCameraObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("摄像机对象"), mCameraObj, typeof(Object), true);
        mCubeMap = (Cubemap)EditorGUILayout.ObjectField(new GUIContent("CubeMap"), mCubeMap, typeof(Object), true);
        if (GUILayout.Button("开始渲染"))
        {
            Render();
        }
    }

    private void Render()
    {
        Camera camera = mCameraObj.GetComponent<Camera>();
        if (camera != null && mCubeMap != null)
        {
            mWidth = mCubeMap.width;
            mHeight = mCubeMap.height;
            for (int m = 0; m < 6; m++)
            {
                CubemapFace face = (CubemapFace)m;
                RenderTexture texture = RenderTexture.GetTemporary(mWidth, mHeight, 24, RenderTextureFormat.ARGB32);
                Vector3 cacheEulerAngles = camera.transform.localEulerAngles;
                camera.transform.localEulerAngles += GetFaceRotate(face);
                camera.targetTexture = texture;
                camera.Render();
                RenderTexture.active = texture;
                Texture2D texture2D = new Texture2D(mWidth, mHeight, TextureFormat.ARGB32, false);
                texture2D.ReadPixels(new Rect(0, 0, mWidth, mHeight), 0, 0);

                //图像进行翻转
                Color[] colors = new Color[mWidth * mHeight];
                int index = 0;
                for (int i = mWidth - 1; i >= 0; i--)
                {
                    for (int j = mHeight - 1; j >= 0; j--)
                    {
                        colors[index] = texture2D.GetPixel(j, i);
                        index++;
                    }
                }
                mCubeMap.SetPixels(colors, face);
                camera.targetTexture = null;
                RenderTexture.ReleaseTemporary(texture);
                camera.transform.localEulerAngles = cacheEulerAngles;
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private Vector3 GetFaceRotate(CubemapFace face)
    {
        switch (face)
        {
            case CubemapFace.PositiveX:
                {
                    return new Vector3(0.0f, 90.0f, 0.0f);
                }
            case CubemapFace.PositiveY:
                {
                    return new Vector3(-90.0f, 0.0f, 0.0f);
                }
            case CubemapFace.PositiveZ:
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
            case CubemapFace.NegativeX:
                {
                    return new Vector3(0.0f, -90.0f, 0.0f);
                }
            case CubemapFace.NegativeY:
                {
                    return new Vector3(90.0f, 00.0f, 0.0f);
                }
            case CubemapFace.NegativeZ:
                {
                    return new Vector3(0.0f, 180.0f, 0.0f);
                }
        }

        return Vector3.zero;
    }
}
