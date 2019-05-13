﻿using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSpriteAnimation : MonoBehaviour {

    public Button mButton1;
    public Button mButton2;

    // Use this for initialization
    void Start () {
        //mAnimation = GetComponent<Animation>();
        //mAnimation.wrapMode = WrapMode.Loop;

        ArmSpriteAnimationAction action = GetComponent<ArmSpriteAnimationAction>();
        action.Init("archer_sprite", 16, SpriteAnimation.AnimationWrapMode.eLoop, () =>
        {
            action.SetDir(0);
            action.Play("attack");
        });

        mButton1.onClick.AddListener(() =>
        {
            action.Play("idle");
        });
        
        mButton2.onClick.AddListener(() =>
        {
            action.Play("die", true);
        });
    }
	
	// Update is called once per frame
	void Update () {

    }
}