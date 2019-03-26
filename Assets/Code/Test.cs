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
            ResourceManager.Instance.ReleaseAll();
        });

        //Material material = ResourceManager.Instance.LoadMaterialSync("material", "material");
        //mObj.GetComponent<MeshRenderer>().sharedMaterial = material;

        //AudioClip audioclip = ResourceManager.Instance.LoadAudioClip("sleep", "sleep");
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
        //ResourceManager.Instance.ReleaseAll();
        //ResourceManager.Instance.LoadPrefab("cube", "cube", (Obj1) =>
        //{
        //    if (Obj1 != null)
        //        Obj1.name = "test33";
        //});
        //GameObject Objt = ResourceManager.Instance.LoadPrefabSync("cube", "cube");
    }

    // Update is called once per frame
    void Update () {
        if(mStart)
        {
            if(mFrame == 0)
            {
                ResourceManager.Instance.ReleaseAll();
            }
            mFrame--;
        }
    }
}
