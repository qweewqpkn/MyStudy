using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureUtility : MonoBehaviour {

    public enum TexFormat
    {
        ePNG,
        eJPG,
    }

	public static Texture2D ConvertRenderTextureToTexture2D(RenderTexture rt, int width, int height)
    {
        RenderTexture.active = rt;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }

    public static void SaveRenderTexture(RenderTexture rt, int width, int height, TexFormat format, string path)
    {
        Texture2D texture = ConvertRenderTextureToTexture2D(rt, width, height);
        if(texture != null)
        {
            byte[] textureData = null;
            switch(format)
            {
                case TexFormat.eJPG:
                    {
                        textureData = texture.EncodeToJPG();
                    }
                    break;
                case TexFormat.ePNG:
                    {
                        textureData = texture.EncodeToPNG();
                    }
                    break;
            }

            FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(textureData);
            bw.Close();
            fs.Close();
            fs.Dispose();
        }
    }
}
