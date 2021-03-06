﻿using BezierSolution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrail {
    private Vector3 mInitTargetPos;
    private GameObject mStartObj;
    private GameObject mTargetObj;
    private float mMoveSpeed;
    private float mRotationSpeed;
    private BezierWalkerWithSpeed mBezierWalker;
    private enum TrialMode { eBezier, eCustom }
    private TrialMode mTrialMode;
    public bool mIsApply = true;

    public void Create(GameObject startObj, GameObject targetObj)
    {
        mInitTargetPos = targetObj.transform.position;
        mTargetObj = targetObj;
        mStartObj = startObj;
        mMoveSpeed = 10;
        mRotationSpeed = 10;
        mTrialMode = TrialMode.eBezier;
        CreateBeizer(startObj, targetObj);
    }

    private void CreateBeizer(GameObject startObj, GameObject targetObj)
    {
        GameObject splineObj = new GameObject();
        BezierSpline spline = splineObj.AddComponent<BezierSpline>();
        spline.Initialize(3);
        for (int i = 0; i < spline.Count; i++)
        {
            BezierPoint point = spline[i];
            if (i == 0)
            {
                point.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                point.followingControlPointLocalPosition = new Vector3(0.0f, 30.0f, 0.0f);
            }
            else if (i == 1)
            {
                point.localPosition = new Vector3(0.0f, -5f, 10.0f);
                point.followingControlPointLocalPosition = new Vector3(0.0f, -30.0f, 0.0f);
            }
            else if (i == 2)
            {
                point.localPosition = new Vector3(0.0f, 5f, 20.0f);
                point.followingControlPointLocalPosition = new Vector3(0.0f, -30.0f, 0.0f);
            }
        }

        if(mIsApply)
        {
            splineObj.transform.position = startObj.transform.position;
            Vector3 templateDir = spline[spline.Count - 1].position - spline[0].position;
            Vector3 actualDir = targetObj.transform.position - startObj.transform.position;
            splineObj.transform.rotation = Quaternion.FromToRotation(templateDir, actualDir);
            templateDir = spline[spline.Count - 1].position - spline[0].position;
            float scale = actualDir.magnitude / templateDir.magnitude;
            splineObj.transform.localScale = new Vector3(scale, scale, scale);

            mBezierWalker = startObj.AddComponent<BezierWalkerWithSpeed>();
            mBezierWalker.Spline = spline;
        }
    }
	
	// Update is called once per frame
	public void Update () {
        if(mInitTargetPos != mTargetObj.transform.position)
        {
            mTrialMode = TrialMode.eCustom;
            mBezierWalker.enabled = false;
        }

        if(mTrialMode == TrialMode.eCustom)
        {
            Vector3 translation = mTargetObj.transform.position - mStartObj.transform.position;
            if (translation.magnitude > 0.1f)
            {
                mStartObj.transform.Translate(Vector3.Normalize(translation) * Time.deltaTime * mMoveSpeed, Space.World);
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.Normalize(translation));
                mStartObj.transform.rotation = Quaternion.Lerp(mStartObj.transform.rotation, targetRotation, mRotationSpeed * Time.deltaTime);
            }
            else
            {
                mStartObj.transform.position = mTargetObj.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.Normalize(translation));
                mStartObj.transform.rotation = targetRotation;
            }
        }
	}
}
