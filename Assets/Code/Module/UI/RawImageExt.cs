using AssetLoad;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RawImageExt : RawImage
{
    private string mTextureName;
    private bool mGrayState;
    private Material mGrayMaterial;
    private Texture2D mTexture2D;

    public void SetTexture(string name, Action callBack = null)
    {
        if(name == mTextureName)
        {
            return;
        }
        else
        {
            if(!string.IsNullOrEmpty(mTextureName))
            {
                ResourceManager.Instance.Release(mTextureName);
            }

            mTextureName = name;
            ResourceManager.Instance.LoadTextureAsync(name, name,
            (res) =>
            {
                if(res == null)
                {
                    return;
                }

                if(this == null || IsDestroyed())
                {
                    ResourceManager.Instance.Release(mTextureName);
                    return;
                }

                texture = res;
                gameObject.SetActive(true);
                if(callBack != null)
                {
                    callBack();
                }
            });
        }
    }

    public void SetTextureBytes(string name, byte[] bytes)
    {
        if(mTexture2D == null)
        {
            mTexture2D = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        }

        mTexture2D.LoadRawTextureData(bytes);
        texture = mTexture2D;
    }

    public void SetGray(bool isGray)
    {
        if (mGrayState == isGray)
            return;
        if (isGray)
        {
            if (null == mGrayMaterial)
            {
                mGrayMaterial = ResourceManager.Instance.GetMaterial("Custom/UI/UIGray");
            }
            material = mGrayMaterial;
        }
        else
        {
            material = null;
        }

        mGrayState = isGray;
    }

    protected override void OnDestroy()
    {
        if(!string.IsNullOrEmpty(mTextureName))
        {
            ResourceManager.Instance.Release(mTextureName);
        }

        if(mTexture2D != null) 
        {
            Destroy(mTexture2D);
        }
    }
}