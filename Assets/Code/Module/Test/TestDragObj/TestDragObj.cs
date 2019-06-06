using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestDragObj : MonoBehaviour
{
    private Transform mDragObjTrans;
    public LayerMask mDragLayer;
    public LayerMask mTargetLayer;
    private Vector3 offset;
    private bool isClickCube;
    public Camera mUICamera;
    public Camera mSceneCamera;
    public static bool mIsInit;

    // Use this for initialization
    void Start()
    {
        mDragLayer = 1 << LayerMask.NameToLayer("drag");
        mTargetLayer = 1 << LayerMask.NameToLayer("target");
        if(!mIsInit)
        {
            mIsInit = true;
            TestGridManager.Instance.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100, mDragLayer))
            {
                isClickCube = true;
                mDragObjTrans = hitInfo.collider.gameObject.transform;
                offset = mDragObjTrans.position - GetTargetPos();
            }
        }

        if (isClickCube)
        {
            mDragObjTrans.position = GetTargetPos() + offset;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isClickCube = false;
            TestDragComplete logic = mDragObjTrans.GetComponent<TestDragComplete>();
            logic.OnDragOver();
        }
    }

    public Vector3 GetTargetPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100, mTargetLayer))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }
}