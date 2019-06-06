using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestGridManager : Singleton<TestGridManager>
{
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
        public GameObject obj;
    }

    private List<List<LogicGridInfo>> mLogicGridMap = new List<List<LogicGridInfo>>();
    private List<List<ShowGridInfo>> mShowGridMap = new List<List<ShowGridInfo>>();

    // Use this for initialization
    public void Init()
    {
        mTotalXSize = mLogicGridXNum * mLogicGridXSize;
        mTotalZSize = mLogicGridZNum * mLogicGridZSize;

        for (int i = 0; i < mLogicGridXNum; i++)
        {
            List<LogicGridInfo> gridInfoList = new List<LogicGridInfo>();
            for (int j = 0; j < mLogicGridZNum; j++)
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
    }

    public bool CheckLogicGridIndex(int x, int z)
    {
        if (x >= 0 && x < mLogicGridXNum && z >= 0 && z < mLogicGridZNum)
        {
            return true;
        }

        return false;
    }

    public bool CheckShowGridIndex(int x, int z)
    {
        if (x >= 0 && x < mShowGridXNum && z >= 0 && z < mShowGridZNum)
        {
            return true;
        }

        return false;
    }

    public LogicGridInfo GetLogicGridInfo(Vector3 pos)
    {
        int x = (int)pos.x;
        int z = (int)pos.z;
        if (CheckLogicGridIndex(x, z))
        {
            return mLogicGridMap[x][z];
        }
        else
        {
            return null;
        }
    }

    //根据下标获取显示格子信息
    public ShowGridInfo GetShowGridInfo(int x, int z)
    {
        if (CheckShowGridIndex(x, z))
        {
            return mShowGridMap[x][z];
        }

        return null;
    }

    //根据pos获取显示格子信息
    public ShowGridInfo GetShowGridInfo(Vector3 pos)
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

    //获取x的相邻格子下标
    public int GetXAdjacentIndex(int x)
    {
        return x == 0 ? 1 : 0;
    }

    //获取相邻格子的信息
    public ShowGridInfo GetAdjacentShowGridInfo(int x, int z)
    {
        int tempX = GetXAdjacentIndex(x);
        if (CheckShowGridIndex(tempX, z))
        {
            return mShowGridMap[tempX][z];
        }

        return null;
    }

    //获取一个格子的中心坐标
    public Vector3 GetShowGridCenterPos(Vector3 pos)
    {
        int x = (int)(pos.x / mShowGridXSize);
        int z = (int)(pos.z / mShowGridZSize);
        return GetShowGridCenterPos(x, z);
    }

    //获取一个格子的中心坐标
    public Vector3 GetShowGridCenterPos(int x, int z)
    {
        if (CheckLogicGridIndex(x, z))
        {
            int posX = x * mShowGridXSize + mShowGridXSize / 2;
            int posZ = z * mShowGridZSize + mShowGridZSize / 2;
            return new Vector3(posX, 0, posZ);
        }

        return Vector3.zero;
    }

    //获取两个格子的中心坐标
    public Vector3 GetTwoShowGridCenterPos(int x1, int z1, int x2, int z2)
    {
        Vector3 grid1CenterPos = GetShowGridCenterPos(x1, z1);
        Vector3 grid2CenterPos = GetShowGridCenterPos(x2, z2);

        return (grid1CenterPos + grid2CenterPos) / 2;
    }
}