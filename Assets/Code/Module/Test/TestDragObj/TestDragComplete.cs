using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDragComplete : MonoBehaviour {

    private TestGridManager.ShowGridInfo mShowGridInfo;

    public void OnDragOver()
    {
        if(mShowGridInfo != null)
        {
            TestGridManager.ShowGridInfo preInfo = TestGridManager.Instance.GetShowGridInfo(mShowGridInfo.x, mShowGridInfo.z);
            preInfo.isHave = false;
            preInfo.obj = null;
            TestGridManager.ShowGridInfo preAdjInfo = TestGridManager.Instance.GetAdjacentShowGridInfo(mShowGridInfo.x, mShowGridInfo.z);
            if(preAdjInfo.isHave)
            {
                preAdjInfo.obj.transform.position = TestGridManager.Instance.GetTwoShowGridCenterPos(preInfo.x, preInfo.z, preAdjInfo.x, preAdjInfo.z);
            }
        }

        TestGridManager.ShowGridInfo info = TestGridManager.Instance.GetShowGridInfo(transform.position);
        mShowGridInfo = info;
        if (info.isHave)
        {
            TestGridManager.ShowGridInfo adjInfo = TestGridManager.Instance.GetAdjacentShowGridInfo(info.x, info.z);
            if (adjInfo.isHave)
            {
                if(mShowGridInfo == null)
                {
                    //销毁格子上的对象，然后替换为我们新拖动上去的对象
                    if (info.obj != null)
                    {
                        Destroy(info.obj);
                    }
                    info.obj = gameObject;
                    info.obj.transform.position = TestGridManager.Instance.GetShowGridCenterPos(info.x, info.z);
                }
                else
                {
                    //交换位置
                }
            }
            else
            {
                //将当前格子的对象移动到相邻对象上
                adjInfo.obj = info.obj;
                adjInfo.isHave = true;
                adjInfo.obj.GetComponent<TestDragComplete>().mShowGridInfo = adjInfo;

                info.obj = gameObject;
                info.isHave = true;
                adjInfo.obj.transform.position = TestGridManager.Instance.GetShowGridCenterPos(adjInfo.x, adjInfo.z);
                info.obj.transform.position = TestGridManager.Instance.GetShowGridCenterPos(info.x, info.z);
            }
        }
        else
        {
            info.obj = gameObject;
            info.isHave = true;
            TestGridManager.ShowGridInfo adjInfo = TestGridManager.Instance.GetAdjacentShowGridInfo(info.x, info.z);
            if (adjInfo.isHave)
            {
                info.obj.transform.position = TestGridManager.Instance.GetShowGridCenterPos(info.x, info.z);
                adjInfo.obj.transform.position = TestGridManager.Instance.GetShowGridCenterPos(adjInfo.x, adjInfo.z);
            }
            else
            {
                info.obj.transform.position = TestGridManager.Instance.GetTwoShowGridCenterPos(info.x, info.z, adjInfo.x, adjInfo.z);
            }
        }
    }

    public void ExchangeShowGrid()
    {

    }
}
