using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarMain : MonoBehaviour {

    public Button mButton;
    public Button mButton1;

    // Use this for initialization
    void Start () {
        GameObject obj = AvatarManager.Instance.LoadAvatar(1);

        mButton.onClick.AddListener(() =>
        {
            AvatarManager.Instance.ChangePart(obj, Avatar.PartType.eBody, "Female/female_top-1_pink");
        });

        mButton1.onClick.AddListener(() =>
        {
            AvatarManager.Instance.ChangeMaterial(obj, Avatar.PartType.eBody, "Materials/Female/female_top-1_blue");
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
