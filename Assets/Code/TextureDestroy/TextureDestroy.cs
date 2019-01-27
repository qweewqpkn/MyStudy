using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureDestroy : MonoBehaviour {

    public RawImage image;
    public Button button;
    public Button button1;
    private Texture texture;

	// Use this for initialization
	void Start () {

        ResourceManager.Instance.mInitComplete = () =>
        {
            button.onClick.AddListener(() =>
            {
                //使用Destroy是无法释放掉texture的,而且这里还会报错Destroying assets is not permitted to avoid data loss.
                //Destroy(texture);
                //使用这个可以卸载，从profile中看到，但是他把ab中的资源也卸载了，导致无法再次从ab中加载出来
                //DestroyImmediate(texture, true);
                //使用这个可以正常卸载掉资源,并且也能再次从AB中加载回来
                //Resources.UnloadAsset(texture);
                texture = null;
                image.texture = null;
            });

            button1.onClick.AddListener(() =>
            {
                ResourceManager.Instance.LoadTexture("texture", "texture", (tex) =>
                {
                    texture = tex;
                    image.texture = tex;
                });
            });
        };
    }
}
