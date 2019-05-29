using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using AssetLoad;

public class ImageExt : Image
{
	private string mABName = "";
    private string mSpriteName = "";
	private bool mGrayState = false;
	public Material mGrayMaterial = null;

	public void SetSprite(string abName,string spriteName)
	{
		if (!string.IsNullOrEmpty (mABName)) {
			ResourceManager.Instance.Release (mABName, mSpriteName);
		}
        mABName = abName;
        mSpriteName = spriteName;
        ResourceManager.Instance.LoadSpriteAsync (mABName, mSpriteName,  (sprite) =>{
            this.sprite = sprite;
		});
	}

    protected override void OnDestroy()
    {
        base.OnDestroy();
		if (null != mABName && !string.IsNullOrEmpty (mABName)) {
			ResourceManager.Instance.Release(mABName, mSpriteName);
        }
    }

    //public void SetUrlSprite(string url)
    //{
    //    if (gameObject.activeSelf && gameObject.activeInHierarchy)
    //    {
    //        //内存中是否有
    //        if (ResourceManager.Instance.mDicUrlSprite.ContainsKey(url))
    //        {
    //            Texture2D texture = ResourceManager.Instance.mDicUrlSprite[url];
    //            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
    //            sprite = newSprite;
    //        }
    //        //本地是否有
    //        else if(ResourceManager.Instance.mDicUrlSpriteLocal.ContainsKey(url))
    //        {
    //            string local_url = ResourceManager.Instance.mDicUrlSpriteLocal[url];
    //            StartCoroutine(LoadUrlSprite(local_url, true));
    //        }
    //        //从网络加载
    //        else
    //        {
    //            StartCoroutine(LoadUrlSprite(url));
    //        }
    //    }
    //}

    //private IEnumerator LoadUrlSprite(string url, bool is_local = false)
    //{
    //    Debug.Log("#### LoadUrlSprite is call url:" + url);
    //    WWW www = new WWW(url);
    //    yield return www;
    //    
    //    if(string.IsNullOrEmpty(www.error))
    //    {
    //        Debug.Log("#### LoadUrlSprite is call www success ");
    //        if(IsGif(www.bytes))
    //        {
    //            Debug.Log("#### LoadUrlSprite is call texture isn ull");
    //            yield return UniGif.GetTextureListCoroutine(www.bytes, (list, count, width, height)=>
    //            {
    //                if(list.Count > 0)
    //                {
    //                    Debug.Log("#### LoadUrlSprite is call GetTextureListCoroutine list.Count");
    //                    ResourceManager.Instance.mDicUrlSprite[url] = list[0].m_texture2d;
    //
    //                    if(!is_local)
    //                        SaveLocal(url, list[0].m_texture2d);
    //
    //                    Sprite newSprite = Sprite.Create(list[0].m_texture2d, new Rect(0.0f, 0.0f, width, height), Vector2.zero);
    //                    sprite = newSprite;
    //                }
    //            });
    //        }
    //        else
    //        {
    //            Texture2D texture = www.texture;
    //            ResourceManager.Instance.mDicUrlSprite[url] = www.texture;
    //
    //            if (!is_local)
    //                SaveLocal(url, www.texture);
    //
    //            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
    //            sprite = newSprite;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("other", "#### LoadUrlSprite is call www error : " + www.error);
    //    }
    //}

    //public void SaveLocal(string url, Texture2D texture)
    //{
    //    string save_name = url.Substring(url.LastIndexOf("/"));
    //    string save_path = Application.persistentDataPath + "/UrlImage" + save_name;
    //
    //    Debug.Log("############save_path=" + save_path);
    //    byte[] data = texture.EncodeToPNG();
    //    File.WriteAllBytes(save_path, data);
    //
    //    ResourceManager.Instance.mDicUrlSpriteLocal[url] = "file:///" + save_path;
    //
    //    ResourceManager.Instance.SaveAllLocalUrlImage();
    //}

    public bool IsGif(byte[] content)
    {
        if (content[0] != 'G' || content[1] != 'I' || content[2] != 'F')
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 灰度
    /// </summary>
    /// <param name="isGray">If set to <c>true</c> is gray.</param>
    /// <param name="raycast">If set to <c>true</c> raycast.</param>
    public void SetGray(bool isGray, bool raycast = false)
	{
		if (mGrayState == isGray)
			return;
		if (isGray) {
			if (null == mGrayMaterial) {
                mGrayMaterial = ResourceManager.Instance.GetMaterial("Custom/UI/UIGray");
			}
			material = mGrayMaterial;
			if (raycast) {
				raycastTarget = false;
			}
		} else {
			material = null;
			if (raycast) {
				raycastTarget = true;
			}
		}
		mGrayState = isGray;
	}
}

