using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestDragObj : MonoBehaviour
{
    private Transform dragGameObject;
    public LayerMask mDragLayer;
    public LayerMask mTargetLayer;
    private Vector3 offset;
    private bool isClickCube;
    public Grid mGrid;
    public Camera mUICamera;
    public Camera mSceneCamera;

    public int mLogicGridXNum = 20;
    public int mLogicGridZNum = 20;
    public int mLogicGridXSize = 1;
    public int mLogicGridZSize = 1;

    public int mShowGridXNum = 2;
    public int mShowGridZNum = 2;
    public int mShowGridXSize = 10;
    public int mShowGridZSize = 10;

    public int mTotalXSize;
    public int mTotalZSize;

    public class LogicGridInfo
    {
        public int x;
        public int z;
        public bool isHave;
    }

    public class ShowGridInfo
    {
        public int x;
        public int z;
        public bool isHave;
    }

    private List<List<LogicGridInfo>> mLogicGridMap = new List<List<LogicGridInfo>>();
    private List<List<ShowGridInfo>> mShowGridMap = new List<List<ShowGridInfo>>();

    // Use this for initialization
    void Start()
    {
        mTotalXSize = mLogicGridXNum * mLogicGridXSize;
        mTotalZSize = mLogicGridZNum * mLogicGridZSize;

        for (int i = 0; i < mLogicGridXNum; i++)
        {
            List<LogicGridInfo> gridInfoList = new List<LogicGridInfo>();
            for(int j = 0; j < mLogicGridZNum; j++)
            {
                LogicGridInfo gridInfo = new LogicGridInfo();
                gridInfo.x = i;
                gridInfo.z = j;
                gridInfoList.Add(gridInfo);
            }
            mLogicGridMap.Add(gridInfoList);
        }

        for (int i = 0; i < mShowGridXNum; i++)
        {
            List<ShowGridInfo> gridInfoList = new List<ShowGridInfo>();
            for (int j = 0; j < mShowGridZNum; j++)
            {
                ShowGridInfo gridInfo = new ShowGridInfo();
                gridInfo.x = i;
                gridInfo.z = j;
                gridInfoList.Add(gridInfo);
            }
            mShowGridMap.Add(gridInfoList);
        }

        mDragLayer = 1 << LayerMask.NameToLayer("drag");
        mTargetLayer = 1 << LayerMask.NameToLayer("target");
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
                dragGameObject = hitInfo.collider.gameObject.transform;
                offset = dragGameObject.transform.position - GetTargetPos();
            }
        }

        if (isClickCube)
        {
            dragGameObject.position = GetTargetPos() + offset;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isClickCube = false;
            ShowGridInfo info = ConverPosToShowGridInfo(dragGameObject.position);
            if (IsXHaveAdjacent(info.x, info.z))
            {
                dragGameObject.position = GetShowGridCenterPos(dragGameObject.position);
            }
            else
            {
                dragGameObject.position = GetAdjacentShowGridCenterPos(info.x, info.z, GetXAdjacentIndex(info.x), info.z);
            }
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

    public bool CheckLogicGridIndex(int x, int z)
    {
        if (x >= 0 && x < mLogicGridXNum && z >= 0 && z < mLogicGridZNum)
        {
            return true;
        }

        return false;
    }

    public LogicGridInfo ConverPosToLogicGridInfo(Vector3 pos)
    {
        int x = (int)pos.x;
        int z = (int)pos.z;
        if(CheckLogicGridIndex(x, z))
        {
            return mLogicGridMap[x][z];
        }
        else
        {
            return null;
        }
    }


    public bool CheckShowGridIndex(int x, int z)
    {
        if(x >= 0 && x < mShowGridXNum && z >= 0 && z < mShowGridZNum)
        {
            return true;
        }

        return false;
    }

    public ShowGridInfo ConverPosToShowGridInfo(Vector3 pos)
    {
        int x = (int)(pos.x / mShowGridXSize);
        int z = (int)(pos.z / mShowGridZSize);
        if (CheckShowGridIndex(x, z))
        {
            return mShowGridMap[x][z];
        }
        else
        {
            return null;
        }
    }

    public Vector3 GetShowGridCenterPos(Vector3 pos)
    {
        int x = (int)(pos.x / mShowGridXSize);
        int z = (int)(pos.z / mShowGridZSize);
        return GetShowGridCenterPos(x, z);
    }

    public Vector3 GetShowGridCenterPos(int x, int z)
    {
        if(CheckLogicGridIndex(x, z))
        {
            int posX = x * mShowGridXSize + mShowGridXSize / 2;
            int posZ = z * mShowGridZSize + mShowGridZSize / 2;
            return new Vector3(posX, 0, posZ);
        }

        return Vector3.zero;
    }

    public Vector3 GetAdjacentShowGridCenterPos(int grid1X, int grid1Z, int grid2X, int grid2Z)
    {
        Vector3 grid1CenterPos = GetShowGridCenterPos(grid1X, grid1Z);
        Vector3 grid2CenterPos = GetShowGridCenterPos(grid2X, grid2Z);

        return (grid1CenterPos + grid2CenterPos) / 2;
    }

    public bool IsXHaveAdjacent(int x, int z)
    {
        int tempX = GetXAdjacentIndex(x);
        if (CheckShowGridIndex(tempX, z))
        {
            if(mShowGridMap[tempX][z].isHave)
            {
                return true;
            }
        }

        return false;
    }

    public int GetXAdjacentIndex(int x)
    {
        if(mShowGridXNum == 2)
        {
            return x == 0 ? 1 : 0;
        }
        else
        {
            int tempX = x + 1;
            if(tempX > mShowGridXNum)
            {
                tempX = x - 1;
            }

            return tempX;
        }
    }
}