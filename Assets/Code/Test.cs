using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;
    public Image mSprite;
    public Image mSprite1;
    public AudioSource mAS;
    public GameObject mObj;
    public bool mStart;
    public int mFrame;

	// Use this for initialization
	void Start () {
        mButton.onClick.AddListener(() =>
        {
            ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
            {
                if (obj != null)
                    obj.name = "testTest1";
            });
            mStart = true;
            mFrame = 1;
            ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
            {
                if (obj != null)
                    obj.name = "testTest2";
            });
            //ResourceManager.Instance.ReleaseAll();
            ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
            {
                if (obj != null)
                    obj.name = "testTest3";
            });
        });

        ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
        {
            if(obj != null)
                obj.name = "testTest";
        });

        ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
        {
            if (obj != null)
                obj.name = "testTest1";
        });

        ResourceManager.Instance.LoadPrefab("cube", "cube", (obj) =>
        {
            if (obj != null)
                obj.name = "testTest2";
        });

        //ResourceManager.Instance.LoadTexture("texture/main_texture", "main_texture", (tex) =>
        //{
        //    mImage.texture = tex;
        //});
        //
        //ResourceManager.Instance.LoadAudioClip("sleep", "sleep", (audio) =>
        //{
        //    mAS.clip = audio;
        //    mAS.Play();
        //});
        //
        //ResourceManager.Instance.LoadMaterial("material", "material", (mat) =>
        //{
        //    MeshRenderer mr = mObj.GetComponent<MeshRenderer>();
        //    mr.material = mat;
        //});
        //ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite) =>
        //{
        //    mSprite.sprite = sprite;
        //});
        //
        //ResourceManager.Instance.LoadSprite("sprite", "0003", (sprite) =>
        //{
        //    mSprite1.sprite = sprite;
        //});
    }

    // Update is called once per frame
    void Update () {
        if(mStart)
        {
            if(mFrame == 0)
            {
                //ResourceManager.Instance.ReleaseAll();
                ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj) =>
                {
                    obj.name = "testTestxxxx";
                });
            }
            mFrame--;
        }
    }
}
