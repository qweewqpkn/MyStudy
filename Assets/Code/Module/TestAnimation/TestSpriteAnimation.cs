using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSpriteAnimation : MonoBehaviour {

    public Button mButton1;
    public Button mButton2;
    public Button mButton3;

    // Use this for initialization
    void Start () {
        //mAnimation = GetComponent<Animation>();
        //mAnimation.wrapMode = WrapMode.Loop;

 
        ArmSpriteAnimation action = GetComponent<ArmSpriteAnimation>();
        action.Init("archer_sprite", 1, SpriteAnimation.AnimationWrapMode.eLoop);
        
        mButton1.onClick.AddListener(() =>
        {
            action.SetDir(4);
            action.Play("attack");
            action.SetDir(3);
        });
        
        mButton2.onClick.AddListener(() =>
        {
            action.SetDir(1);
        });

        mButton3.onClick.AddListener(() =>
        {
            action.SetDir(2);
        });
    }
	
	// Update is called once per frame
	void Update () {

    }
}
