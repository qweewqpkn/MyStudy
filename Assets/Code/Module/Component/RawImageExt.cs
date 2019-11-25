using AssetLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageExt : RawImage
{
    private string mTextureName;
    private List<string> mTextureNameList = new List<string>();
    private bool mGrayState;
    private Material mGrayMaterial;
    private Texture2D mTexture2D;

    public void SetTexture(string name, Action callBack = null)
    {
        if(name == mTextureName)
        {
            SetActive(true);
            if(callBack != null)
            {
                callBack();
            }
        }
        else
        {
            mTextureNameList.Add(name);
            mTextureName = name;
            ResourceManager.instance.LoadTextureAsync(mTextureName, mTextureName,
            (res) =>
            {
                if(res == null || this == null || IsDestroyed())
                {
                    return;
                }

                if(res.name == mTextureName)
                {
                    texture = res;
                    SetActive(true);
                    if (callBack != null)
                    {
                        callBack();
                    }
                }
            });
        }
    }

    private void SetActive(bool isShow)
    {
        if (gameObject.activeSelf != isShow)
        {
            gameObject.SetActive(isShow);
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
                mGrayMaterial = ResourceManager.instance.GetMaterial("Custom/UI/UIGray");
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
        for(int i = 0; i < mTextureNameList.Count; i++)
        {
            ResourceManager.instance.Release(mTextureNameList[i]);
        }

        if(mTexture2D != null) 
        {
            Destroy(mTexture2D);
        }
    }
}